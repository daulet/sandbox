using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Echo.Sample.RecordingSample
{
    public class Application
    {
        private readonly ICloudTable _table;

        public Application(ICloudTable table)
        {
            _table = table;
        }

        /// <summary>
        /// This method adds a customer to Azure table
        /// and returns number of customers with the same last name.
        /// </summary>
        /// <returns>
        /// Number of customers with the same last name.
        /// </returns>
        public int AddCustomer(CustomerEntity customer)
        {
            // Add new customer to the table
            try
            {
                _table.Insert(customer);
            }
            catch (StorageException ex)
                when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                // customer record already exists
            }

            // query the table
            var query = new TableFilterBuilder<CustomerEntity>()
                .And(x => x.PartitionKey, CompareOp.EQ, customer.PartitionKey);

            var userCount = _table.ExecuteQuery(query).Count();

            return userCount;
        }
    }
}
