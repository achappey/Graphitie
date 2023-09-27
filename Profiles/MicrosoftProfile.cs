using Microsoft.Graph;

namespace Graphitie.Profiles.Microsoft;

public class MicrosoftProfile : AutoMapper.Profile
{

    public MicrosoftProfile()
    {
        CreateMap<User, Models.User>();
        
        CreateMap<User, Models.Employee>();        
        
        CreateMap<SignIn, Models.SignIn>();
        CreateMap<Group, Models.Group>();
        CreateMap<SignInLocation, Models.SignInLocation>();
        CreateMap<SignInStatus, Models.SignInStatus>();
        CreateMap<UserRegistrationDetails, Models.UserRegistrationDetails>();

        CreateMap<Models.CalendarEvent, Event>();
        CreateMap<Models.EventAttendee, Attendee>();
        CreateMap<Models.EmailAddress, EmailAddress>();
        CreateMap<Models.EventDateTime, DateTimeTimeZone>();
        CreateMap<Models.ItemBody, ItemBody>();
        CreateMap<Models.Mail, Message>();
        CreateMap<Models.Recipient, Recipient>();

        CreateMap<Message, Models.Mail>();
        CreateMap<Recipient, Models.Recipient>();
        CreateMap<Event, Models.CalendarEvent>();
        CreateMap<Attendee, Models.EventAttendee>();
        CreateMap<EmailAddress, Models.EmailAddress>();
        CreateMap<DateTimeTimeZone, Models.EventDateTime>();
        CreateMap<ItemBody, Models.ItemBody>();

        CreateMap<SubscribedSku, Models.License>()
        .ForMember(t => t.EnabledUnits, f => f.MapFrom(y => y.PrepaidUnits.Enabled));

        CreateMap<AssignedLicense,string>().ConvertUsing(y => y.SkuId.HasValue ? y.SkuId.ToString() : null);

        CreateMap<UserExperienceAnalyticsDevicePerformance, Graphitie.Models.DevicePerformance>();

        CreateMap<Device, Models.Device>()
        .ForMember(t => t.RegisteredOwner, f => f.MapFrom(g => g.RegisteredOwners.Select(z => z.Id).FirstOrDefault()));

        CreateMap<SecureScore, Models.SecureScore>()
          .ForMember(t => t.ComparativeScore, f => f.MapFrom(g => g.AverageComparativeScores.Select(z => z.AverageScore).Average()))
          .ForMember(t => t.Score, f => f.MapFrom(g => g.CurrentScore / g.MaxScore * 100));

        CreateMap<Alert, Models.SecurityAlert>()
            .ForMember(t => t.Users, f => f.MapFrom(g => g.UserStates.Select(z => z.UserPrincipalName)));


    }
}