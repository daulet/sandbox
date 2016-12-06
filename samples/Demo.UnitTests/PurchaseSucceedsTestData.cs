using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Samples.Demo.UnitTests
{
    internal class PurchaseSucceedsTestData : IEnumerable<object[]>
    {
        private readonly IList<string> _testCaseFilenames = new List<string>
        {
            "Samples.Demo.UnitTests.Echoes.HappyCase.echo",
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var testCaseFilename in _testCaseFilenames)
            {
                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testCaseFilename))
                {
                    using (var streamReader = new StreamReader(resourceStream))
                    {
                        var echoes = new List<string>();
                        while (!streamReader.EndOfStream)
                        {
                            echoes.Add(streamReader.ReadLine());
                        }
                        yield return new object[] { new EchoReader(echoes) };
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
