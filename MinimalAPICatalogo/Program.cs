using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;
using MinimalAPICatalogo.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container //ConfigureServices (classe startup) 



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// obter a string de conexão

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => 
                                            options.UseMySql(connectionString, 
                                            ServerVersion.AutoDetect(connectionString)));

// registro do serviço de geração do token
builder.Services.AddSingleton<ITokenService>(new TokenService());

//validação do token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthentication();


var app = builder.Build(); // Em caso de inclusão no container dever ser antes do comando build


// endpoint para login

app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
{
    if (userModel == null)
    {
        return Results.BadRequest("Login INVÁLIDO!");
    }
    if (userModel.UserName == "macoratti" && userModel.Password == "numsey#123")
    {
        var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
           app.Configuration["Jwt:Issuer"],
           app.Configuration["Jwt:Audience"],
           userModel);
        return Results.Ok(new { token = tokenString });
    }
    else
    {
        return Results.BadRequest("Login INVÁLIDO!");
    }
}).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Autenticacao");



//Definir os endpoints categoria

app.MapGet("/", () => "Catalogo de Produtos - 2023").ExcludeFromDescription();
                                                       //injeção de dependência do contexto do banco de dados
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
{                                // dados da categoria
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

// Retornar uma lista de categorias
app.MapGet("/categorias", async (AppDbContext db) => 
    await db.Categorias.ToListAsync()).RequireAuthorization();

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

//endpoints para produtos

app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();

    return Results.Created($"/produtos/{produto.ProdutoId}", produto);
});

//retornar lista de produtos
app.MapGet("/produtos", async (AppDbContext db) => 
    await db.Produtos.ToListAsync()).RequireAuthorization();

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

// Configure the HTTP request pipeline. // Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//ativar os serviços de autorização e autenticação
app.UseAuthentication();
app.UseAuthorization();

app.Run();

