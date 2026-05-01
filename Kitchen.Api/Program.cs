using Kitchen.Core.Repositories;
using Kitchen.Application.Services;
using Kitchen.Infrastructure.DAL.Repositories;
using Kitchen.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCore();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KitchenDbContext>();
    dbContext.Database.Migrate();

    var ingredients = dbContext.Ingredients.ToList();
    if (!ingredients.Any())
    {
        ingredients = new List<Ingredient>()
        {
            new Ingredient("Test", 1, StorageLocation.Unspecified),
            new Ingredient("Test - lodówka", 2, StorageLocation.Fridge),
            new Ingredient("Test - zamra¿arka", 5, StorageLocation.Freezer),
            new Ingredient("Test - spi¿arnia", 10, StorageLocation.Pantry)
        };
        dbContext.Ingredients.AddRange(ingredients);
        dbContext.SaveChanges();
    }
}

app.Run();