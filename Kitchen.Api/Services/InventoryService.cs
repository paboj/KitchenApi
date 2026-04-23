using System.Xml.Linq;
using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Models.DTOs;
using Kitchen.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly List<Ingredient> _ingredients = new();

    public IEnumerable<Ingredient> GetAll() => _ingredients.AsReadOnly();

    //TODO: EF? operatory wewnątrz zapytań LINQ-to-Entities mogą nie działać? (zazwyczaj mapuje się Value Object do kolumny w bazie).
    public Ingredient? GetByName(string name) => _ingredients.FirstOrDefault(i => i.Name == name);

    public void Add(AddToStockCommand command)
    {
        var ingredient = new Ingredient(
            command.Name,
            command.Amount,
            command.Location
        );
        _ingredients.Add(ingredient);
    }

    public bool Update(ModifyInStockCommand command)
    {
        var existing = GetByName(command.Name);
        if (existing == null) return false;

        existing.AdjustAmount(command.Amount);
        existing.PlaceOrMove(command.Location);
        return true;
    }

    public bool Delete(string name)
    {
        var ingredient = GetByName(name);
        return ingredient != null && _ingredients.Remove(ingredient);
    }
}