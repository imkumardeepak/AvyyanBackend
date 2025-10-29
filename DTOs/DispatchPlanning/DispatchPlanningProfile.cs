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
            
            // Add missing mapping for CreateDispatchPlanningRequestDto
            CreateMap<CreateDispatchPlanningRequestDto, Models.DispatchPlanning>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LoadingNo, opt => opt.Ignore())
                .ForMember(dest => dest.DispatchOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
            
            CreateMap<Models.DispatchedRoll, DispatchedRollDto>();
            CreateMap<DispatchedRollDto, Models.DispatchedRoll>();
        }
    }
}