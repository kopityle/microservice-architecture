namespace Common.Http;

public interface ITraceIdAccessor
{
    string GetValue();
    void WriteValue(string? value);
}

internal class TraceIdAccessor : ITraceIdAccessor
{
    private string _value = Guid.NewGuid().ToString(); // Сразу генерируем, если не пришел извне

    public string GetValue()
    {
        return _value;
    }

    public void WriteValue(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _value = value;
        }
    }
}