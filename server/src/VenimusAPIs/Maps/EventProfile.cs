using AutoMapper;

namespace VenimusAPIs.Maps
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<ViewModels.CreateNewEvent, Models.Event>()
                .ForMember(dest => dest._id, opt => opt.Ignore());
        }
    }
}
