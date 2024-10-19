using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarioV8.Modelos;
using SistemaInventarioV8.Modelos.ViewModels;
using SistemaInventarioV8.Utilidades;
using System.Security.Claims;

namespace SistemaInventarioV8.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        public OrdenDetalleVM ordenDetalleVM { get; set; }

        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detalle(int id)
        {
            ordenDetalleVM = new OrdenDetalleVM() 
            { 
                Orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades:"UsuarioAplicacion"),
                OrdenDetalleLista = await _unidadTrabajo.OrdenDetalle.ObtenerTodos(d => d.OrdenId == id,
                                                                                         incluirPropiedades:"Producto")
            };
            return View(ordenDetalleVM);
        }

        [Authorize(Roles = DS.RoleAdmin)]
        public async Task<IActionResult> Procesar(int id)
        {
            var orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id);
            orden.Estado = DS.EstadoEnProceso;
            await _unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Orden cambiada a estado en proceso";
            return RedirectToAction("Detalle", new {id = id});
        }

        [HttpPost]
        [Authorize(Roles = DS.RoleAdmin)]
        public async Task<IActionResult> EnviarOrden(OrdenDetalleVM ordenDetalleVM)
        {
            var orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == ordenDetalleVM.Orden.Id);
            orden.Estado = DS.EstadoEnviado;
            orden.Carrier = ordenDetalleVM.Orden.Carrier;
            orden.NumeroEnvio = ordenDetalleVM.Orden.NumeroEnvio;
            orden.FechaEnvio = ordenDetalleVM.Orden.FechaEnvio;
            await _unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Orden cambiada a estado enviado";
            return RedirectToAction("Detalle", new { id = ordenDetalleVM.Orden.Id });
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenLista(string estado)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Orden> todos;
            if (User.IsInRole(DS.RoleAdmin))
            {
                todos = await _unidadTrabajo.Orden.ObtenerTodos(incluirPropiedades: "UsuarioAplicacion");
            }
            else
            {
                todos = await _unidadTrabajo.Orden.ObtenerTodos(o => o.UsuarioAplicacionId == claim.Value,incluirPropiedades: "UsuarioAplicacion");
            }
            switch (estado)
            {
                case "aprobado":
                    todos = todos.Where(o=>o.Estado == DS.EstadoAprobado);
                    break;
                case "completado":
                    todos = todos.Where(o => o.Estado == DS.EstadoEnviado);
                    break;
                default: 
                    todos = todos.Where(o => o.Estado != DS.EstadoPendiente);
                    break;
            }

            return Json(new { data = todos });
        }
        #endregion
    }
}
