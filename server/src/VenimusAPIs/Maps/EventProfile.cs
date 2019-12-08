using AutoMapper;

namespace VenimusAPIs.Maps
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<ViewModels.CreateEvent, Models.GroupEvent>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ViewModels.UpdateEvent, Models.GroupEvent>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
