using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Echo.Sample.RecordingSample
{
    /// <summary>
    /// Simple wrapper around CloudTable that implements ICloudTable
    /// </summary>
    public class AzureTable : ICloudTable
    {
        private readonly CloudTable _table;

        public AzureTable(CloudTable table)
        {
            _table = table;
        }

        public TableResult Execute(TableOperation operation)
        {
            return _table.Execute(operation);
        }

        public IEnumerable<TElement> ExecuteQuery<TElement>(TableFilterBuilder<TElement> filterBuilder)
            where TElement : ITableEntity, new()
        {
            var query = new TableQuery<TElement>()
            {
                FilterString = filterBuilder.ToString(),
            };

            return _table.ExecuteQuery(query);
        }
    }
}
