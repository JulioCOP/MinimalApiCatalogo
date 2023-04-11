using Microsoft.EntityFrameworkCore;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container //ConfigureServices (classe startup) 



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// obter a string de conexão

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => 
                                            options.UseMySql(connectionString, 
                                            ServerVersion.AutoDetect(connectionString)));

var app = builder.Build(); // Em caso de inclusão no container dever ser antes do comando build

//Definir os endpoints
app.MapGet("/", () => "Catalogo de Produtos - 2023");
                                                       //injeção de dependência do contexto do banco de dados
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
{                                // dados da categoria
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

// Retornar uma lista de categorias
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync());

// Retornar uma unica categoria

app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categorias.FindAsync(id)
    is Categoria categoria ? Results.Ok(categoria) : Results.NotFound();
});

app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db) =>
{
    if (categoria.CategoriaId != id)
    {
        return Results.BadRequest();
    }
    var categoriaDB = await db.Categorias.FindAsync(id);
    if (categoriaDB is null) return Results.NotFound();
    categoriaDB.Nome = categoria.Nome;
    categoriaDB.Descricao = categoria.Descricao;

    await db.SaveChangesAsync();
    return Results.Ok(categoriaDB);
});

app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
{
    var categoria = await db.Categorias.FindAsync(id);

    if (categoria is null)
    {
        return Results.NotFound("CATEGORIA NÃO ENCONTRADA!");
    }

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Configure the HTTP request pipeline. // Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

