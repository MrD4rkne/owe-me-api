using System.ComponentModel;
using OweMe.Api.Endpoints.Common;

namespace OweMe.Api.Endpoints.Ledgers.Get;

public sealed record GetLedgerResponse : AuditableEntityModel
{
    [Description("Unique identifier of the ledger.")]
    [ReadOnly(true)]
    public Guid Id { get; init; }

    [Description("Name of the ledger.")] public string Name { get; init; } = string.Empty;

    [Description("Description of the ledger.")]
    public string Description { get; init; } = string.Empty;
}