using Echo.Sample.RecordingSample;
using System.IO;
using Xunit;

namespace Echo.Sample.ReplaySample
{
    public class ApplicationTests
    {
        [Fact]
        public void ReplayRecording()
        {
            using (var reader = new StreamReader("98f2556d-dfd4-4935-97a2-54921f278347.echo"))
            {
                using (var player = new Player(reader))
                {
                    // get your dependency mock based on provided recording
                    var azureTable = player.GetReplayingTarget<ICloudTable>();

                    // test your modified application
                    var app = new ModifiedApplication(azureTable);

                    // run your application exactly same way as in RecordingSample

                    // Create a new customer entity with random first name
                    var customer = new CustomerEntity("Harp", "Walter")
                    {
                        Email = "Walter@contoso.com",
                        PhoneNumber = "425-555-0101"
                    };

                    // Note: this is expected to fail due intentionally
                    // broken implementation of ModifiedApplication
                    var relatedCustomers = app.AddCustomer(customer);
                    
                    Assert.Equal(5, relatedCustomers);
                }
            }
        }
    }
}
