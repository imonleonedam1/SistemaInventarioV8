using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaInventarioV8.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.AccesoDatos.Configuracion
{
    public class OrdenConfiguracion : IEntityTypeConfiguration<Orden>
    {
        public void Configure(EntityTypeBuilder<Orden> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UsuarioAplicacionId).IsRequired(); 
            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.PrecioTotal).IsRequired(); 
            builder.Property(x => x.Estado).IsRequired(); 
            builder.Property(x => x.EstadoPago).IsRequired();
            builder.Property(x => x.NombreCliente).IsRequired();
            builder.Property(x => x.NumeroEnvio).IsRequired(false); 
            builder.Property(x => x.Carrier).IsRequired(false); 
            builder.Property(x => x.TransaccionId).IsRequired(false);
            builder.Property(x => x.Telefono).IsRequired(false);
            builder.Property(x => x.Direccion).IsRequired(false);
            builder.Property(x => x.Ciudad).IsRequired(false);
            builder.Property(x => x.Pais).IsRequired(false);
            builder.Property(x => x.SessionId).IsRequired(false);

            builder.HasOne(x => x.UsuarioAplicacion).WithMany()
                .HasForeignKey(x => x.UsuarioAplicacionId)
                .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
