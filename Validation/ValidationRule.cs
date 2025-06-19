using System;

namespace WebApplication1.Validation
{
    public class ValidationRule
    {
        public string ClassName { get; set; }
        public string FieldName { get; set; }
        public Func<object, bool> Condition { get; set; }
        public bool IsRequired { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationRule(string className, string fieldName, Func<object, bool> condition, bool isRequired, string errorMessage)
        {
            ClassName = className;
            FieldName = fieldName;
            Condition = condition;
            IsRequired = isRequired;
            ErrorMessage = errorMessage;
        }
    }
}