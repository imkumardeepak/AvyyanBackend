using AvyyanBackend.DTOs.SalesOrder;

namespace AvyyanBackend.Interfaces
{
    public interface ISalesOrderWebService
    {
        Task<IEnumerable<SalesOrderWebResponseDto>> GetAllAsync();
        Task<SalesOrderWebResponseDto?> GetByIdAsync(int id);
        Task<SalesOrderWebResponseDto> CreateAsync(CreateSalesOrderWebRequestDto createSalesOrderWebDto);
        Task<SalesOrderWebResponseDto?> UpdateAsync(int id, UpdateSalesOrderWebRequestDto updateSalesOrderWebDto);
        Task<bool> DeleteAsync(int id);
    }
}