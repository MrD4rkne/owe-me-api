namespace OweMe.Tests.Common;

public static class GuidHelper
{
    public static Guid CreateDifferentGuid(Guid other, int maxAttempts = 100)
    {
        if (maxAttempts <= 0)
            throw new ArgumentException("maxAttempts must be greater than 0", nameof(maxAttempts));

        for (int i = 0; i < maxAttempts; i++)
        {
            var newGuid = Guid.NewGuid();
            if (newGuid != other)
                return newGuid;
        }

        throw new InvalidOperationException("Unable to generate a unique Guid after maximum attempts.");
    }
}