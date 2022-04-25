using Microsoft.Graph;

namespace Graphitie.Profiles.Microsoft;

public class MicrosoftProfile : AutoMapper.Profile
{

    public MicrosoftProfile()
    {
        CreateMap<User, Graphitie.Models.User>();
        CreateMap<User, Graphitie.Models.Employee>();
        CreateMap<UserRegistrationDetails, Graphitie.Models.UserRegistrationDetails>();
        
        CreateMap<SignIn, Graphitie.Models.SignIn>();
        CreateMap<SignInLocation, Graphitie.Models.SignInLocation>();

        CreateMap<Device, Graphitie.Models.Device>()
        .ForMember(t => t.RegisteredOwner, f => f.MapFrom(g => g.RegisteredOwners.Select(z => z.Id).FirstOrDefault()));

        CreateMap<SecureScore, Graphitie.Models.SecureScore>()
          .ForMember(t => t.ComparativeScore, f => f.MapFrom(g => g.AverageComparativeScores.Select(z => z.AverageScore).Average()))
          .ForMember(t => t.Score, f => f.MapFrom(g => (g.CurrentScore / g.MaxScore) * 100));

        CreateMap<Alert, Graphitie.Models.SecurityAlert>()
            .ForMember(t => t.Users, f => f.MapFrom(g => g.UserStates.Select(z => z.UserPrincipalName)));


    }
}