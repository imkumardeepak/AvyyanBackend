using AvyyanBackend.Models;

namespace AvyyanBackend.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Add repository properties here as you create specific repositories
        // Example: IProductRepository Products { get; }
        // Example: ICategoryRepository Categories { get; }
        // Example: ICustomerRepository Customers { get; }
        // Example: IOrderRepository Orders { get; }
        IRepository<TapeColorMaster> TapeColors { get; }
        IRepository<ShiftMaster> Shifts { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}