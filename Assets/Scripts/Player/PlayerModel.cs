using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerModel : Creature
{
    public event Action OnStateChanged;
    public float MaxArmor { get; private set; }
    private float armor;
    private PlayerController playerController;

    public float flashSpeed = 5f;                              
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    bool damaged;
    public Image damageImage;

    public float Armor
    {
        get => armor;
        private set
        {
            OnStateChanged?.Invoke();
            if (value < 0)
                armor = 0;
            else if (value > MaxArmor)
                armor = MaxArmor;
            else
                armor = value;
        }
    }

    public PlayerModel(GameObject playerInstance, int level = 1)
        : base(playerInstance, level)
    {
        MaxArmor = MaxHealth / 4 * level;
        Armor = 0;
        playerController = CreatureInstance.GetComponent<PlayerController>();
        playerController.GetDamage += GetDamage;
        playerController.GetHealth += GetHealth;
        playerController.GetArmor += GetArmor;
    }

    void Update()
    {
        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }

    public void LevelUp()
    {
        Level++;
        MaxArmor = MaxHealth / 4 * Level;
        MaxHealth = Level * 50;
        playerController.Regeneration = MaxHealth / 180;
        playerController.UpLevel();
        OnStateChanged?.Invoke();
    }

    private void GetDamage(float damage)
    {
        if (State == CreatureState.Dead)
        {
            //throw new Exception("Player is already dead!");
        }
        damaged = true;
        var residualDamage = damage - Armor;
        Armor -= damage;
        if (residualDamage < 0)
            residualDamage = 0;
        Health -= residualDamage;
        OnStateChanged?.Invoke();
    }

    private void GetHealth(float health)
    {
        if (State == CreatureState.Dead)
        Health += health;
        OnStateChanged?.Invoke();
    }

    private void GetArmor(float armor)
    {
        if (State == CreatureState.Dead)
            throw new Exception("Player is dead!");
        Armor += armor;
        OnStateChanged?.Invoke();
    }
}
