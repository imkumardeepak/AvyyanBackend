using Microsoft.EntityFrameworkCore.Storage;
using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;

namespace AvyyanBackend.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add repository properties here as you create specific repositories
        // Example:
        // private IProductRepository? _products;
        // public IProductRepository Products => _products ??= new ProductRepository(_context);

        // private ICategoryRepository? _categories;
        // public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);

        // private ICustomerRepository? _customers;
        // public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);

        // private IOrderRepository? _orders;
        // public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
