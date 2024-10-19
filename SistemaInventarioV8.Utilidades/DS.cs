using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.Utilidades
{
    public static class DS
    {
        public const string Exitosa = "Exitosa";
        public const string Error = "Error";

        public const string ImagenRuta = @"\imagenes\producto\";
        public const string ssCarroCompra = "Sesion carro Compras";

        public const string RoleAdmin = "Admin";
        public const string RoleCliente = "Cliente";
        public const string RoleInventario = "Inventario";

        public const string EstadoPendiente = "Pendiente";
        public const string EstadoAprobado = "Aprobado";
        public const string EstadoEnProceso = "Procesando";
        public const string EstadoEnviado = "Enviado";
        public const string EstadoCancelado = "Cancelado";
        public const string EstadoDevuelto = "Devuelto";

        public const string PagoEstadoPendiente = "Pendiente";
        public const string PagoEstadoAprobado = "Aprobado";
        public const string PagoEstadoRetrasado = "Retrasado";
        public const string PagoEstadoRechazado = "Rechazado";
    }
}
