/**
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
**/

/***
FOR RUN : 
dotnet build
dotnet run
*/

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

Console.WriteLine("Azure Blob Storage exercise\n");

// Run the examples asynchronously, wait for the results before proceeding
ProcessAsync().GetAwaiter().GetResult();

Console.WriteLine("Press enter to exit the sample application.");
Console.ReadLine();

static async Task ProcessAsync()
{
    // Copy the connection string from the portal in the variable below.
    string storageConnectionString = "CONNECTION STRING";

    // Create a client that can authenticate with a connection string
    BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);



    // CREATE CONTAINER
    //Create a unique name for the container
    string containerName = "wtblob" + Guid.NewGuid().ToString();

    // Create the container and return a container client object
    BlobContainerClient containerClient = await CreateContainer(blobServiceClient);


    //UPLOAD BLOB TO A CONTAİNER
    // Create a local file in the ./data/ directory for uploading and downloading
    // Get a reference to the blob
    BlobClient blobClient = await UploadBlobToAContainer(containerClient);



    //LIST THE BLOB CONTAINERS 
    // List blobs in the container
    await ListBlobContainers(containerClient);

    // DOWNLOAD BLOB 
    // Download the blob to a local file
    // Append the string "DOWNLOADED" before the .txt extension 
    await DownloadBlob(blobClient);

    // DELETE CONTAINER 
    // Delete the container and clean up local files created
    await DeleteContainer(containerClient);
}


static async Task<BlobContainerClient> CreateContainer(BlobServiceClient blobServiceClient)
{
    // CREATE CONTAINER
    //Create a unique name for the container
    string containerName = "wtblob" + Guid.NewGuid().ToString();

    // Create the container and return a container client object
    BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
    Console.WriteLine("A container named '" + containerName + "' has been created. " +
        "\nTake a minute and verify in the portal." +
        "\nNext a file will be created and uploaded to the container.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    return containerClient;
}

static async Task<BlobClient> UploadBlobToAContainer(BlobContainerClient containerClient)
{
    string localPath = "./data/";
    string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    string localFilePath = Path.Combine(localPath, fileName);

    // Write text to the file
    await File.WriteAllTextAsync(localFilePath, "Hello, World!");

    // Get a reference to the blob
    BlobClient blobClient = containerClient.GetBlobClient(fileName);

    Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

    // Open the file and upload its data
    using (FileStream uploadFileStream = File.OpenRead(localFilePath))
    {
        await blobClient.UploadAsync(uploadFileStream);
        uploadFileStream.Close();
    }

    Console.WriteLine("\nThe file was uploaded. We'll verify by listing" +
            " the blobs next.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    return blobClient;
}

static async Task ListBlobContainers(BlobContainerClient containerClient)
{
    Console.WriteLine("Listing blobs...");
    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        Console.WriteLine("\t" + blobItem.Name);
    }

    Console.WriteLine("\nYou can also verify by looking inside the " +
            "container in the portal." +
            "\nNext the blob will be downloaded with an altered file name.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

static async Task DownloadBlob(BlobClient blobClient)
{
    string localPath = "./data/";
    string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    string localFilePath = Path.Combine(localPath, fileName);
    string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

    Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

    // Download the blob's contents and save it to a file
    BlobDownloadInfo download = await blobClient.DownloadAsync();

    using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
    {
        await download.Content.CopyToAsync(downloadFileStream);
    }
    Console.WriteLine("\nLocate the local file in the data directory created earlier to verify it was downloaded.");
    Console.WriteLine("The next step is to delete the container and local files.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

static async Task DeleteContainer(BlobContainerClient containerClient)
{
    string localPath = "./data/";
    string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    string localFilePath = Path.Combine(localPath, fileName);
    string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");
   Console.WriteLine("\n\nDeleting blob container...");
    await containerClient.DeleteAsync();

    Console.WriteLine("Deleting the local source and downloaded files...");
    File.Delete(localFilePath);
    File.Delete(downloadFilePath);

    Console.WriteLine("Finished cleaning up.");
}
