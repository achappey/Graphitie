using Azure.Security.KeyVault.Secrets;

namespace Graphitie.Services;

public interface IKeyVaultService
{
    public IEnumerable<SecretProperties> GetSecretProperties(string name);
    public Task<KeyVaultSecret> GetSecret(string name);

}

public class KeyVaultService : IKeyVaultService
{
    private readonly SecretClient _secretClient;
    
    public KeyVaultService(
        SecretClient secretClient)
    {
        _secretClient = secretClient;
    }

    public async Task<KeyVaultSecret> GetSecret(string name)
    {
        return await this._secretClient.GetSecretAsync(name);
    }

    public IEnumerable<SecretProperties> GetSecretProperties(string name)
    {
        var items = this._secretClient.GetPropertiesOfSecrets();

        return items.Where(r => r.Name.StartsWith(name));
    }

}