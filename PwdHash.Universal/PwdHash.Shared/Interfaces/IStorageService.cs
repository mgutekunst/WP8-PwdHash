namespace PwdHash.Interfaces
{
    public interface IStorageService
    {
        T GetFromPasswordVault<T>(string key);

        void SaveToPasswordVault(string key, object obj);
        
    }
}
