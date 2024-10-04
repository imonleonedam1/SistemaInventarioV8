using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventarioV8.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.AccesoDatos.Repositorio.IRepositorio
{
    public interface IInventarioRepositorio : IRepositorio<Inventario>
    { 
        void Actualizar(Inventario inventario);
        IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj);
    }
}
