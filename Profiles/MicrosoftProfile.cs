using Microsoft.Graph;

namespace Graphitie.Profiles.Microsoft;

public class MicrosoftProfile : AutoMapper.Profile
{

    public MicrosoftProfile()
    {
        CreateMap<User, Graphitie.Models.User>();
        
        CreateMap<User, Graphitie.Models.Employee>();        
        
        CreateMap<SignIn, Graphitie.Models.SignIn>();
        CreateMap<Group, Graphitie.Models.Group>();
        CreateMap<SignInLocation, Graphitie.Models.SignInLocation>();
        CreateMap<SignInStatus, Graphitie.Models.SignInStatus>();
        CreateMap<UserRegistrationDetails, Graphitie.Models.UserRegistrationDetails>();

        CreateMap<SubscribedSku, Graphitie.Models.License>()
        .ForMember(t => t.EnabledUnits, f => f.MapFrom(y => y.PrepaidUnits.Enabled));

        CreateMap<AssignedLicense,string>().ConvertUsing(y => y.SkuId.HasValue ? y.SkuId.ToString() : null);

        CreateMap<UserExperienceAnalyticsDevicePerformance, Graphitie.Models.DevicePerformance>();

        CreateMap<Device, Graphitie.Models.Device>()
        .ForMember(t => t.RegisteredOwner, f => f.MapFrom(g => g.RegisteredOwners.Select(z => z.Id).FirstOrDefault()));

        CreateMap<SecureScore, Graphitie.Models.SecureScore>()
          .ForMember(t => t.ComparativeScore, f => f.MapFrom(g => g.AverageComparativeScores.Select(z => z.AverageScore).Average()))
          .ForMember(t => t.Score, f => f.MapFrom(g => (g.CurrentScore / g.MaxScore) * 100));

        CreateMap<Alert, Graphitie.Models.SecurityAlert>()
            .ForMember(t => t.Users, f => f.MapFrom(g => g.UserStates.Select(z => z.UserPrincipalName)));


    }
}