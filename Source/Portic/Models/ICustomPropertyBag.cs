namespace Portic.Models
{
    public interface IReadonlyCustomPropertyBag
    {
        T GetOrDefault<T>(string key, T defaultValue);
    }

    public interface ICustomPropertyBag : IReadonlyCustomPropertyBag
    {
        void SetProperty(string key, object value);
    }
}