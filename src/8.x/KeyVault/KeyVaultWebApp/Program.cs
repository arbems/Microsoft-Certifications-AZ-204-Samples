using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

SecretClientOptions options = new()
{
    Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
};
var client = new SecretClient(
    new Uri("https://" + builder.Configuration.GetSection("keyVaultName").Value! + ".vault.azure.net/"),
    new DefaultAzureCredential(),
    options);

KeyVaultSecret secret = client.GetSecret("mySecret");

app.MapGet("/", () => secret.Value);

