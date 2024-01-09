using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

string keyVaultName = builder.Configuration.GetSection("keyVaultName").Value!;
string userAssignedClientId = "eb5a2de6-dccd-4d43-b58e-dec19003f04a"; // "<your managed identity client Id>";

var client = new SecretClient(
    new Uri("https://" + keyVaultName + ".vault.azure.net/"),
    new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId }),
    new SecretClientOptions()
    {
        Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
    });

KeyVaultSecret secret = client.GetSecret("mySecret");

app.MapGet("/", () => secret.Value);
