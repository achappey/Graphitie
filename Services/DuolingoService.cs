using AutoMapper;
using Graphitie.Models;
using Graphitie.Connectors.Duolingo;

namespace Graphitie.Services;

public interface IDuolingoService
{
    public Task<IEnumerable<Language>> GetLanguages();
    public Task<IEnumerable<Activity>> GetActivities();

}

public class DuolingoService : IDuolingoService
{
    private readonly Duolingo _duolingo;
    private readonly IMapper _mapper;
    private readonly IKeyVaultService _keyVaultService;
    public DuolingoService(
        KeyVaultService keyVaultService,
        IMapper mapper,
        Duolingo duolingo)
    {
        _duolingo = duolingo;
        _mapper = mapper;
        _keyVaultService = keyVaultService;
    }

    public async Task<IEnumerable<Language>> GetLanguages()
    {
        var duolingoItems = await this.GetDuolingoUsers();

        List<Language> result = new List<Language>();

        foreach (var duolingo in duolingoItems)
        {
            if (duolingo != null)
            {
                result.AddRange(
                    duolingo.Languages
                    .Where(t => t.Points > 0)
                    .Select(t => this._mapper.Map<Language>(t)));
            }
        }

        return result.GroupBy(y => y.Name, y => new { y.Code, y.Points, y.Level },
        (v, a) => new Language()
        {
            Name = v,
            Code = a.Select(z => z.Code).First(),
            Points = a.Select(y => y.Points).Sum(),
            Level = (int)Math.Round(a.Select(y => y.Level).Average())
        })
        .OrderByDescending(b => b.Points);
    }


    public async Task<IEnumerable<Activity>> GetActivities()
    {
        var duolingoItems = await this.GetDuolingoUsers();

        List<Activity> result = new List<Activity>();

        foreach (var duolingo in duolingoItems)
        {

            if (duolingo != null && duolingo.LanguageData.Count() > 0)
            {
                var languageData = duolingo.LanguageData.First().Value;

                var activities = languageData.Calendar
                .Select(t => Tuple.Create(t, languageData.Skills.FirstOrDefault(y => y.Id == t.Skill)));

                result.AddRange(
                    activities
                    .Select(t => this._mapper.Map<Activity>(t)));
            }
        }

        return result.OrderByDescending(y => y.DateTime);
    }

    private async Task<IEnumerable<Graphitie.Connectors.Duolingo.Models.User>> GetDuolingoUsers()
    {
        var duolingoItems = this._keyVaultService.GetSecretProperties("duolingo");

        List<Graphitie.Connectors.Duolingo.Models.User> result = new List<Graphitie.Connectors.Duolingo.Models.User>();

        foreach (var duolingo in duolingoItems)
        {
            var secret = await this._keyVaultService.GetSecret(duolingo.Name);

            var duolingoUser = await this._duolingo.GetUser(duolingo.ContentType, secret.Value);

            if (duolingoUser != null)
            {
                result.Add(duolingoUser);
            }
        }

        return result;

    }

}