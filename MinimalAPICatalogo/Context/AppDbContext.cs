﻿using Microsoft.EntityFrameworkCore;
using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        //Mapeamento das entidades - DBSet
        public DbSet<Produto>? Produtos { get; set; }
        public DbSet<Categoria>? Categorias { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // usar os metodos da fluentapi para configurar as propriedades das entitidades
            // e explicitar o relacionamento 1 para muitos

            modelBuilder.Entity<Categoria>().HasKey(c => c.CategoriaId); //Para a tabela categoria, categoriaID é usado como chave primária
            //Definição das colunas
            modelBuilder.Entity<Categoria>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Categoria>().Property(c => c.Descricao).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Produto>().HasKey(c => c.ProdutoId);
            modelBuilder.Entity<Produto>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Produto>().Property(c => c.Descricao).HasMaxLength(150);
            modelBuilder.Entity<Produto>().Property(c => c.Imagem).HasMaxLength(100);

            modelBuilder.Entity<Produto>().Property(c => c.Preco).HasPrecision(14, 2);

            //relacionamento categoria-produto

            modelBuilder.Entity<Produto>()
                .HasOne<Categoria>(c => c.Categoria) //metodo HasOne -  especifica um lado do relacionamento
                .WithMany(p => p.Produtos)   //metodo WithMany - define que a propriedade tem um relacionamento tipo muitos
                .HasForeignKey(y => y.CategoriaID); // definido a chave estrangeira do produto
        }
    }
}
