namespace Portic.Models
{
    public interface IReadOnlyCustomPropertyBag
    {
        T GetOrDefault<T>(string key, T defaultValue);
    }

    public interface ICustomPropertyBag : IReadOnlyCustomPropertyBag
    {
        void SetProperty(string key, object value);
    }
}