using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Description;
using OweMe.Api.Endpoints.Common;
using OweMe.Api.Identity;
using OweMe.Application.Ledgers.Queries.Get;
using Wolverine;

namespace OweMe.Api.Endpoints.Ledgers.Get;

public sealed class GetLedgerByIdEndpoint : IEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/ledgers/{ledgerId:guid}", GetLedger)
            .WithName("GetLedger")
            .WithDescription("Get a ledger by ID.")
            .WithTags(Tags.Ledger)
            .Produces<GetLedgerResult>()
            .ProducesExtendedProblem(StatusCodes.Status404NotFound)
            .WithStandardProblems()
            .RequireAuthorization(Constants.POLICY_API_SCOPE);
    }

    public static async Task<IResult> GetLedger(
        [FromRoute] Guid ledgerId,
        IMessageBus messageBus)
    {
        var query = new GetLedgerQuery(ledgerId);
        var ledger = await messageBus.InvokeAsync<GetLedgerResult>(query);
        return Results.Ok(ledger);
    }
}