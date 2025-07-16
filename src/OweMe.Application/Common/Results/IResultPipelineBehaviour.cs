using MediatR;

namespace OweMe.Application.Common.Results;

public interface IResultPipelineBehaviour<in TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
}