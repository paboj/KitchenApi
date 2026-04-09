using Kitchen.Api.Domain.Entities;

namespace Kitchen.Api.Services
{
    public interface IInventoryService
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient? GetByName(string name);
        void Add(Ingredient ingredient);
        bool Update(Ingredient ingredient);
        bool Delete(string name);
    }
}