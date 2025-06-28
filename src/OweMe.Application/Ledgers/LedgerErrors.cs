using OweMe.Application.Common;

namespace OweMe.Domain.Ledgers;

public static class LedgerErrors
{
    public static class Errors
    {
        public static readonly Error LedgerNotFound = new(
            Codes.LedgerNotFound,
            "The requested ledger was not found or user does not have access to it."
        );
    }

    public static class Codes
    {
        public const string LedgerNotFound = "Ledger.NotFound";
    }
}