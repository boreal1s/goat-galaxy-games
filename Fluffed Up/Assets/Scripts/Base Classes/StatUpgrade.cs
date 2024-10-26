using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Health,
    Defense,
    AttackSpeed,
    Damage,
    MoveSpeed,
}

public class StatUpgrade
{
    float statValue;
    StatType statType;

    public StatUpgrade(float statValue, StatType statType)
    {
        this.statValue = statValue;
        this.statType = statType;
    }
}
