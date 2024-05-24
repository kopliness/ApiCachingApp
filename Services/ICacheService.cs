namespace ApiCachingApp.Services;

public interface ICacheService
{
    public Task<T> GetData<T>(string key);
    public Task SetData<T>(string key, T value, DateTimeOffset expirationTime);
    public Task RemoveData(string key);
}