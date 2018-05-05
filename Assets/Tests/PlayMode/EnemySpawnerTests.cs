using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;

public class EnemySpawnerTests : MonoBehaviour
{
    [UnityTest]
    public IEnumerator EnemySpawnerTestCorrectObjectTrigger_CorrectTrigger()
    {
        
        var triggerPrefab = Resources.Load(@"Prefabs\Player") as GameObject;
        var spawnerPrefab = Resources.Load(@"Prefabs\Spawner") as GameObject;
        
        var enemyPrefab = Resources.Load(@"Prefabs\MeleeEnemy") as GameObject;
        var spawnerInstance = Instantiate(spawnerPrefab);
        var spawnerController = spawnerInstance.GetComponent<Spawner>();
        spawnerController.Enemy = spawnerPrefab;
        spawnerController.LevelsRange = new Vector2(3, 3);
        spawnerController.SpawnTime = 0;
        spawnerInstance.GetComponent<BoxCollider>().transform.localScale = new Vector3(5, 5, 5);
        var triggerInstance = Instantiate(triggerPrefab);

        triggerInstance.transform.position = spawnerInstance.transform.position;
        var gameModel = new GameObject("GameModel").AddComponent<GameModel>();

        yield return null;

        var actual = GameObject.FindGameObjectsWithTag(enemyPrefab.tag).Length;
        Debug.Log(actual);
        Assert.AreEqual(1, actual);
    }
}
