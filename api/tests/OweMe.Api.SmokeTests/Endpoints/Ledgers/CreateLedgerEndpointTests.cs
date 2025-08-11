using OweMe.Api.Client;
using OweMe.Api.SmokeTests.Helpers;
using Shouldly;

namespace OweMe.Api.SmokeTests.Endpoints.Ledgers;

public class CreateLedgerEndpointTests(OweMeClientFixture fixture)
{
    private readonly CreateLedgerRequest validCreateLedgerRequest = new()
    {
        Name = "Test Ledger",
        Description = "This is a test ledger."
    };

    [Fact]
    public async Task For_ValidRequest_Should_CreateLedgerSuccessfully()
    {
        // Arrange
        var client = fixture.GetClient(OweMeClientFixture.AuthenticatedClientKey);

        // Act
        var ledgerId =
            await CreateLedgerHelper.CreateLedger(client, validCreateLedgerRequest,
                TestContext.Current.CancellationToken);

        // Assert
        ledgerId.ShouldNotBe(Guid.Empty, "Ledger ID should not be empty after successful creation.");

        var ledger = await client.GetLedgerAsync(ledgerId);
        ledger.ShouldNotBeNull("Ledger should not be null after creation.");
        ledger.Result.ShouldNotBeNull("Ledger result should not be null.");
        ledger.Result.Id.ShouldBe(ledgerId, "Ledger ID should match the created ledger ID.");
        ledger.Result.Name.ShouldBe(validCreateLedgerRequest.Name, "Ledger name should match the request.");
        ledger.Result.Description.ShouldBe(validCreateLedgerRequest.Description,
            "Ledger description should match the request.");
        ledger.Result.CreatedBy.ShouldNotBe(Guid.Empty, "Ledger created by should not be empty.");
        ledger.Result.UpdatedAt.ShouldBeNull("Ledger updated date should be null for a newly created ledger.");
        ledger.Result.UpdatedBy.ShouldBeNull("Ledger updated by should be null for a newly created ledger.");
    }

    [Fact]
    public async Task For_UnauthorizedUser_ShouldReturn_Unauthorized()
    {
        // Arrange
        var client = fixture.GetClient(OweMeClientFixture.UnauthenticatedClientKey);

        // Act
        var apiException = await Should.ThrowAsync<ApiException>(() =>
            client.CreateLedgerAsync(validCreateLedgerRequest, TestContext.Current.CancellationToken));

        // Assert
        apiException.ShouldNotBeNull("API exception should not be null for unauthorized request.");
        apiException.StatusCode.ShouldBe(401, "Expected status code 401 Unauthorized for unauthenticated request.");
    }
}