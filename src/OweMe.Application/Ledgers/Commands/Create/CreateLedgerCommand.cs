using MediatR;

namespace OweMe.Application.Ledgers.Commands.Create;

public record CreateLedgerCommand : IRequest<Guid>
{
    public string Name { get; set; }
    
    public string Description { get; set; }
}