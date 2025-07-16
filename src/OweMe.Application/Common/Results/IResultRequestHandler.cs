using MediatR;

namespace OweMe.Application.Common.Results;

public interface IResultRequestHandler<in TInput, TOutput> : IRequestHandler<TInput, Result<TOutput>>
    where TInput : IResultRequest<TOutput>;