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
            // stockItem.Definition (if any) was loaded with AsNoTracking, so the
            // context doesn't know it already exists. Without this, DbSet.Add
            // marks the whole reachable graph as Added and EF tries to re-insert
            // an already-existing ProductDefinition, causing a PK violation.
            if (stockItem.Definition != null && _dbContext.Entry(stockItem.Definition).State == EntityState.Detached)
            {
                _dbContext.Attach(stockItem.Definition);
            }

            _dbContext.StockItems.Add(stockItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(StockItem stockItem)
        {
            // Same reasoning as Add: stockItem.Definition comes from an
            // AsNoTracking query, so without this, DbSet.Update marks it as
            // Modified too and issues a needless UPDATE against ProductDefinitions
            // on every StockItem edit.
            if (stockItem.Definition != null && _dbContext.Entry(stockItem.Definition).State == EntityState.Detached)
            {
                _dbContext.Attach(stockItem.Definition);
            }

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
            .Include(i => i.Definition)
            .AsNoTracking()
            .ToListAsync();

        public async Task<StockItem?> GetByIdWithDetails(Guid id)
            => await _dbContext.StockItems
            .Include(i => i.Definition)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == new StockItemId(id));


        public async Task<IEnumerable<StockItem>> GetByNameWithDetails(string name)
            => await _dbContext.StockItems
            .Include(i => i.Definition)
            .AsNoTracking()
            .Where(x => x.Name == new ProductName(name))
            .ToListAsync();
    }
}
