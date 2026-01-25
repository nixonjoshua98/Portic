namespace Portic.Models
{
    internal interface IReadOnlyCustomPropertyBag
    {
        T GetOrDefault<T>(string key, T defaultValue);
    }
}