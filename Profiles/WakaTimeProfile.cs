using Graphitie.Models;

namespace Graphitie.Profiles.WakaTime;

public class WakaTimeProfile : AutoMapper.Profile
{

    public WakaTimeProfile()
    {

        CreateMap<Graphitie.Connectors.WakaTime.Models.HeartBeat, Activity>()
            .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds((long)src.Time)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Format("{0} {1}", src.Category, src.Language)))
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => ActivityType.CODE));

        CreateMap<Graphitie.Connectors.WakaTime.Models.Duration, Activity>()
            .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds((long)src.End)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => ActivityType.CODE));

    }
}