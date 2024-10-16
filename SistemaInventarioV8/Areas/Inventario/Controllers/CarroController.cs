using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarioV8.Modelos.ViewModels;
using SistemaInventarioV8.Utilidades;
using System.Security.Claims;

namespace SistemaInventarioV8.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        public CarroCompraVM carroCompraVM { get; set; }
        public CarroController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM();
            carroCompraVM.Orden = new Modelos.Orden();
            carroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(
                                                                  u => u.UsuarioAplicacionId == claim.Value,
                                                                  incluirPropiedades: "Producto");
            carroCompraVM.Orden.PrecioTotal = 0;
            carroCompraVM.Orden.UsuarioAplicacionId = claim.Value;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio; //precio actual del producto
                carroCompraVM.Orden.PrecioTotal += (lista.Precio * lista.Cantidad);
            }

            return View(carroCompraVM);
        }

        public async Task<IActionResult> mas(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.Id == carroId);
            carroCompras.Cantidad += 1;
            await _unidadTrabajo.Guardar();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> menos(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.Id == carroId);
            if (carroCompras.Cantidad == 1)
            {
                var carroLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c =>
                                                    c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);
                var numeroProductos = carroLista.Count();
                _unidadTrabajo.CarroCompra.Remover(carroCompras);
                await _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompra, numeroProductos - 1);
            }
            else
            {
                carroCompras.Cantidad -= 1;
                await _unidadTrabajo.Guardar();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> remover(int carroId)
        {
            var carroCompras = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.Id == carroId); 
            var carroLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c =>
                                                c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId);
            var numeroProductos = carroLista.Count();
            _unidadTrabajo.CarroCompra.Remover(carroCompras);
            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompra, numeroProductos - 1); 
            return RedirectToAction("Index");
        }
    }
}
