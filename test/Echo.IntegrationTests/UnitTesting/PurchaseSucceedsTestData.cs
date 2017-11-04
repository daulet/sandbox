using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Echo.IntegrationTests.UnitTesting
{
    internal class PurchaseSucceedsTestData : IEnumerable<object[]>
    {
        private readonly IList<string> _testCaseFilenames = new List<string>
        {
            "Echo.IntegrationTests.Recording.HappyCase.echo",
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var testCaseFilename in _testCaseFilenames)
            {
                yield return new object[]
                {
                    new EchoReader(
                        new StreamReader(
                            Assembly.GetExecutingAssembly().GetManifestResourceStream(testCaseFilename)))
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
