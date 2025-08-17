namespace OweMe.Application.Ledgers.Commands.Create;

public record CreateLedgerCommand
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}