using Kitchen.Api.Repositories;
using Kitchen.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IIngredientRepository, InMemoryIngredientRepository>();
builder.Services.AddSingleton<IIngredientTypeRepository, InMemoryIngredientTypeRepository>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddSingleton<ICatalogService, CatalogService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();