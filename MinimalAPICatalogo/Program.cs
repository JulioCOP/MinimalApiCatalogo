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


// Configure the HTTP request pipeline. // Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

