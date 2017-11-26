using System.Linq;
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
            var insertOperation = TableOperation.Insert(customer);
            _table.Execute(insertOperation);

            // query the table
            var query = new TableQuery<CustomerEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, customer.PartitionKey));

            var userCount = _table.ExecuteQuery(query).Count();

            return userCount;
        }
    }
}
