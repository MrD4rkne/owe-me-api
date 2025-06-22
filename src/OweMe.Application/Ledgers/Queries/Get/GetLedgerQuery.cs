using MediatR;
using OweMe.Application.Common;

namespace OweMe.Domain.Ledgers.Queries.Get;

public record GetLedgerQuery(Guid id) : IRequest<Result<LedgerDto>>
{
    public Guid Id { get; set; } = id;
}