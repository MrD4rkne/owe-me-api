using System.Diagnostics.CodeAnalysis;
using OweMe.Application.Common.Results;

namespace OweMe.Application.Ledgers.Queries.Get;

[method: SetsRequiredMembers]
public record GetLedgerQuery(Guid Id) : IResultRequest<LedgerDto>;