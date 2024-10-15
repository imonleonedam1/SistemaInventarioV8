using SistemaInventarioV8.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio
{
    public interface ICarroCompraRepositorio : IRepositorio<CarroCompra>
    {

        void Actualizar(CarroCompra carroCompra);
    }
}
