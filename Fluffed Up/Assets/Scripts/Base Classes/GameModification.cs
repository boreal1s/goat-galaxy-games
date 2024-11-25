using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModification
{
    public List<Upgrade> followingMods;
    public DropTables.Rarity rarity;
    public float modValue;

    public GameModification(List<Upgrade> followingMods, DropTables.Rarity rarity, float modValue = 1f)
    {
        this.followingMods = followingMods;
        this.rarity = rarity;
        this.modValue = modValue;
    }
}
