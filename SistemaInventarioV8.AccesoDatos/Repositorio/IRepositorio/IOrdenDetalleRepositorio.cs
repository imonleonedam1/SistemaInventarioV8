using SistemaInventarioV8.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio
{
    public interface IOrdenDetalleRepositorio : IRepositorio<OrdenDetalle>
    {

        void Actualizar(OrdenDetalle ordenDetalle);
    }
}
