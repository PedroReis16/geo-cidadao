namespace GeoCidadao.Jobs.Exceptions
{
    public class JobDataKeyNotFoundException(string jobDataKey) : Exception($"Job data map '{jobDataKey}' não definido")
    {
        public string JobDataKey { get; private set; } = jobDataKey;
    }
}
