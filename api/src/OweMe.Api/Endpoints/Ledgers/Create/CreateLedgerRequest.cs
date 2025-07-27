using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OweMe.Domain.Ledgers;

namespace OweMe.Api.Endpoints.Ledgers.Create;

public sealed record CreateLedgerRequest
{
    [Description("The name of the ledger.")]
    [Required]
    [StringLength(LedgerConstants.MaxNameLength, MinimumLength = 1)]
    public required string Name { get; init; }

    [Description("A description of the ledger.")]
    [StringLength(LedgerConstants.MaxDescriptionLength, MinimumLength = 1)]
    public string? Description { get; init; }
}