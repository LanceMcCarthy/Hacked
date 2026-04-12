namespace Hacked.Tests;

public class BreachModelTests
{
    [Test]
    public void Breach_Equality_IsCaseInsensitiveOnTitle()
    {
        var b1 = new Breach { Title = "Adobe" };
        var b2 = new Breach { Title = "adobe" };

        b1.Should().Be(b2);
    }

    [Test]
    public void Breach_IsNew_DefaultsToFalse()
    {
        var breach = new Breach { Name = "Test", Title = "Test" };
        breach.IsNew.Should().BeFalse();
    }

    [Test]
    public void Breach_Id_ReturnsTitle()
    {
        var breach = new Breach { Title = "LinkedIn" };
        breach.Id.Should().Be("LinkedIn");
    }

    [Test]
    public void MonitoredAccount_HasNewBreaches_WhenBreachIsNew()
    {
        var account = new MonitoredAccount { Address = "test@example.com" };
        account.Breaches.Add(new Breach { Title = "SomeSite", IsNew = true });

        account.HasNewBreaches.Should().BeTrue();
        account.NewBreachCount.Should().Be(1);
    }
}
