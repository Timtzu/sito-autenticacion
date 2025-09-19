
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sito_autenticacion.Model;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace sito_autenticacion.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // Or, specify precision and scale separately

            
        }

        public DbSet<Usuario> Users { get; set; }
        

    }
}