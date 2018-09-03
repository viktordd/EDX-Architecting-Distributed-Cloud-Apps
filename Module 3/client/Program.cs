using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace service
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials("edxcloudapps", "MfMgLvfsHdM3QzSB7vwyn2nOwJRXwzD4+dkzeqEFDR01eV5dgMY+5sD7qMkyHv9SYiHoZMumDBGEw2EOHYZo0A=="), true);

            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("jobqueue");

            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var message = new CloudQueueMessage(GetMessage(args));

            queue.AddMessageAsync(message).GetAwaiter().GetResult();

        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}