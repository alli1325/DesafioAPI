using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DesafioAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace DesafioAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Starter> Starters { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>().HasData
            (
                new Categoria
                {
                    Id = 1,
                    Nome = "Turma-1",
                    Tecnologia = "C#"
                },
                new Categoria
                {
                    Id = 2,
                    Nome = "Turma-2",
                    Tecnologia = "Java"
                },
                new Categoria
                {
                    Id = 3,
                    Nome = "Turma-3",
                    Tecnologia = "PHP"
                }
            );

            modelBuilder.Entity<Starter>().HasData
            (
                new Starter
                {
                    Id = 1,
                    Cpf = "11111111111",
                    Email = "allison@starter.com",
                    Letra = "ALOR",
                    Nome = "Allison",
                    Foto = "padra.png"
                },
                new Starter
                {
                    Id = 2,
                    Cpf = "22222222222",
                    Email = "clecio@starter.com",
                    Letra = "CLCI",
                    Nome = "Clécio",
                    Foto = "padra.png"
                },
                new Starter
                {
                    Id = 3,
                    Cpf = "33333333333",
                    Email = "ubiratan@starter.com",
                    Letra = "UBRT",
                    Nome = "Ubiratan",
                    Foto = "padra.png"
                },
                new Starter
                {
                    Id = 4,
                    Cpf = "44444444444",
                    Email = "joao@starter.com",
                    Letra = "JOAO",
                    Nome = "João",
                    Foto = "padra.png"
                },
                new Starter
                {
                    Id = 5,
                    Cpf = "55555555555",
                    Email = "Antonio@starter.com",
                    Letra = "ANTN",
                    Nome = "Caio",
                    Foto = "padra.png"
                }

                
            );

            modelBuilder.Entity<Usuario>().HasData
            (
                new
                {
                    Id = 1,
                    Email = "admin@admin",
                    Role = "Admin",
                    Senha = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Gft@1234")).ToString(),
                    User = "Admin"
                },
                new
                {
                    Id = 2,
                    Email = "usuario@usuario",
                    Role = "Usuario",
                    Senha = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Gft@1234")).ToString(),
                    User = "Usuario"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}