﻿using AutoMapper;

namespace VenimusAPIs.Maps
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<ViewModels.CreateNewEvent, Models.Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ViewModels.UpdateEvent, Models.Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
