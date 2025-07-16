using MediatR;

namespace OweMe.Application.Common.Results;

public interface IResultRequest<T> : IRequest<Result<T>>;