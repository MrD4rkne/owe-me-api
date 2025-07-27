using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Description;
using OweMe.Api.Identity;
using OweMe.Application.Ledgers;
using OweMe.Application.Ledgers.Queries.Get;

namespace OweMe.Api.Endpoints.Ledgers;

public sealed class GetLedgerByIdEndpoint : IEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/ledgers/{ledgerId:guid}", GetLedger)
            .WithName("GetLedger")
            .WithDescription("Get a ledger by ID.")
            .WithTags(Tags.Ledger)
            .Produces<LedgerDto>()
            .ProducesExtendedProblem(StatusCodes.Status404NotFound)
            .WithStandardProblems()
            .RequireAuthorization(Constants.POLICY_API_SCOPE);
    }

    public static async Task<IResult> GetLedger(
        [FromRoute] Guid ledgerId,
        IMediator mediator)
    {
        var query = new GetLedgerQuery(ledgerId);
        var ledger = await mediator.Send(query);
        return Results.Ok(ledger);
    }
}