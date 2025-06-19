using System;

namespace WebApplication1.Validation
{
    public interface IRuleManager
    {
        void SetRequired<T>(string fieldName, Func<T, bool> condition, bool isRequired, string errorMessage);
        ValidationResult Validate<T>(T entity);
    }
}