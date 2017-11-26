using Echo.Sample.RecordingSample;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace Echo.Sample.ReplaySample
{
    /// <summary>
    /// Slightly modified version of Application from RecordingSample.
    /// Note: no exception handling on Execute() call.
    /// </summary>
    public class ModifiedApplication
    {
        private readonly ICloudTable _table;

        public ModifiedApplication(ICloudTable table)
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
            _table.Insert(customer);

            // query the table
            var query = new TableFilterBuilder<CustomerEntity>()
                .And(x => x.PartitionKey, CompareOp.EQ, customer.PartitionKey);

            var userCount = _table.ExecuteQuery(query).Count();

            return userCount;
        }
    }
}
