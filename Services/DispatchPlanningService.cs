using AvyyanBackend.DTOs.DispatchPlanning;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using AutoMapper;

namespace AvyyanBackend.Services
{
    public class DispatchPlanningService
    {
        private readonly IDispatchPlanningRepository _repository;
        private readonly IMapper _mapper;

        public DispatchPlanningService(IDispatchPlanningRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DispatchPlanningDto>> GetAllAsync()
        {
            var dispatchPlannings = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DispatchPlanningDto>>(dispatchPlannings);
        }

        public async Task<DispatchPlanningDto?> GetByIdAsync(int id)
        {
            var dispatchPlanning = await _repository.GetByIdAsync(id);
            return dispatchPlanning == null ? null : _mapper.Map<DispatchPlanningDto>(dispatchPlanning);
        }

        public async Task<DispatchPlanningDto> CreateAsync(CreateDispatchPlanningDto createDto)
        {
            // Generate LoadingNo
            var loadingNo = await _repository.GenerateLoadingNoAsync();
            
            var dispatchPlanning = _mapper.Map<DispatchPlanning>(createDto);
            dispatchPlanning.LoadingNo = loadingNo;
            dispatchPlanning.CreatedAt = DateTime.UtcNow;
            dispatchPlanning.IsActive = true;
            
            var created = await _repository.CreateAsync(dispatchPlanning);
            return _mapper.Map<DispatchPlanningDto>(created);
        }

        public async Task<DispatchPlanningDto> UpdateAsync(int id, UpdateDispatchPlanningDto updateDto)
        {
            var dispatchPlanning = _mapper.Map<DispatchPlanning>(updateDto);
            var updated = await _repository.UpdateAsync(id, dispatchPlanning);
            return _mapper.Map<DispatchPlanningDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DispatchedRollDto>> GetDispatchedRollsByPlanningIdAsync(int planningId)
        {
            var dispatchedRolls = await _repository.GetDispatchedRollsByPlanningIdAsync(planningId);
            return _mapper.Map<IEnumerable<DispatchedRollDto>>(dispatchedRolls);
        }

        public async Task<DispatchedRollDto> CreateDispatchedRollAsync(DispatchedRollDto dto)
        {
            var dispatchedRoll = _mapper.Map<DispatchedRoll>(dto);
            dispatchedRoll.CreatedAt = DateTime.UtcNow;
            dispatchedRoll.IsActive = true;
            
            var created = await _repository.CreateDispatchedRollAsync(dispatchedRoll);
            return _mapper.Map<DispatchedRollDto>(created);
        }
    }
}