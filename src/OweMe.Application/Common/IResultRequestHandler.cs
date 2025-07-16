using MediatR;

namespace OweMe.Application.Common;

public interface IResultRequestHandler<in TInput, TOutput> : IRequestHandler<TInput, Result<TOutput>>
    where TInput : IResultRequest<TOutput>;