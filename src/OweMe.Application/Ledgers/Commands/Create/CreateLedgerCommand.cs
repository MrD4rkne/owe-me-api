using MediatR;

namespace OweMe.Application.Ledgers.Commands.Create;

public record CreateLedgerCommand : IRequest<Guid>
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}