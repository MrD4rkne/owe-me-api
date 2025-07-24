namespace OweMe.Api.Description;

public static class ProblemsExtensions
{
    private static readonly string ProblemDetailsContentType = "application/problem+json";
    
    /// <summary>
    /// Configures the endpoint to produce standard problem details responses
    /// </summary>
    public static TBuilder WithStandardProblems<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        return builder.ProducesExtendedProblem(StatusCodes.Status500InternalServerError);
    }
    
    /// <summary>
    /// Configures the endpoint to produce extended problem details responses
    /// </summary>
    public static TBuilder ProducesExtendedProblem<TBuilder>(this TBuilder builder, int statusCode, string? contentType = null)
        where TBuilder : IEndpointConventionBuilder
    {
        if (string.IsNullOrEmpty(contentType))
        {
            contentType = ProblemDetailsContentType;
        }

        return builder.WithMetadata(new ProducesResponseTypeMetadata(statusCode, typeof(ExtendedProblemDetails), [contentType]));
    }
}
