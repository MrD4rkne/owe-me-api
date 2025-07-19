using MediatR;
using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Identity;
using OweMe.Application.Common;
using OweMe.Application.Ledgers;
using OweMe.Application.Ledgers.Commands.Create;
using OweMe.Application.Ledgers.Queries.Get;
using OweMe.Domain.Ledgers;

namespace OweMe.Api.Controllers;

public static class LedgersController
{
    public static void MapLedgersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ledgers", CreateLedger)
            .WithName("CreateLedger")
            .WithDescription("Create a new ledger that groups expenses and payments between users.")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Constants.POLICY_API_SCOPE);

        app.MapGet("/api/ledgers/{ledgerId:guid}", GetLedger)
            .WithName("GetLedger")
            .WithDescription("Get a ledger by ID.")
            .Produces<LedgerDto>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Constants.POLICY_API_SCOPE);
    }

    public static async Task<IResult> CreateLedger(
        [FromBody] CreateLedgerCommand createLedgerCommand,
        IMediator mediator)
    {
        var creationResult = await mediator.Send(createLedgerCommand);
        var ledgerId = creationResult.Value;
        return Results.Created($"/api/ledgers/{ledgerId}", null);
    }

    public static async Task<IResult> GetLedger(
        [FromRoute] Guid ledgerId,
        IMediator mediator)
    {
        var query = new GetLedgerQuery(ledgerId);
        var ledger = await mediator.Send(query);
        return ledger switch
        {
            { IsSuccess: true } => Results.Ok(ledger.Value),
            { Error: { Code: LedgerErrors.Codes.LedgerNotFound } } => Results.NotFound(ledger.Error.Description),
            _ => throw new InvalidOperationException("Unexpected result from GetLedgerQuery")
        };
    }
}