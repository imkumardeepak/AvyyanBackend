using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Models;

namespace AvyyanBackend.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User and Auth mappings
            CreateMap<User, UserDto>();

            CreateMap<RegisterDto, User>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();

            // Role and PageAccess mappings
            CreateMap<RoleMaster, RoleDto>().ReverseMap();
            CreateMap<CreateRoleDto, RoleMaster>();
            CreateMap<UpdateRoleDto, RoleMaster>();

            CreateMap<PageAccess, PageAccessDto>().ReverseMap();
            CreateMap<CreatePageAccessDto, PageAccess>();
            CreateMap<UpdatePageAccessDto, PageAccess>();

            // MachineManager mappings
            CreateMap<MachineManager, MachineManagerDto>().ReverseMap();
            CreateMap<CreateMachineManagerDto, MachineManager>();
            CreateMap<UpdateMachineManagerDto, MachineManager>();
        }
    }
}