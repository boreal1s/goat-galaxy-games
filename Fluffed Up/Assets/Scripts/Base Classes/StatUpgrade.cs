using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Health,
    Defense,
    AttackSpeed,
    AttackPower,
    MoveSpeed,
}

public class StatUpgrade
{
    public float statValue;
    public StatType statType;

    public StatUpgrade(float statValue, StatType statType)
    {
        this.statValue = statValue;
        this.statType = statType;
    }
}
