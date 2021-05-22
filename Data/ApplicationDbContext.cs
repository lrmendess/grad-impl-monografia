using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SCAP.Data.Seeds;
using SCAP.Models;
using SCAP.ViewModels;

namespace SCAP.Data
{
    public class ApplicationDbContext : IdentityDbContext<Pessoa>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // Desabilitando o tracking de objetos, pois estamos utilizando ViewModels
            // e métodos para operações de banco de dados.
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;

            Database.Migrate();
        }

        public DbSet<Afastamento> Afastamentos { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Mandato> Mandatos { get; set; }
        public DbSet<Parecer> Pareceres { get; set; }
        public DbSet<Parentesco> Parentescos { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<Secretario> Secretarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mudança da deleção Cascade de todas as entidades para Restrict (exceto as do Identity)
            // Isso só foi feito para que esse fosse o único DbContext da aplicação
            var entityTypes = modelBuilder.Model.GetEntityTypes().Except(
                new List<IMutableEntityType>
                {
                    modelBuilder.Model.FindEntityType(typeof(IdentityUser)),
                    modelBuilder.Model.FindEntityType(typeof(IdentityRole)),
                    modelBuilder.Model.FindEntityType(typeof(IdentityUserRole<string>)),
                    modelBuilder.Model.FindEntityType(typeof(IdentityUserClaim<string>)),
                    modelBuilder.Model.FindEntityType(typeof(IdentityUserLogin<string>)),
                    modelBuilder.Model.FindEntityType(typeof(IdentityUserToken<string>)),
                    modelBuilder.Model.FindEntityType(typeof(IdentityRoleClaim<string>))
                }
            );

            foreach (var relationship in entityTypes.SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            modelBuilder.SeedRoles();
        }
    }
}
