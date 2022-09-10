using Azure.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Azure;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Graphitie.Services;
using Graphitie.Connectors.WakaTime;
using Graphitie;

using Octokit;

var odataEndpoint = "odata";
var version = "v1";

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var appConfig = builder.Configuration.Get<AppConfig>();
var apiTitle = appConfig.NameSpace.Replace(".", " ");

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(version, new OpenApiInfo { Title = apiTitle, Version = version });

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.RelativePath != null ? docName == odataEndpoint ?
            apiDesc.RelativePath.Contains(odataEndpoint) : !apiDesc.RelativePath.Contains(odataEndpoint) : false;
    });
});

builder.Services.AddAutoMapper(
          typeof(Graphitie.Profiles.Microsoft.MicrosoftProfile),
          typeof(Graphitie.Profiles.WakaTime.WakaTimeProfile),
          typeof(Graphitie.Profiles.GitHub.GitHubProfile)
      );

builder.Services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();

builder.Services.AddScoped<GraphitieService>();
builder.Services.AddScoped<MicrosoftService>();
builder.Services.AddSingleton<KeyVaultService>();
builder.Services.AddSingleton<WakaTime>();
builder.Services.AddSingleton<GitHubClient>(new GitHubClient(new ProductHeaderValue(appConfig.NameSpace)));

builder.Services.AddHttpClient();

builder.Services.AddAzureClients(b =>
 {
     b.AddSecretClient(new Uri(appConfig.KeyVault));

     b.UseCredential(new ClientSecretCredential(appConfig.AzureAd.TenantId,
     appConfig.AzureAd.ClientId,
     appConfig.AzureAd.ClientSecret));
 });


builder.Services.AddControllers()
    .AddOData(opt => opt.AddRouteComponents(odataEndpoint, GetGraphModel(appConfig.NameSpace))
            .Filter().Select().Expand().OrderBy().Count().SetMaxTop(999).SkipToken());

var microsoft = builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
          .AddMicrosoftIdentityWebApp(builder.Configuration)
          .EnableTokenAcquisitionToCallDownstreamApi()
          .AddMicrosoftGraphAppOnly(authenticationProvider => new GraphServiceClient(new ClientSecretCredential(appConfig.AzureAd.TenantId,
                appConfig.AzureAd.ClientId,
                appConfig.AzureAd.ClientSecret)))
          .AddInMemoryTokenCaches();

var app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        string.Format("/swagger/{0}/swagger.json", version),
        string.Format("{0} {1}", apiTitle, version));
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static IEdmModel GetGraphModel(string name)
{
    ODataConventionModelBuilder builder = new();

    builder.EntitySet<Graphitie.Models.User>("Users").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.Device>("Devices").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.Group>("Groups").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.License>("Licenses").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.SecurityAlert>("SecurityAlerts").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.SecureScore>("SecureScores").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.SignIn>("SignIns").EntityType.Namespace = name;
    builder.EntitySet<Graphitie.Models.UserRegistrationDetails>("UserRegistrationDetails").EntityType.Namespace = name;

    builder.Namespace = name;

    return builder.GetEdmModel();
}

public class AddRolesClaimsTransformation : IClaimsTransformation
{
    private readonly ILogger<AddRolesClaimsTransformation> _logger;

    public AddRolesClaimsTransformation(ILogger<AddRolesClaimsTransformation> logger)
    {
        _logger = logger;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var mappedRolesClaims = principal.Claims
            .Where(claim => claim.Type == "roles")
            .Select(claim => new Claim(ClaimTypes.Role, claim.Value))
            .ToList();

        // Clone current identity
        var clone = principal.Clone();

        if (clone.Identity is not ClaimsIdentity newIdentity) return Task.FromResult(principal);

        // Add role claims to cloned identity
        foreach (var mappedRoleClaim in mappedRolesClaims)
            newIdentity.AddClaim(mappedRoleClaim);

        if (mappedRolesClaims.Count > 0)
            _logger.LogInformation("Added roles claims {mappedRolesClaims}", mappedRolesClaims);
        else
            _logger.LogInformation("No roles claims added");

        return Task.FromResult(clone);
    }
}