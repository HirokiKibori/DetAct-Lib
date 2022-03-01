using Xunit;
using Xunit.Abstractions;

public abstract class BasicTest
{
    protected readonly ITestOutputHelper output;

    public BasicTest(ITestOutputHelper output) => this.output = output;
}