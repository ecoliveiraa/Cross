using System;

namespace WebApplication1.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) {
        
        //aqui devia receber um codigo, anyway. Devia ser a classe pai. Depois tinhas excecoes custom para cada Caso e na qual defenias o codigo e extendiam esta
        
        }
    }
}