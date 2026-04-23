using System.Xml.Linq;
using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Domain.Exceptions;
using Kitchen.Api.Models.DTOs;
using Kitchen.Api.Repositories;
using Kitchen.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly IIngredientRepository _repository;
    private Ingredient FindIngredient(string name)
    {
        var ingredient = GetByName(name);
        if (ingredient == null) throw new IngredientNotFoundException();

        return ingredient;

    }

    public InventoryService(IIngredientRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Ingredient> GetAll() => _repository.GetAll();

    public Ingredient? GetByName(string name) => _repository.GetByName(name);

    public void Add(AddToStockCommand command)
    {
        var ingredient = new Ingredient(
            command.Name,
            command.Amount,
            command.Location
        );
        _repository.Add(ingredient);
    }

    public void Update(ModifyInStockCommand command)
    {
        var ingredient = FindIngredient(command.Name);

        ingredient.AdjustAmount(command.Amount);
        ingredient.PlaceOrMove(command.Location);
    }

    public void Delete(string name)
    {
        var ingredient = FindIngredient(name);

        _repository.Delete(name);
    }
}