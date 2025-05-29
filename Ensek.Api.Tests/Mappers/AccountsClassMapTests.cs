using Ensek.Api.Mappers;

namespace Ensek.Api.Tests.Mappers;

public class AccountsClassMapTests
{
    [Test]
    public void Map_AccountId_MapsToAccountIdColumn()
    {
        var map = new AccountsClassMap();
        var memberMap = map.MemberMaps.SingleOrDefault(m => m.Data.Member.Name == "AccountId");
        Assert.That(memberMap, Is.Not.Null);
        Assert.That(memberMap.Data.Names, Contains.Item("AccountId"));
    }

    [Test]
    public void Map_FirstName_MapsToFirstNameColumn()
    {
        var map = new AccountsClassMap();
        var memberMap = map.MemberMaps.SingleOrDefault(m => m.Data.Member.Name == "FirstName");
        Assert.That(memberMap, Is.Not.Null);
        Assert.That(memberMap.Data.Names, Contains.Item("FirstName"));
    }

    [Test]
    public void Map_LastName_MapsToLastNameColumn()
    {
        var map = new AccountsClassMap();
        var memberMap = map.MemberMaps.SingleOrDefault(m => m.Data.Member.Name == "LastName");
        Assert.That(memberMap, Is.Not.Null);
        Assert.That(memberMap.Data.Names, Contains.Item("LastName"));
    }

    [Test]
    public void Map_MeterReadings_IsIgnored()
    {
        var map = new AccountsClassMap();
        var memberMap = map.MemberMaps.SingleOrDefault(m => m.Data.Member.Name == "MeterReadings");
        Assert.That(memberMap, Is.Not.Null);
        Assert.That(memberMap.Data.Names, Is.Empty);
    }
}