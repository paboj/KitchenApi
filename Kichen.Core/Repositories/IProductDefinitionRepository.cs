using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IProductDefinitionRepository
    {
        IEnumerable<ProductDefinition> GetAll();
        ProductDefinition? GetByName(string name);
        void Add(ProductDefinition productDefinition);
        void Delete(string name);
        void Update(ProductDefinition productDefinition);
    }
}
