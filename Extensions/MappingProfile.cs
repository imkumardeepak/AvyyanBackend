using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Models;

namespace AvyyanBackend.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // MachineManager mappings
            CreateMap<MachineManager, MachineManagerDto>();

            CreateMap<CreateMachineManagerDto, MachineManager>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateMachineManagerDto, MachineManager>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // Authentication mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.IsOnline))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Will be populated separately

            CreateMap<Role, RoleDto>();

            CreateMap<PageAccess, PageAccessDto>()
                .ForMember(dest => dest.CanView, opt => opt.Ignore())
                .ForMember(dest => dest.CanCreate, opt => opt.Ignore())
                .ForMember(dest => dest.CanEdit, opt => opt.Ignore())
                .ForMember(dest => dest.CanDelete, opt => opt.Ignore())
                .ForMember(dest => dest.CanExport, opt => opt.Ignore());




            // Chat Room mappings
            CreateMap<ChatRoom, ChatRoomDto>()
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Members.Count))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault()));

            CreateMap<CreateChatRoomDto, ChatRoom>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Chat Message mappings
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => $"{src.Sender.FirstName} {src.Sender.LastName}"))
                .ForMember(dest => dest.SenderProfileImage, opt => opt.MapFrom(src => src.Sender.ProfilePicture));

            CreateMap<SendMessageDto, ChatMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenderId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Chat Room Member mappings
            CreateMap<ChatRoomMember, ChatRoomMemberDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.User.IsOnline));

            // Message Reaction mappings
            CreateMap<MessageReaction, MessageReactionDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            // Notification mappings
            CreateMap<Notification, NotificationDto>();

            CreateMap<CreateNotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        }
    }
}
