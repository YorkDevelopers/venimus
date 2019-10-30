using AutoMapper;

namespace VenimusAPIs.Maps
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<ViewModels.CreateGroup, Models.Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ViewModels.UpdateGroup, Models.Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Models.Group, ViewModels.GetGroup>();

            CreateMap<Models.Group, ViewModels.ListGroups>();
        }
    }
}
