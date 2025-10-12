using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using OweMe.Api.Description;
using OweMe.Api.Endpoints.Common;
using OweMe.Api.Identity;
using OweMe.Application.Ledgers.Commands.Create;
using Wolverine;

namespace OweMe.Api.Endpoints.Ledgers.Create;

public sealed class CreateLedgerEndpoint : IEndpoint
{
    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/ledgers", CreateLedger)
            .WithName("CreateLedger")
            .WithDescription("Create a new ledger that groups expenses and payments between users.")
            .WithTags(Tags.Ledger)
            .Produces(StatusCodes.Status201Created)
            .ProducesExtendedProblem(StatusCodes.Status400BadRequest)
            .WithStandardProblems()
            .RequireAuthorization(Constants.POLICY_API_SCOPE);
    }

    public static async Task<IResult> CreateLedger(
        [FromBody] CreateLedgerCommand createLedgerRequest,
        IMessageBus messageBus,
        CancellationToken cancellationToken = default)
    {
        var createLedgerCommand = new CreateLedgerCommand
        {
            Name = createLedgerRequest.Name,
            Description = createLedgerRequest.Description
        };

        var ledger =
            await messageBus.InvokeAsync<CreateLedgerCommandHandler.LedgerCreated>(createLedgerCommand,
                cancellationToken);
        return Results.Created($"/api/ledgers/{ledger.Id}", null);
    }
}