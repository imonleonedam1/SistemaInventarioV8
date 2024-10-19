using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioV8.AccesoDatos.Data;
using SistemaInventarioV8.Modelos;
using SistemaInventarioV8.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.AccesoDatos.Inicializador
{
    public class DbInicializador : IDbInicializador
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInicializador(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager; 
            _roleManager = roleManager;
        }

        public void Inicializar()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (_db.Roles.Any(r => r.Name == DS.RoleAdmin)) return;

            _roleManager.CreateAsync(new IdentityRole(DS.RoleAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.RoleCliente)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.RoleInventario)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new UsuarioAplicacion 
            { 
                UserName = "ibai.monleon@itequia.com",
                Email = "ibai.monleon@itequia.com",
                EmailConfirmed = true,
                Nombres = "Ibai",
                Apellidos = "Monleon Elia"
            }, "Admin123*").GetAwaiter().GetResult();

            UsuarioAplicacion usuario = _db.UsuarioAplicacion.Where(u => u.UserName == "ibai.monleon@itequia.com").FirstOrDefault();
            _userManager.AddToRoleAsync(usuario, DS.RoleAdmin).GetAwaiter().GetResult();
        }
    }
}
