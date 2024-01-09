az group create --name az204-vault-rg --location westeurope

az keyvault create --name az204vault-abc --resource-group az204-vault-rg --location westeurope

az keyvault secret set --vault-name az204vault-abc --name "mySecret" --value "hVFkk965BuUv"

az keyvault secret purge --name "mySecret" --vault-name az204vault-abc
