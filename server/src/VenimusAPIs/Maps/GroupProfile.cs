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

            CreateMap<Models.Group, ViewModels.ListGroups>()
                .ForMember(dest => dest.LogoInBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.Logo)));

            CreateMap<Models.Group, ViewModels.ListActiveGroups>()
                .ForMember(dest => dest.LogoInBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.Logo)));

            CreateMap<Models.Group, ViewModels.ListMyGroups>()
                .ForMember(dest => dest.LogoInBase64, opt => opt.MapFrom(src => Convert.ToBase64String(src.Logo)));
        }
    }
}
