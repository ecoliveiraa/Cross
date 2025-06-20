using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApplication1.Models;

namespace WebApplication1.Validation
{
    public class RuleManager : IRuleManager
    {
        private readonly List<ValidationRule> _rules = new();

        public void SetRequired<T>(string fieldName, Func<T, bool> condition, bool isRequired, string errorMessage)
        {
            var className = typeof(T).Name;
            
            var rule = new ValidationRule(
                className, 
                fieldName, 
                obj => condition((T)obj), 
                isRequired, 
                errorMessage
            );
            
            _rules.Add(rule);
        }

        public ValidationResult Validate<T>(T entity)
        {
            var result = new ValidationResult();
            var className = typeof(T).Name;
            var entityType = typeof(T);

            var applicableRules = _rules.Where(r => r.ClassName == className);

            foreach (var rule in applicableRules)
            {
                if (rule.Condition(entity) && rule.IsRequired)
                {
                    var property = entityType.GetProperty(rule.FieldName, BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        var value = property.GetValue(entity);
                        
                        if (IsFieldEmpty(value))
                        {
                            result.AddError(rule.ErrorMessage);
                        }
                    }
                }
            }

            return result;
        }

      private bool IsFieldEmpty(object value)
      {
          if (value == null) return true;
          if (value is string str) return string.IsNullOrWhiteSpace(str);
          if (value is Guid guid) return guid == Guid.Empty;
          if (value is int intVal) return intVal == 0;
          if (value is decimal decVal) return decVal == 0;

          if (value is System.Collections.ICollection collection) return collection.Count == 0;
          
          return false;
      }
    }
}
