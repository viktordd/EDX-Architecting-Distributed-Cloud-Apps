using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials("edxcloudapps", "MfMgLvfsHdM3QzSB7vwyn2nOwJRXwzD4+dkzeqEFDR01eV5dgMY+5sD7qMkyHv9SYiHoZMumDBGEw2EOHYZo0A=="), true);

            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("jobqueue");

            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var quit = false;

            Console.WriteLine("Enter q and press enter to quit, just press enter to process next message.");
            while (!quit)
            {
                var retrievedMessage = queue.GetMessageAsync(TimeSpan.FromSeconds(2), null, null).GetAwaiter().GetResult();

                if (retrievedMessage != null)
                {
                    var messageText = retrievedMessage.AsString;
                    Console.WriteLine($"Received: {messageText}, dequeue count is {retrievedMessage.DequeueCount}");
                    if (retrievedMessage.DequeueCount > 2)
                    {
                        Console.WriteLine("Mark the message as poison message, either move to a dead letter queue or log.");
                        queue.DeleteMessageAsync(retrievedMessage).GetAwaiter().GetResult();
                    }
                    else
                    {
                        if (messageText.Split('.').Length % 2 == 1)
                        {
                            Console.WriteLine("Processed with success.");
                            queue.DeleteMessageAsync(retrievedMessage).GetAwaiter().GetResult();
                        }
                        else
                        {
                            Console.WriteLine("Received message but cannot process, has odd number of dots.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No message on the queue.");
                }
                quit = Console.ReadLine().Contains("q");
            }
        }
    }
}
