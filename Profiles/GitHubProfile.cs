using Graphitie.Models;

namespace Graphitie.Profiles.GitHub;

public class GitHubProfile : AutoMapper.Profile
{

    public GitHubProfile()
    {

        CreateMap<Octokit.Repository, Repository>();
        
        CreateMap<Octokit.Activity, Activity>()
            .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => ActivityType.DEVOPS ));

    }
}