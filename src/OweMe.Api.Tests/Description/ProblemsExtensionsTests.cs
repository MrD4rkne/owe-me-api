using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OweMe.Api.Common;
using OweMe.Api.Extensions;
using Shouldly;

namespace OweMe.Api.Tests.Description;

public class ProblemsExtensionsTests
{
    /// <summary>
    /// Represents the content type for problem details responses.
    /// </summary>
    private const string ProblemDetailsContentType = "application/problem+json";
    
    [Fact]
    public void WithStandardProblems_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new MyBuilder();

        // Act
        var result = builder.WithStandardProblems();

        // Assert
        
        result.ShouldBeOfType<MyBuilder>();
        result.ShouldBeAssignableTo<IEndpointConventionBuilder>();
        result.ShouldBe(builder);
    }
    
    [Fact]
    public void WithStandardProblems_ShouldRegisterAllCommonErrors()
    {
        // Arrange
        var builder = new MyBuilder();
        IReadOnlyCollection<int> expectedStatusCodes = [500];

        // Act
        var result = builder.WithStandardProblems();
        var action = result.EndpointConventionBuilders[0]; 
        var endpointBuilder = new MyEndpointBuilder();
        
        action(endpointBuilder);
        
        // Assert
        endpointBuilder.Metadata.Count.ShouldBe(expectedStatusCodes.Count);
        foreach (int statusCode in expectedStatusCodes)
        {
            var metadataForCode = endpointBuilder.Metadata
                .OfType<ProducesResponseTypeMetadata>()
                .FirstOrDefault(m => m.StatusCode == statusCode);
            metadataForCode.ShouldNotBeNull($"Should register problem details model for code {statusCode}");
            metadataForCode.ShouldBeOfType<ProducesResponseTypeMetadata>();
            metadataForCode.ShouldBeEquivalentTo(
                new ProducesResponseTypeMetadata(statusCode, typeof(ExtendedProblemDetails), [ProblemDetailsContentType]),
                $"Should register problem details model: {nameof(ExtendedProblemDetails)} for code {statusCode}");
        }
    }
    
    [Fact]
    public void ProducesExtendedProblem_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new MyBuilder();
        const int code = 400;

        // Act
        var result = builder.ProducesExtendedProblem(code);

        // Assert
        result.ShouldBeOfType<MyBuilder>();
        result.ShouldBeAssignableTo<IEndpointConventionBuilder>();
        result.ShouldBe(builder);
    }
    
    [Fact]
    public void ProducesExtendedProblem_ShouldRegisterProvidedCode()
    {
        // Arrange
        var builder = new MyBuilder();
        const int code = 400;

        // Act
        var result = builder.ProducesExtendedProblem(code);
        
        var action = result.EndpointConventionBuilders[0]; 
        var endpointBuilder = new MyEndpointBuilder();
        action(endpointBuilder);
        
        // Assert
        endpointBuilder.Metadata.Count.ShouldBe(1);
        endpointBuilder.Metadata[0].ShouldBeOfType<ProducesResponseTypeMetadata>();
        endpointBuilder.Metadata[0].ShouldBeEquivalentTo(
            new ProducesResponseTypeMetadata(code, typeof(ExtendedProblemDetails), [ProblemDetailsContentType]),
            $"Should register problem details content for code {code}, model: {nameof(ExtendedProblemDetails)}");
    }
    
    private class MyEndpointBuilder : EndpointBuilder
    {
        public override Endpoint Build()
        {
            throw new NotImplementedException();
        }
    }
    
    private class MyBuilder : IEndpointConventionBuilder
    {
        public List<Action<EndpointBuilder>> EndpointConventionBuilders { get; } = [];
        
        public void Add(Action<EndpointBuilder> convention)
        {
            EndpointConventionBuilders.Add(convention);
        }

        public void Finally(Action<EndpointBuilder> finallyConvention)
        {
            throw new NotImplementedException();
        }
    }
}