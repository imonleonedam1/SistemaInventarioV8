using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class InventarioDetalleRepositorio : Repositorio<InventarioDetalle>, IInventarioDetalleRepositorio
    {
        private readonly ApplicationDbContext _db;

        public InventarioDetalleRepositorio(ApplicationDbContext db): base(db) 
        {
            _db = db;
        }
        public void Actualizar(InventarioDetalle inventarioDetalle)
        {
            var inventarioDetalleBD = _db.InventarioDetalles.FirstOrDefault(b => b.Id == inventarioDetalle.Id);

            if (inventarioDetalleBD != null) 
            {
                inventarioDetalleBD.StockAnterior = inventarioDetalle.StockAnterior;
                inventarioDetalleBD.Cantidad = inventarioDetalle.Cantidad; 

                _db.SaveChanges();
            }
        }
    }
}
