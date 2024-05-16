using Azure.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Azure;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Graphitie.Services;
using Graphitie;


var odataEndpoint = "odata";
var version = "v1";

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var appConfig = builder.Configuration.Get<AppConfig>();
var apiTitle = appConfig!.NameSpace.Replace(".", " ");

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
          typeof(Graphitie.Profiles.Microsoft.MicrosoftProfile)
      );

builder.Services.AddScoped<GraphitieService>();
builder.Services.AddScoped<MicrosoftService>();
//builder.Services.AddSingleton<KeyVaultService>();

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
