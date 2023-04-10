var builder = WebApplication.CreateBuilder(args);

// Add services to the container //ConfigureServices (classe startup) 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); // Em caso de inclusão no container dever ser antes do comando build

// Configure the HTTP request pipeline. // Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

