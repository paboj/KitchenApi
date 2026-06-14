using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Repositories;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Kitchen.Infrastructure.DAL.Repositories
{
    internal sealed class PostgresStockItemRepository : IStockItemRepository
    {
        private readonly KitchenDbContext _dbContext;

        public PostgresStockItemRepository(KitchenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<StockItem>> GetAll()
            => await _dbContext.StockItems
            .AsNoTracking()
            .ToListAsync();
        

        public async Task<StockItem?> GetById(Guid id)
            => await _dbContext.StockItems
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == new StockItemId(id));

        public async Task<IEnumerable<StockItem>> GetByName(string name)
           => await _dbContext.StockItems
           .AsNoTracking()
           .Where(x => x.Name == new ProductName(name))
           .ToListAsync();

        public async Task Add(StockItem stockItem)
        {
            _dbContext.StockItems.Add(stockItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(StockItem stockItem)
        {
            _dbContext.StockItems.Update(stockItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var stockItem = await GetById(id);

            if (stockItem != null)
            {
                _dbContext.StockItems.Remove(stockItem);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<StockItem>> GetAllWithDetails()
            => await _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .ToListAsync();

        public async Task<StockItem?> GetByIdWithDetails(Guid id)
            => await _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == new StockItemId(id));


        public async Task<IEnumerable<StockItem>> GetByNameWithDetails(string name)
            => await _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .Where(x => x.Name == new ProductName(name))
            .ToListAsync();
    }
}
