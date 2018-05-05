using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float MaxHealth { get; protected set; }
    public CreatureState State { get; protected set; }
    public int Level { get; protected set; }
    private float health;
    public float Health
    {
        get => health;
        protected set
        {
            if (value <= 0)
            {
                health = 0;
                State = CreatureState.Dead;
                Dead();
            }
            else if (value > MaxHealth)
                health = MaxHealth;
            else
                health = value;
        }
    }
    public GameObject CreatureInstance { get; protected set; }
    public Vector3 Position
    {
        get
        {
            if (CreatureInstance == null)
                throw new NullReferenceException("Creature is not instantiated");
            return CreatureInstance.transform.position;
        }
    }

    protected virtual void Dead()
        => Destroy(CreatureInstance);

    public Creature(GameObject creatureInstance, int level = 1)
    {
        CreatureInstance = creatureInstance ?? throw new NullReferenceException("Creature is not instantiated");
        State = CreatureState.Alive;
        MaxHealth = level * 50;
        Health = MaxHealth;
        Level = level;
    }
}
