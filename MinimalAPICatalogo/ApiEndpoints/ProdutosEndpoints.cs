using Microsoft.EntityFrameworkCore;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.ApiEndpoints
{
    public static class ProdutosEndpoints
    {
        public static void MapProdutosEndpoints(this WebApplication app)
        {
            //endpoints para produtos

            app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
            {
                db.Produtos.Add(produto);
                await db.SaveChangesAsync();

                return Results.Created($"/produtos/{produto.ProdutoId}", produto);
            });

            //retornar lista de produtos
            app.MapGet("/produtos", async (AppDbContext db) =>
                await db.Produtos.ToListAsync()).WithTags("Produtos").RequireAuthorization();

            // Retornar um unico produto

            app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
            {
                return await db.Produtos.FindAsync(id)
                is Produto produto ? Results.Ok(produto) : Results.NotFound();
            });

            //Atualizar ou alterar um produto
            app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) =>
            {
                if (produto.ProdutoId != id)
                {
                    return Results.BadRequest();
                }
                var produtoDB = await db.Produtos.FindAsync(id);
                if (produtoDB is null) return Results.NotFound();
                produtoDB.Nome = produto.Nome;
                produtoDB.Descricao = produto.Descricao;
                produtoDB.Preco = produto.Preco;
                produtoDB.Imagem = produto.Imagem;
                produtoDB.DataCompra = produto.DataCompra;
                produtoDB.Estoque = produto.Estoque;
                produtoDB.ProdutoId = produto.ProdutoId;

                await db.SaveChangesAsync();
                return Results.Ok(produtoDB);
            });

            // Deletar um produto
            app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
            {
                var produto = await db.Produtos.FindAsync(id);
                if (produto is null)
                {
                    return Results.NotFound("PRODUTO NÃO ENCONTRADO!");
                }

                db.Produtos.Remove(produto);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

        }
    }
}
