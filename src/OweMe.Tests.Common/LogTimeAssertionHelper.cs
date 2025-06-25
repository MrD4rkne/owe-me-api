using System.Text.RegularExpressions;

namespace OweMe.Tests.Common;

public static class LogTimeAssertionHelper
{
    public static void ContainsExpectedTime(string logMessage, long expectedTime, double percent, string unit = "ms")
    {
        var regex = new Regex($@"\b(\d+)\s*{unit}\b", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        var match = regex.Match(logMessage);
        if (!match.Success)
        {
            throw new Exception($"Expected time to match {unit} on {logMessage}");
        }

        if (!long.TryParse(match.Groups[1].Value, out long actualTime))
        {
            throw new Exception($"Failed to parse actual time from log message: {logMessage}");
        }

        double lowerBound = expectedTime * (1 - percent / 100.0);
        double upperBound = expectedTime * (1 + percent / 100.0);

        if (actualTime < lowerBound || actualTime > upperBound)
        {
            throw new Exception(
                $"Actual time {actualTime} {unit} is not within the expected range of {lowerBound} to {upperBound} {unit} for log message: {logMessage}");
        }
    }
}