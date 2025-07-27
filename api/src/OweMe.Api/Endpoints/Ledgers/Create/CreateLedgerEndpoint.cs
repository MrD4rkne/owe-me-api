using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Description;
using OweMe.Api.Identity;
using OweMe.Application.Ledgers.Commands.Create;

namespace OweMe.Api.Endpoints.Ledgers.Create;

public sealed class CreateLedgerEndpoint : IEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ledgers", CreateLedger)
            .WithName("CreateLedger")
            .WithDescription("Create a new ledger that groups expenses and payments between users.")
            .WithTags(Tags.Ledger)
            .Accepts<CreateLedgerRequest>("application/json")
            .Produces(StatusCodes.Status201Created)
            .ProducesExtendedProblem(StatusCodes.Status400BadRequest)
            .WithStandardProblems()
            .RequireAuthorization(Constants.POLICY_API_SCOPE);
    }

    public static async Task<IResult> CreateLedger(
        [FromBody] CreateLedgerRequest createLedgerRequest,
        IMediator mediator)
    {
        var createLedgerCommand = new CreateLedgerCommand
        {
            Name = createLedgerRequest.Name,
            Description = createLedgerRequest.Description
        };

        var ledgerId = await mediator.Send(createLedgerCommand);
        return Results.Created($"/api/ledgers/{ledgerId}", null);
    }
}