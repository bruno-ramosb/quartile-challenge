using FluentValidation;
using MediatR;
using Quartile.Application.Common.Response;

namespace Quartile.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var resultType = typeof(TResponse);
                var failMethod = typeof(Result<>).MakeGenericType(resultType.GetGenericArguments()[0]).GetMethod("Fail", new[] { typeof(IEnumerable<FluentValidation.Results.ValidationFailure>) });
                
                if (failMethod != null)
                {
                    var result = failMethod.Invoke(null, new object[] { failures });
                    return (TResponse)result!;
                }
            }

            return await next();
        }
    }
} 