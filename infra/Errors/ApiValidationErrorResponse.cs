using System.Collections.Generic;
using infr.Errors;

namespace infra.Errors
{
    public class ApiValidationErrorResponse: ApiResponse
    {
        public ApiValidationErrorResponse() : base(400)
        {
        }

        public IEnumerable<string> Errors { get; set; }
    }
}