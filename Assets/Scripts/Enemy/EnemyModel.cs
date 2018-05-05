using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyModel : Creature
{
    public int Id { get; private set; }
    public event Action<int> OnDeath;
    private Image healthBar;
    private Canvas enemyCanvas;
    private GameModel gameModel;

    protected override void Dead()      // Переписываем метод смерти базового класса Creature, 
    {                                   // т.к. нам нужно вызвать событие OnDeath, чтобы удалить класс модели этого врага
        try
        {
            Drop();
            OnDeath?.Invoke(Id);
        }
        finally
        {
            base.Dead();
        }
    }

    private void Drop()
    {
        if (!gameModel.AvailableDrop.Any())
            return;
        var random = UnityEngine.Random.Range(0, Level + 1);
        if (random < Level - Level / 4)
        {
            var dropIndex = UnityEngine.Random.Range(0, gameModel.AvailableDrop.Count);
            var dropItem = gameModel.AvailableDrop[dropIndex];
            dropItem.GetComponent<Drop>().Points = Level * 5;
            var instance = Instantiate(dropItem, CreatureInstance.transform, false);
            instance.transform.SetParent(CreatureInstance.transform.parent, true);
            instance.transform.localScale = dropItem.transform.localScale;
        }
    }

    public EnemyModel(GameObject enemyInstance, int id, int level) 
        : base(enemyInstance, level)
    {
        Id = id;
        var enemyController = CreatureInstance.GetComponent<EnemyController>();     // Находим класс контроллер врага
        enemyController.GetDamage += GetDamage;
        enemyController.Damage = level * 5;
        healthBar = CreatureInstance.GetComponentInChildren<Image>();
        enemyCanvas = CreatureInstance.GetComponentInChildren<Canvas>();
        gameModel = GameObject.Find("GameModel").GetComponent<GameModel>();
        enemyCanvas.GetComponentInChildren<Text>().text = $"Lvl.{Level}";
    }

    private void GetDamage(float damage)
    {
        if (State == CreatureState.Dead)
            throw new Exception("Creature is already dead!");
        Health -= damage;
        if (Health < MaxHealth)
        {
            if (enemyCanvas.enabled == false)
                enemyCanvas.enabled = true;
            healthBar.fillAmount = Health / MaxHealth;
        }
    }
}