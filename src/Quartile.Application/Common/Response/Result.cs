using FluentValidation.Results;
using Quartile.Application.Common.Constants;
using System.Net;
using System.Text.Json.Serialization;

namespace Quartile.Application.Common.Response
{
    public class Result<T> : IResult<T>
    {
        protected internal Result(T data, string message, HttpStatusCode statusCode)
        {
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }

        protected internal Result(List<string> errors, HttpStatusCode statusCode)
        {
            Notifications = errors;
            StatusCode = statusCode;
        }


        public static Result<T> Successful(T data, string message = Messages.SUCCESSFUL_OPERATION, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var result =  new Result<T>(data,message, statusCode);
            result.Success = true;
            return result;
        }

        public static Result<T> Fail(List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var result = new Result<T>(errors, statusCode);
            result.Success = false;
            return result;
        }

        public static Result<T> Fail(string erro, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var result = new Result<T>(new List<string> { erro }, statusCode);
            result.Success = false;
            return result;
        }

        public static Result<T> Fail(IEnumerable<ValidationFailure> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            List<string> erros = new List<string>();
            foreach (var error in errors)
                erros.Add(error.ErrorMessage);

            var result = new Result<T>(erros, statusCode);
            result.Success = false;
            return result;
        }


        public bool Success { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T Data { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string> Notifications { get; private set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; private set; }
    }
}
