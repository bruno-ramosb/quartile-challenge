using Quartile.Application.Common.Response;
using Microsoft.AspNetCore.Mvc;


namespace Quartile.Api.Helper
{
    internal static class ResultHelper
    {
        public static IActionResult ToActionResult<T>(this Result<T> result) =>
            new ObjectResult(result) { StatusCode = (int)result.StatusCode };
    }
}