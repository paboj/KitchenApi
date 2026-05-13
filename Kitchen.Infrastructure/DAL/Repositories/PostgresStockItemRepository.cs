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

        public IEnumerable<StockItem> GetAll()
            => _dbContext.StockItems
            .AsNoTracking()
            .ToList();
        

        public StockItem? GetByName(string name)
            => _dbContext.StockItems
            .AsNoTracking()
            .FirstOrDefault(x => x.Name == new ProductName(name));

        public void Add(StockItem stockItem)
        {
            _dbContext.StockItems.Add(stockItem);
            _dbContext.SaveChanges();
        }

        public void Update(StockItem stockItem)
        {
            _dbContext.StockItems.Update(stockItem);
            _dbContext.SaveChanges();
        }

        public void Delete(string name)
        {
            var stockItem = GetByName(name);

            if (stockItem != null)
            {
                _dbContext.StockItems.Remove(stockItem);
                _dbContext.SaveChanges();
            }
        }
        public IEnumerable<StockItem> GetAllWithDetails()
            => _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .ToList();


        public StockItem? GetByNameWithDetails(string name)
            => _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .FirstOrDefault(x => x.Name == new ProductName(name));
    }
}
