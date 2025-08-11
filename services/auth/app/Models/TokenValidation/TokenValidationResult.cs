using System.Collections.Generic;

namespace app.Models.TokenValidation
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, string> Claims { get; set; }
        public string ErrorMessage { get; set; }
    }
}