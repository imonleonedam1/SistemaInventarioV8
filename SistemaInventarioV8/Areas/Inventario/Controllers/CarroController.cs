using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarioV8.Modelos;
using SistemaInventarioV8.Modelos.ViewModels;
using SistemaInventarioV8.Utilidades; 
using Stripe.Checkout;
using System.Security.Claims;

namespace SistemaInventarioV8.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private string _webUrl;
        [BindProperty]
        public CarroCompraVM carroCompraVM { get; set; }
        public CarroController(IUnidadTrabajo unidadTrabajo, IConfiguration configuracion)
        {
            _unidadTrabajo = unidadTrabajo;
            _webUrl = configuracion.GetValue<string>("DomainUrls:WEB_URL");
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM
            {
                Orden = new Modelos.Orden(),
                CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(
                                                                  u => u.UsuarioAplicacionId == claim.Value,
                                                                  incluirPropiedades: "Producto")
            };
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
        public async Task<IActionResult> Proceder()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM = new CarroCompraVM
            {
                Orden = new Modelos.Orden(),
                CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(
                                                                  u => u.UsuarioAplicacionId == claim.Value,
                                                                  incluirPropiedades: "Producto"),
                Compania = await _unidadTrabajo.Compania.ObtenerPrimero()
            };
            carroCompraVM.Orden.PrecioTotal = 0;
            carroCompraVM.Orden.UsuarioAplicacion = await _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => 
                                                                                            u.Id == claim.Value);

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio; //precio actual del producto
                carroCompraVM.Orden.PrecioTotal += (lista.Precio * lista.Cantidad);
            }
            carroCompraVM.Orden.NombreCliente = carroCompraVM.Orden.UsuarioAplicacion.Nombres + " " +
                                                carroCompraVM.Orden.UsuarioAplicacion.Apellidos;
            carroCompraVM.Orden.Telefono = carroCompraVM.Orden.UsuarioAplicacion.PhoneNumber;
            carroCompraVM.Orden.Direccion = carroCompraVM.Orden.UsuarioAplicacion.Direccion;
            carroCompraVM.Orden.Pais = carroCompraVM.Orden.UsuarioAplicacion.Pais;
            carroCompraVM.Orden.Ciudad = carroCompraVM.Orden.UsuarioAplicacion.Ciudad;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                var producto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(p => p.ProductoId == lista.ProductoId &&
                                                                                       p.BodegaId == carroCompraVM.Compania.BodegaVentaId);

                if (lista.Cantidad > producto.Cantidad)
                {
                    TempData[DS.Error] = "Cantidad del producto " + lista.Producto.Descripcion + " excede el stock actual (" + producto.Cantidad + ")";
                    return RedirectToAction("Index");
                }
            }

            return View(carroCompraVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceder(CarroCompraVM carroCompraVM)
        { 
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioAplicacionId == claim.Value,
                                                                                                incluirPropiedades:"Producto");
            carroCompraVM.Compania = await _unidadTrabajo.Compania.ObtenerPrimero();
            carroCompraVM.Orden.PrecioTotal = 0;
            carroCompraVM.Orden.UsuarioAplicacionId = claim.Value;
            carroCompraVM.Orden.FechaCreacion = DateTime.Now;

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                lista.Precio = lista.Producto.Precio;
                carroCompraVM.Orden.PrecioTotal += (lista.Precio + lista.Cantidad);
            }

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                var producto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(p => p.ProductoId == lista.ProductoId &&
                                                                                       p.BodegaId == carroCompraVM.Compania.BodegaVentaId);

                if (lista.Cantidad > producto.Cantidad)
                {
                    TempData[DS.Error] = "Cantidad del producto " + lista.Producto.Descripcion + " excede el stock actual (" + producto.Cantidad + ")";
                    return RedirectToAction("Index");
                }
            }
            carroCompraVM.Orden.Estado = DS.EstadoPendiente;
            carroCompraVM.Orden.EstadoPago = DS.PagoEstadoPendiente;
            await _unidadTrabajo.Orden.Agregar(carroCompraVM.Orden);
            await _unidadTrabajo.Guardar();

            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                { 
                    ProductoId = lista.ProductoId,
                    OrdenId = carroCompraVM.Orden.Id,
                    Precio = lista.Precio,
                    Cantidad = lista.Cantidad
                };
                await _unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);
                await _unidadTrabajo.Guardar();
            }
            var usuario = await _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == claim.Value);
            var options = new SessionCreateOptions
            {
                SuccessUrl = _webUrl + $"inventario/carro/OrdenConfirmacion?id={carroCompraVM.Orden.Id}",
                CancelUrl = _webUrl + "inventario/carro/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = usuario.Email
            };
            foreach (var lista in carroCompraVM.CarroCompraLista)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(lista.Precio * 100),
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = lista.Producto.Descripcion
                        }
                        
                    },
                    Quantity = lista.Cantidad
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unidadTrabajo.Orden.ActualizarPagoStripeId(carroCompraVM.Orden.Id, session.Id, session.PaymentIntentId);
            await _unidadTrabajo.Guardar();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); 
        }

        public async Task<IActionResult> OrdenConfirmacion(int id)
        {
            var orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades:"UsuarioAplicacion");
            var service = new SessionService();
            Session session = service.Get(orden.SessionId);
            var carroCompra = await _unidadTrabajo.CarroCompra.ObtenerTodos(u => u.UsuarioAplicacionId == orden.UsuarioAplicacionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unidadTrabajo.Orden.ActualizarPagoStripeId(id, session.Id, session.PaymentIntentId);
                _unidadTrabajo.Orden.ActualizarEstado(id, DS.EstadoAprobado, DS.PagoEstadoAprobado);
                await _unidadTrabajo.Guardar();

                var compania = await _unidadTrabajo.Compania.ObtenerPrimero();
                foreach (var lista in carroCompra)
                {
                    var bodegaProducto = new BodegaProducto();
                    bodegaProducto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(b => b.ProductoId == lista.ProductoId
                                                                                            && b.BodegaId == compania.BodegaVentaId); 
                    await _unidadTrabajo.KardexInventario.RegistrarKardex(bodegaProducto.Id, "Salida",
                                                                            "Venta - Orden# " + id, bodegaProducto.Cantidad,
                                                                            lista.Cantidad, orden.UsuarioAplicacionId);
                    bodegaProducto.Cantidad -= lista.Cantidad;
                    await _unidadTrabajo.Guardar();
                }

            }
            List<CarroCompra> carroCompraLista = carroCompra.ToList();
            _unidadTrabajo.CarroCompra.RemoverRango(carroCompraLista);
            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompra,0);

            return View(id);
        }
    }
}
