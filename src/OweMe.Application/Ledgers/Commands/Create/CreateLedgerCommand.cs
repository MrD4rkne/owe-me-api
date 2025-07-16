using MediatR;
using OweMe.Application.Common;

namespace OweMe.Application.Ledgers.Commands.Create;

public record CreateLedgerCommand : IResultRequest<Guid>
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}