using System.Xml.Linq;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Application.Models.Requests;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class InventoryService : IInventoryService
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

        _repository.Update(ingredient);
    }

    public void Delete(string name)
    {
        var ingredient = FindIngredient(name);

        _repository.Delete(name);
    }
}