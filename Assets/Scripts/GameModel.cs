using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Level1,
    Running,
    Paused
}

public enum CreatureState
{
    Alive,
    Dead
}

public class GameModel : MonoBehaviour
{
    public GameState State { get; private set; }
    public PlayerModel MainPlayer { get; private set; }
    public Dictionary<int, EnemyModel> Enemies = new Dictionary<int, EnemyModel>();
    public List<GameObject> AvailableDrop = new List<GameObject>();
    private GameObject[] spawners;
    private List<int> usedIds = new List<int>();
    public float Experience { get; private set; }
    public float ExpToLevelUp { get; private set; }
    public float LastExpMargin { get; private set; }

    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        MainPlayer = new PlayerModel(player);
        MainPlayer.OnStateChanged += StopGame;

        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (var spawner in spawners)
            spawner.GetComponent<Spawner>().OnSpawn += AddEnemy;
        CheckForUpLevel();
    }

    private void StopGame()
    {
        if (MainPlayer.State == CreatureState.Dead)
            Debug.Log("Game over!");
    }

    private void ClearEnemy(int id)
    {
        Experience += Enemies[id].Level;
        usedIds.Remove(id);
        Enemies.Remove(id);
        CheckForUpLevel();
    }

    private void CheckForUpLevel()
    {
        if (Experience > ExpToLevelUp)
        {
            LastExpMargin = ExpToLevelUp;
            MainPlayer.LevelUp();
            UpSpawnersLevel();
        }
        ExpToLevelUp = (float)(5 * Math.Pow(MainPlayer.Level, 2));
    }

    private void UpSpawnersLevel()
    {
        foreach(var obj in spawners)
        {
            var spawner = obj.GetComponent<Spawner>();
            spawner.LevelsRange += new Vector2(1, 1);
        }
    }

    private void AddEnemy(GameObject instance, int level)
    {
        var id = instance.GetInstanceID();
        var enemyModel = new EnemyModel(instance, id, level);
        enemyModel.OnDeath += ClearEnemy;
        Enemies.Add(id, enemyModel);
    }
}
