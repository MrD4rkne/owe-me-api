namespace OweMe.Api.SmokeTests.Helpers;

public static class HeaderHelpers
{
    private const string LocationHeaderName = "Location";

    public static bool TryGetLocationHeaderValue(this IReadOnlyDictionary<string, IEnumerable<string>> headers,
        out string location)
    {
        location = string.Empty;
        if (!headers.TryGetValue(LocationHeaderName, out var locationHeader) || !locationHeader.Any())
        {
            return false;
        }

        location = locationHeader.First();
        return true;
    }

    public static T GetLocationHeaderValue<T>(this IReadOnlyDictionary<string, IEnumerable<string>> headers,
        string prefix,
        Func<string, T> converter)
    {
        if (!TryGetLocationHeaderValue(headers, out string locationHeader))
        {
            throw new ArgumentException($"Location header '{LocationHeaderName}' not found in headers.");
        }

        if (!locationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"Location header '{LocationHeaderName}' does not start with the expected prefix '{prefix}'.");
        }

        string value = locationHeader[prefix.Length..];
        return converter(value);
    }
}