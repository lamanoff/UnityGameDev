using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PlayerModelTests
{
    private GameObject playerInstance;
    private PlayerController playerController;
    private PlayerModel playerModel;

    private void Construct()
    {
        playerInstance = new GameObject();
        playerController = playerInstance.AddComponent<PlayerController>();
        playerModel = new PlayerModel(playerInstance, 1);
    }

    [Test]
    public void TestGettingDamageWithinMaxLimit()
    {
        Construct();
        var oldValue = playerModel.Health + playerModel.Armor;
        var expected = oldValue / 2;
        try
        {
            playerController.TakeDamage(Vector3.zero, expected);
        }
        catch { }
        var actual = playerModel.Health + playerModel.Armor;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestGettingHealthOverMaxLimit()
    {
        Construct();  
        try
        {
            playerController.TakeHealth(playerModel.MaxHealth * 2);
        }
        catch { }
        var expected = playerModel.MaxHealth;
        var actual = playerModel.Health;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestGettingHealthWithinMaxLimit()
    {
        Construct();
        var oldValue = playerModel.Health / 2;
        try
        {
            playerController.TakeDamage(Vector3.zero, oldValue); 
        }
        catch { }
        playerController.TakeHealth(1);
        var expected = oldValue + 1;
        var actual = playerModel.Health;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestGettingArmorWithinMaxLimit()
    {
        Construct();
        try
        {
            playerController.TakeArmor(playerModel.MaxArmor / 2);
        }
        catch { }
        var expected = playerModel.MaxArmor / 2;
        var actual = playerModel.Armor;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestLevelUp()
    {
        Construct();
        var oldValue = playerModel.Level;
        playerModel.LevelUp();
        var expected = oldValue + 1;
        var actual = playerModel.Level;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestKeepingInstance()
    {
        Construct();
        var expected = playerInstance;
        var actual = playerModel.CreatureInstance;
        Assert.AreEqual(expected, actual);
    }

    [TearDown]
    public void AfterEveryTest() => Construct();
}
