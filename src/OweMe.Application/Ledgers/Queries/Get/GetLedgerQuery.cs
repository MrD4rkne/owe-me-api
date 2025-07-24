using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace OweMe.Application.Ledgers.Queries.Get;

[method: SetsRequiredMembers]
public record GetLedgerQuery(Guid Id) : IRequest<LedgerDto>;