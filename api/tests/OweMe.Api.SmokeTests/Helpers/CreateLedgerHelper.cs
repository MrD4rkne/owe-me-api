using OweMe.Api.Client;
using Shouldly;

namespace OweMe.Api.SmokeTests.Helpers;

public static class CreateLedgerHelper
{
    public static async Task<Guid> CreateLedger(OweMeApiClient client,
        CreateLedgerRequest createLedgerRequest,
        CancellationToken cancellationToken = default)
    {
        var response = await client.CreateLedgerAsync(createLedgerRequest, cancellationToken);

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(201, "Expected status code 201 Created for successful ledger creation.");
        response.Headers.ShouldNotBeNull();

        return response.Headers.GetLocationHeaderValue("/api/ledgers/", Guid.Parse);
    }
}