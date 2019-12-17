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
        }
    }
}
