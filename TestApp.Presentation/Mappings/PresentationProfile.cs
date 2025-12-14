using AutoMapper;
using TestApp.Application.Models;
using TestApp.Presentation.Models;

namespace TestApp.Presentation.Mappings;

public class PresentationProfile : Profile
{
    public PresentationProfile()
    {
        CreateMap<SignalModel, Signal>().ReverseMap();
    }
}