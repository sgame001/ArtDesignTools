namespace HKLibrary
{
    public interface IConfigManager
    {
        void Set(string _key, object _value);

        T Get<T>(string _key);

        T Get<T>(string _key, T _defaultValue);
    }
}