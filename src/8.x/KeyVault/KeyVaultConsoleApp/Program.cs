using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Configuration;

string secretName = "mySecret";
string secretValue = "abc123456";

// <authenticate>
string keyVaultName = ConfigurationManager.AppSettings["keyVaultName"]!;
var kvUri = "https://" + keyVaultName + ".vault.azure.net";

var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
// </authenticate>

// <setsecret>
await client.SetSecretAsync(secretName, secretValue);
// </setsecret>

// <getsecret>
KeyVaultSecret secret = await client.GetSecretAsync(secretName);
// </getsecret>

Console.WriteLine("Your secret is '" + secret.Value + "'.");

// <deletesecret>
var operation = await client.StartDeleteSecretAsync(secretName);
await operation.WaitForCompletionAsync();
// </deletesecret>

// <purgesecret>
// You only need to wait for completion if you want to purge or recover the key.
await client.PurgeDeletedSecretAsync(secretName);
// </purgesecret>

Console.WriteLine(" done.");