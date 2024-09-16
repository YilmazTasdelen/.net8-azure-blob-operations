az login
az group create --location <myLocation> --name az204-blob-rg
az storage account create --resource-group az204-blob-rg --name <myStorageAcct> --location <myLocation> --sku Standard_LRS

Get credentials for the storage account.
** Navigate to the Azure portal.
** Locate the storage account created.
** Select Access keys in the Security + networking section of the navigation pane. Here, you can view your account access keys and the complete connection string for each key.
** Find the Connection string value under key1, and select the Copy button to copy the connection string. You'll add the connection string value to the code in the next section.
** In the Blob section of the storage account overview, select Containers. Leave the windows open so you can view changes made to the storage as you progress through the exercise.

dotnet new console -n az204-blob
cd az204-blob
dotnet build
mkdir data
dotnet add package Azure.Storage.Blobs



FOR RUN : 
dotnet build
dotnet run
