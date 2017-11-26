using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Echo.Sample.RecordingSample
{
    /// <summary>
    /// This interface is an abstraction of Azure CloudTable.
    /// In order to use Recorder and Player it is necessary to
    /// declare dependencies as public interfaces. It is also
    /// a good design practice to abstract your dependencies,
    /// e.g. it simplifies mocking.
    /// </summary>
    public interface ICloudTable
    {
        TableResult Insert<TElement>(TElement element)
            where TElement : ITableEntity, new();

        TElement[] ExecuteQuery<TElement>(TableFilterBuilder<TElement> filterBuilder)
            where TElement : ITableEntity, new();
    }
}
