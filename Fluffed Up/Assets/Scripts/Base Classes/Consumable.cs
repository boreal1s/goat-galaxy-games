using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    Health,
    Defense,
    AttackSpeed,
    Damage,
    MoveSpeed,
}

public class Consumable
{
    public float power;
    public ConsumableType consumableType;

    public Consumable(float power, ConsumableType consumableType)
    {
        this.power = power;
        this.consumableType = consumableType;
    }
}
