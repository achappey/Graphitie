using Graphitie.Models;

namespace Graphitie.Profiles.Duolingo;

public class DuolingoProfile : AutoMapper.Profile
{
	
	public DuolingoProfile()
	{
		
		CreateMap<Graphitie.Connectors.Duolingo.Models.Language, Language>();

		CreateMap<Tuple<Graphitie.Connectors.Duolingo.Models.Calendar, Graphitie.Connectors.Duolingo.Models.Skill>, Activity>()
			.ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.Item1.DateTime))) 
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Format("{0} {1}: {2}", src.Item2.LanguageName, src.Item1.EventType, src.Item2.Name)))
			.ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => ActivityType.LANGUAGE ));

	}
}