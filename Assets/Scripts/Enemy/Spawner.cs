using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy;
    public float SpawnTime = 3f;
    public Vector2 LevelsRange = new Vector2(1, 5);
    public event Action<GameObject, int> OnSpawn;

    void Start()
    {
        InvokeRepeating("Spawning", SpawnTime, SpawnTime);
    }

    void Spawning()
    {
        var level = (int)UnityEngine.Random.Range(LevelsRange.x, LevelsRange.y + 1);
        var instance = Instantiate(Enemy, transform.position, Quaternion.identity);
        OnSpawn?.Invoke(instance, level);
    }
}
