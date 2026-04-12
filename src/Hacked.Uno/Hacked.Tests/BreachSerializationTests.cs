using System.Collections.Generic;
using System.Text.Json;

namespace Hacked.Tests;

public class BreachSerializationTests
{
    [Test]
    public void Breach_IsSelected_ExcludedFromJson()
    {
        var breach = new Breach { Title = "Adobe", IsSelected = true };
        var json = JsonSerializer.Serialize(breach);

        json.Should().NotContain("IsSelected");
    }

    [Test]
    public void Breach_Id_ExcludedFromJson()
    {
        var breach = new Breach { Title = "Adobe" };
        var json = JsonSerializer.Serialize(breach);

        // Id is [JsonIgnore] — should not appear as a JSON property
        json.Should().NotContain("\"Id\"");
    }

    [Test]
    public void Breach_IsNew_IncludedInJson()
    {
        var breach = new Breach { Title = "Adobe", IsNew = true };
        var json = JsonSerializer.Serialize(breach);

        json.Should().Contain("IsNew");
    }

    [Test]
    public void Breach_IsNew_RoundTrips()
    {
        var breach = new Breach { Title = "Adobe", Name = "Adobe", IsNew = true };
        var json = JsonSerializer.Serialize(breach);
        var deserialized = JsonSerializer.Deserialize<Breach>(json)!;

        deserialized.IsNew.Should().BeTrue();
    }

    [Test]
    public void Breach_PwnCount_RoundTrips()
    {
        var breach = new Breach { Title = "Adobe", Name = "Adobe", PwnCount = 152445165L };
        var json = JsonSerializer.Serialize(breach);
        var deserialized = JsonSerializer.Deserialize<Breach>(json)!;

        deserialized.PwnCount.Should().Be(152445165L);
    }

    [Test]
    public void Breach_BreachDate_RoundTrips()
    {
        var date = new DateTime(2013, 10, 4, 0, 0, 0, DateTimeKind.Utc);
        var breach = new Breach { Title = "Adobe", Name = "Adobe", BreachDate = date };
        var json = JsonSerializer.Serialize(breach);
        var deserialized = JsonSerializer.Deserialize<Breach>(json)!;

        deserialized.BreachDate.Should().Be(date);
    }

    [Test]
    public void Breach_DataClasses_RoundTrips()
    {
        var breach = new Breach
        {
            Title = "Adobe",
            Name = "Adobe",
            DataClasses = new List<string> { "Email addresses", "Passwords" }
        };
        var json = JsonSerializer.Serialize(breach);
        var deserialized = JsonSerializer.Deserialize<Breach>(json)!;

        deserialized.DataClasses.Should().BeEquivalentTo(new[] { "Email addresses", "Passwords" });
    }
}
