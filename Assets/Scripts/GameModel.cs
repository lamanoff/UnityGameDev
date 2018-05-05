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
    public GameState State { get; private set; }    // Статус игры
    public PlayerModel MainPlayer { get; private set; } // Главный игрок
    public Dictionary<int, EnemyModel> Enemies = new Dictionary<int, EnemyModel>();  // <Id, Model>
    public List<GameObject> AvailableDrop = new List<GameObject>();
    private GameObject[] spawners;  // Спавнеры
    private List<int> usedIds = new List<int>(); // Занимаемые Id
    public float Experience { get; private set; }
    public float ExpToLevelUp { get; private set; }
    public float LastExpMargin { get; private set; }

    // Та ситуация, когда нужно создать класс модели игрока раншье, чем инициализируются все остальные скрипты (иначе HUD не может подписаться на его события)
    void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player"); // Ищем главного игрока (на нём висит тэг "Player")
        MainPlayer = new PlayerModel(player);                    // Создаём класс модели главного игрока
        MainPlayer.OnStateChanged += StopGame;
    }

    private void StopGame()
    {
        if (MainPlayer.State == CreatureState.Dead)
            Debug.Log("Game over!");
    }

    // Инициализируем основной класс модели игрового мира.
    private void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner"); // Ищем все спавнеры на сцене
        foreach (var spawner in spawners)
            spawner.GetComponent<Spawner>().OnSpawn += AddEnemy; // Подписываемся на событие "создания игрока" у всех спавнеров, которые мы нашли
        CheckForUpLevel();
    }

    // Когда врага убили, мы должны освободить занимаемый им Id и удалить его класс модель
    private void ClearEnemy(int id)
    {
        Experience += Enemies[id].Level;
        usedIds.Remove(id); // Освобождаем Id
        Enemies.Remove(id); // Удаляем класс модели
        CheckForUpLevel();
    }

    private void CheckForUpLevel()
    {
        if (Experience > ExpToLevelUp)
        {
            LastExpMargin = ExpToLevelUp;
            MainPlayer.LevelUp();
        }
        ExpToLevelUp = (float)(5 * Math.Pow(MainPlayer.Level, 2));
    }

    // Создаём для врага его класс модели
    private void AddEnemy(GameObject instance, int level)
    {
        var id = instance.GetInstanceID();                                  // Получаем Id
        var enemyModel = new EnemyModel(instance, id, level);               // Создаём класс модели
        enemyModel.OnDeath += ClearEnemy;                                   // Подписываемся на событие его смерти
        Enemies.Add(id, enemyModel);                                        // Сохраняем врага в словаре врагов
    }
}
