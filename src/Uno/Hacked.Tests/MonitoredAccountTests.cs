using System.Text.Json;

namespace Hacked.Tests;

public class MonitoredAccountTests
{
    [Test]
    public void MonitoredAccount_HasNewBreaches_FalseWhenNoNewBreaches()
    {
        var account = new MonitoredAccount { Address = "test@example.com" };
        account.Breaches.Add(new Breach { Title = "SomeSite", IsNew = false });

        account.HasNewBreaches.Should().BeFalse();
    }

    [Test]
    public void MonitoredAccount_NewBreachCount_ReturnsCorrectCount()
    {
        var account = new MonitoredAccount { Address = "test@example.com" };
        account.Breaches.Add(new Breach { Title = "Site1", IsNew = true });
        account.Breaches.Add(new Breach { Title = "Site2", IsNew = true });
        account.Breaches.Add(new Breach { Title = "Site3", IsNew = false });

        account.NewBreachCount.Should().Be(2);
    }

    [Test]
    public void MonitoredAccount_NewBreachCount_ZeroWhenNoBreaches()
    {
        var account = new MonitoredAccount { Address = "empty@example.com" };

        account.NewBreachCount.Should().Be(0);
        account.HasNewBreaches.Should().BeFalse();
    }

    [Test]
    public void MonitoredAccount_Serialization_RoundTrip()
    {
        var account = new MonitoredAccount
        {
            Address = "test@example.com",
            Id = "test-id",
            LastUpdated = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };
        account.Breaches.Add(new Breach { Title = "Adobe", Name = "Adobe", IsNew = true });

        var json = JsonSerializer.Serialize(account);
        var deserialized = JsonSerializer.Deserialize<MonitoredAccount>(json)!;

        deserialized.Address.Should().Be(account.Address);
        deserialized.Id.Should().Be(account.Id);
        deserialized.Breaches.Should().HaveCount(1);
        deserialized.Breaches[0].Title.Should().Be("Adobe");
    }

    [Test]
    public void MonitoredAccount_Serialization_JsonIgnorePropertiesDefaultToFalse()
    {
        var account = new MonitoredAccount { Address = "test@example.com" };
        // IsSelected and IsUpdating are [JsonIgnore] — should not persist

        var json = JsonSerializer.Serialize(account);
        var deserialized = JsonSerializer.Deserialize<MonitoredAccount>(json)!;

        deserialized.IsSelected.Should().BeFalse();
        deserialized.IsUpdating.Should().BeFalse();
    }
}
