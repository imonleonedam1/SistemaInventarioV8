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
    public class BodegaRepositorio : Repositorio<Bodega>, IBodegaRepositorio
    {
private readonly ApplicationDbContext _db;

        public BodegaRepositorio(ApplicationDbContext db): base(db) 
        {
            _db = db;
        }
        public void Acualizar(Bodega bodega)
        {
            var bodegaBD = _db.Bodegas.FirstOrDefault(b => b.Id == bodega.Id);

            if (bodegaBD != null) { 
                bodegaBD.Nombre = bodega.Nombre;
                bodegaBD.Descripcion = bodega.Descripcion;
                bodegaBD.Estado = bodega.Estado;
                _db.SaveChanges();
            }
        }
    }
}
