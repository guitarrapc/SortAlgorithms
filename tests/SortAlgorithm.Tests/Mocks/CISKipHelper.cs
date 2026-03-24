namespace SortAlgorithm.Tests;

public class SkipCIAttribute : SkipAttribute
{
    public SkipCIAttribute() : base("Local only test") { }

    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(Environment.GetEnvironmentVariable("CI")?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false);
}
