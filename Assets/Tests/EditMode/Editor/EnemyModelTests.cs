using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EnemyModelTests
{
    private GameObject enemyInstance;
    private EnemyController enemyController;
    private EnemyModel enemyModel;

    private void Construct()
    {
        enemyInstance = new GameObject();
        enemyController = enemyInstance.AddComponent<EnemyController>();
        try
        {
            enemyModel = new EnemyModel(enemyInstance, 1, 1);
        }
        catch { }
    }

    [Test]
    public void TestGettingDamageWithinMaxLimit()
    {
        Construct();
        var oldValue = enemyModel.Health;
        var expected = oldValue / 2;
        try
        {
            enemyController.TakeDamage(expected);
        }
        catch { }
        var actual = enemyModel.Health;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestKeepingInstance()
    {
        Construct();
        var expected = enemyInstance;
        var actual = enemyModel.CreatureInstance;
        Assert.AreEqual(expected, actual);
    }

    [TearDown]
    public void AfterEveryTest() => Construct();
}
