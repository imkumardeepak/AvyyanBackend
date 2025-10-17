using AutoMapper;
using AvyyanBackend.Models;
using AvyyanBackend.DTOs.DispatchPlanning;

namespace AvyyanBackend.DTOs.DispatchPlanning
{
    public class DispatchPlanningProfile : Profile
    {
        public DispatchPlanningProfile()
        {
            CreateMap<Models.DispatchPlanning, DispatchPlanningDto>();
            CreateMap<DispatchPlanningDto, Models.DispatchPlanning>();
            CreateMap<CreateDispatchPlanningDto, Models.DispatchPlanning>();
            CreateMap<UpdateDispatchPlanningDto, Models.DispatchPlanning>();
            
            CreateMap<Models.DispatchedRoll, DispatchedRollDto>();
            CreateMap<DispatchedRollDto, Models.DispatchedRoll>();
        }
    }
}