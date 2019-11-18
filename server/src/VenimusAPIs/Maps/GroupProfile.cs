using System;
using AutoMapper;

namespace VenimusAPIs.Maps
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<ViewModels.CreateGroup, Models.Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => Convert.FromBase64String(src.LogoInBase64)));

            CreateMap<ViewModels.UpdateGroup, Models.Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => Convert.FromBase64String(src.LogoInBase64)));

            CreateMap<Models.Group, ViewModels.GetGroup>()
                .ForMember(dest => dest.LogoInBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.Logo)));

            CreateMap<Models.Group, ViewModels.ViewMyGroupMembership>()
                .ForMember(dest => dest.GroupDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.GroupLogoInBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.Logo)))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.GroupSlackChannelName, opt => opt.MapFrom(src => src.SlackChannelName))
                .ForMember(dest => dest.GroupSlug, opt => opt.MapFrom(src => src.Slug));
        }
    }
}
