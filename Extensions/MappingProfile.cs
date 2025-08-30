using AutoMapper;
using AvyyanBackend.DTOs.Auth;
using AvyyanBackend.DTOs.User;
using AvyyanBackend.DTOs.Role;
using AvyyanBackend.DTOs.Machine;
using AvyyanBackend.DTOs.WebSocket;
using AvyyanBackend.DTOs.FabricStructure;
using AvyyanBackend.DTOs.Location;
using AvyyanBackend.DTOs.YarnType;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Models;

namespace AvyyanBackend.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth Controller mappings
            CreateMap<User, AuthUserDto>();
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => "User"));
            CreateMap<PageAccess, AuthPageAccessDto>();

            // User Controller mappings
            CreateMap<User, UserProfileResponseDto>();
            CreateMap<User, AdminUserResponseDto>();
            CreateMap<CreateUserRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateUserRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore());
            CreateMap<UpdateUserProfileRequestDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.RoleName, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
            CreateMap<PageAccess, UserPageAccessDto>();

            // Role Controller mappings
            CreateMap<RoleMaster, RoleResponseDto>()
                .ForMember(dest => dest.PageAccesses, opt => opt.MapFrom(src => src.PageAccesses));
            CreateMap<CreateRoleRequestDto, RoleMaster>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateRoleRequestDto, RoleMaster>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PageAccesses, opt => opt.Ignore());
            CreateMap<PageAccess, PageAccessResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<PageAccess, RolePageAccessDto>();
            CreateMap<CreatePageAccessRequestDto, PageAccess>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
            CreateMap<UpdatePageAccessRequestDto, PageAccess>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // Machine Controller mappings
            CreateMap<MachineManager, MachineResponseDto>();
			CreateMap<User, AuthUserDto>();
			CreateMap<UserPageAccessDto, AuthPageAccessDto>();
			CreateMap<CreateMachineRequestDto, MachineManager>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateMachineRequestDto, MachineManager>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Fabric Structure mappings
            CreateMap<FabricStructureMaster, FabricStructureResponseDto>();
            CreateMap<CreateFabricStructureRequestDto, FabricStructureMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateFabricStructureRequestDto, FabricStructureMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Complex mappings for user permissions
            CreateMap<User, UserPermissionsResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PageAccesses, opt => opt.Ignore()); // Will be mapped separately

            // Location mappings
            CreateMap<LocationMaster, LocationResponseDto>();
            CreateMap<CreateLocationRequestDto, LocationMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateLocationRequestDto, LocationMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
                
            // Yarn Type mappings
            CreateMap<YarnTypeMaster, YarnTypeResponseDto>();
            CreateMap<CreateYarnTypeRequestDto, YarnTypeMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateYarnTypeRequestDto, YarnTypeMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Sales Order mappings
            CreateMap<SalesOrder, SalesOrderResponseDto>();
            CreateMap<SalesOrderItem, SalesOrderItemResponseDto>();
            CreateMap<CreateSalesOrderRequestDto, SalesOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProcessDate, opt => opt.Ignore());
            CreateMap<CreateSalesOrderItemRequestDto, SalesOrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SalesOrderId, opt => opt.Ignore());
            CreateMap<UpdateSalesOrderRequestDto, SalesOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<UpdateSalesOrderItemRequestDto, SalesOrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SalesOrderId, opt => opt.Ignore());
        }
    }
}