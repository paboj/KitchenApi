using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IProductDefinitionRepository
    {
        Task<IEnumerable<ProductDefinition>> GetAll();
        Task<ProductDefinition?> GetByName(string name);
        Task Add(ProductDefinition productDefinition);
        Task Delete(string name);
        Task Update(ProductDefinition productDefinition);
    }
}
