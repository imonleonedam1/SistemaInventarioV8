using SistemaInventarioV8.AccesoDatos.Data;
using SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarioV8.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.AccesoDatos.Repositorio
{
    public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        private readonly ApplicationDbContext _db;

        public OrdenRepositorio(ApplicationDbContext db): base(db) 
        {
            _db = db;
        }
        public void Actualizar(Orden orden)
        {
            _db.Update(orden);
        }

        public void ActualizarEstado(int id, string ordenEstado, string pagoEstado)
        {
            var ordenBD = _db.Ordenes.FirstOrDefault(o => o.Id == id);
            if (ordenBD != null)
            {
                ordenBD.Estado = ordenEstado;
                ordenBD.EstadoPago = pagoEstado;
            }
        }

        public void ActualizarPagoStripeId(int id, string sessionId, string transaccionId)
        {
            var ordenBD = _db.Ordenes.FirstOrDefault(o => o.Id == id);
            if (ordenBD != null)
            {
                if (!String.IsNullOrEmpty(sessionId))
                {
                    ordenBD.SessionId = sessionId;
                }
                if (!String.IsNullOrEmpty(transaccionId))
                {
                    ordenBD.TransaccionId = transaccionId;
                    ordenBD.FechaPago = DateTime.Now;
                }
            }
        }
    }
}
