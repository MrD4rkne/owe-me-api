using System.Diagnostics.CodeAnalysis;
using MediatR;
using OweMe.Application.Common;

namespace OweMe.Application.Ledgers.Queries.Get;

[method: SetsRequiredMembers]
public record GetLedgerQuery(Guid id) : IRequest<Result<LedgerDto>>
{
    public required Guid Id { get; init; } = id;
}