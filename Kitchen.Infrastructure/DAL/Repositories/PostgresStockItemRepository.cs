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
        

        public StockItem? GetById(Guid id)
            => _dbContext.StockItems
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == new StockItemId(id));

        public IEnumerable<StockItem> GetByName(string name)
           => _dbContext.StockItems
           .AsNoTracking()
           .Where(x => x.Name == new ProductName(name))
           .ToList();

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

        public void Delete(Guid id)
        {
            var stockItem = GetById(id);

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

        public StockItem? GetByIdWithDetails(Guid id)
            => _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == new StockItemId(id));


        public IEnumerable<StockItem> GetByNameWithDetails(string name)
            => _dbContext.StockItems
            .Include(i => i.Type)
            .AsNoTracking()
            .Where(x => x.Name == new ProductName(name))
            .ToList();
    }
}
