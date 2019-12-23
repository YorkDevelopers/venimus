using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.Services
{
    internal class APIResult
    {
        private ValidationProblemDetails _errorDetails;

        public bool WasASuccess { get; private set; }

        private APIResult()
        {
            WasASuccess = true;
        }

        private APIResult(ValidationProblemDetails errorDetails)
        {
            _errorDetails = errorDetails;
            WasASuccess = false;
        }

        internal static APIResult Failure(ValidationProblemDetails errorDetails)
        {
            return new APIResult(errorDetails);
        }

        internal static APIResult Success()
        {
            return new APIResult();
        }

        internal ActionResult Evalulate(Func<ActionResult> onSuccess, Func<ValidationProblemDetails, ActionResult> onFailure)
        {
            return WasASuccess switch
            {
                true => onSuccess(),
                false => onFailure(_errorDetails),
            };
        }

        internal static async Task<APIResult> Create(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return Success();
            }
            else
            {
                var text = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var validationProblemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(text, options);
                return Failure(validationProblemDetails);
            }
        }
    }
}