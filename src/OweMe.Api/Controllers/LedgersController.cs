using MediatR;
using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Common;
using OweMe.Api.Extensions;
using OweMe.Api.Identity;
using OweMe.Application.Ledgers;
using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Application.Ledgers.Queries.Get;

namespace OweMe.Api.Controllers;

public static class LedgersController
{
    public static void MapLedgersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ledgers", CreateLedger)
            .WithName("CreateLedger")
            .WithDescription("Create a new ledger that groups expenses and payments between users.")
            .Accepts<CreateLedgerCommand>("application/json")
            .Produces(StatusCodes.Status201Created)
            .ProducesExtendedProblem(StatusCodes.Status400BadRequest)
            .WithStandardProblems()
            .RequireAuthorization(Constants.POLICY_API_SCOPE);

        app.MapGet("/api/ledgers/{ledgerId:guid}", GetLedger)
            .WithName("GetLedger")
            .WithDescription("Get a ledger by ID.")
            .Produces<LedgerDto>()
            .ProducesExtendedProblem(StatusCodes.Status404NotFound)
            .WithStandardProblems()
            .RequireAuthorization(Constants.POLICY_API_SCOPE);
    }

    public static async Task<IResult> CreateLedger(
        [FromBody] CreateLedgerCommand createLedgerCommand,
        IMediator mediator)
    {
        var ledgerId = await mediator.Send(createLedgerCommand);
        return Results.Created($"/api/ledgers/{ledgerId}", null);
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