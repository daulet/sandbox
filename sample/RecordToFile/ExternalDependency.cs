namespace Echo.Sample.RecordToFile
{
    public interface IExternalDependency
    {
        string Concat(string str1, string str2, string str3);
    }

    public class ExternalDependency : IExternalDependency
    {
        public string Concat(string str1, string str2, string str3)
        {
            return string.Concat(str1, str2, str3);
        }
    }
}
