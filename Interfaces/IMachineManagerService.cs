using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IMachineManagerService
    {
        // Basic CRUD Operations
        Task<IEnumerable<MachineManagerDto>> GetAllMachinesAsync();
        Task<MachineManagerDto?> GetMachineByIdAsync(int id);
        Task<MachineManagerDto> CreateMachineAsync(CreateMachineManagerDto createMachineDto);
        Task<MachineManagerDto?> UpdateMachineAsync(int id, UpdateMachineManagerDto updateMachineDto);
        Task<bool> DeleteMachineAsync(int id);

        // Search Operations
        Task<IEnumerable<MachineManagerDto>> SearchMachinesAsync(string? machineName, decimal? dia);

        // Bulk Operations
        Task<IEnumerable<MachineManagerDto>> CreateMultipleMachinesAsync(IEnumerable<CreateMachineManagerDto> createMachineDtos);

        // Validation
        Task<bool> IsMachineNameUniqueAsync(string machineName, int? excludeId = null);
    }
}
