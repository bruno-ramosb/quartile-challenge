using Quartile.Application.Common.Response;
using Microsoft.AspNetCore.Mvc;


namespace Quartile.Api.Helper
{
    internal static class ResultHelper
    {
        public static ActionResult<T> ToActionResult<T>(this Result<T> result)
        {
            if (result.Success)
            {
                return new ObjectResult(result.Data)
                {
                    StatusCode = (int)result.StatusCode
                };
            }

            return new ObjectResult(new
            {
                Notifications = result.Notifications
            })
            {
                StatusCode = (int)result.StatusCode
            };
        }
    }
}