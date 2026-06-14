using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;

namespace Kitchen.Application.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<ProductDefinition>> GetAll();
        Task<ProductDefinition?> GetByName(string name);
        Task Add(AddProductDefinitionCommand command);
        Task Update(ModifyProductDefinitionCommand command);
        Task Delete(string name);
    }
}
