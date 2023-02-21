using FluentAssertions;

namespace TestExample;

public class UnitTest
{
    [Fact]
    public void Test1()
    {
        var name = "name";
        name.Should().Be("name");
    }
}
