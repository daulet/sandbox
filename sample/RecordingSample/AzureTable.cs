using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

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

        public TableResult Insert<TElement>(TElement element)
            where TElement : ITableEntity, new()
        {
            return _table.Execute(TableOperation.Insert(element));
        }

        public TElement[] ExecuteQuery<TElement>(TableFilterBuilder<TElement> filterBuilder)
            where TElement : ITableEntity, new()
        {
            var query = new TableQuery<TElement>()
            {
                FilterString = filterBuilder.ToString(),
            };

            return _table.ExecuteQuery(query).ToArray();
        }
    }
}
