using System;
using Microsoft.AspNetCore.Mvc;

namespace Login.Api.Extensions
{
    public static class ExceptionExtensions
    {
        public static ProblemDetails ToProblemDetails(this Exception ex, string errorTitle)
        {
            return new ProblemDetails
            {
                Title = errorTitle,
                Detail = ex.Message
            };
        }
    }
}
