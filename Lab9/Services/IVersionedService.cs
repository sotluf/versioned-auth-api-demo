namespace Lab9.Services;

public interface IVersionedService
{
    public Task<int> GetIntegerAsync();
    public Task<string> GetTextAsync();
    public Task<byte[]> GenerateExcelAsync();
}
