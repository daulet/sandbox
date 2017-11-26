using System;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace Echo.Sample.RecordingSample
{
    internal class Program
    {
        /// <summary>
        /// Example is based on https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-dotnet
        /// </summary>
        private static void Main(string[] _)
        {
            // Create Azure Table if necessary
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("people");
            table.CreateIfNotExists();

            using (var writer = new StreamWriter(@"recording.echo"))
            {
                using (var recorder = new Recorder(writer))
                {
                    // get instance of your dependency
                    // NOTE: type is ICloudTable interface
                    // instead of more specific AzureTable
                    ICloudTable azureTable = new AzureTable(table);

                    // get your recordable dependency from recorder
                    azureTable = recorder.GetRecordingTarget(azureTable);

                    // finally, inject your recordable dependency
                    var app = new Application(azureTable);

                    // run your application

                    // Create a new customer entity.
                    var customer = new CustomerEntity("Harp", "Walter")
                    {
                        Email = "Walter@contoso.com",
                        PhoneNumber = "425-555-0101"
                    };

                    var relatedCustomers = app.AddCustomer(customer);
                    Console.WriteLine($"{relatedCustomers} related customers in storage.");
                }
            }

            Console.ReadKey();
        }
    }
}
