using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy;
    public float SpawnTime = 3f;
    public Vector2 LevelsRange = new Vector2(1, 5);
    public event Action<GameObject, int> OnSpawn;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            InvokeRepeating("Spawning", SpawnTime, SpawnTime);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            CancelInvoke("Spawning");
    }

    void Spawning()
    {
        var level = (int)UnityEngine.Random.Range(LevelsRange.x, LevelsRange.y + 1);
        var instance = Instantiate(Enemy, transform.position, Quaternion.identity);
        OnSpawn?.Invoke(instance, level);
    }
}
