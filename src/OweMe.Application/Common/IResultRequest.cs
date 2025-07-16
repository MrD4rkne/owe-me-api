using MediatR;

namespace OweMe.Application.Common;

public interface IResultRequest<T> : IRequest<Result<T>>;