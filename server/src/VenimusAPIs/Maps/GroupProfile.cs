using AutoMapper;

namespace VenimusAPIs.Maps
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<ViewModels.CreateNewGroup, Models.Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Models.Group, ViewModels.GetGroup>();
        }
    }
}
