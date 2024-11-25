using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModification
{
    public List<Upgrade> followingMods;
    public DropTables.Rarity rarity;
    public float modValue;
    public float modChance;

    public PlayerModification(List<Upgrade> followingMods, DropTables.Rarity rarity, float modValue = 1f, float modChance = 0)
    {
        this.followingMods = followingMods;
        this.rarity = rarity;
        this.modValue = modValue;
        this.modChance = modChance;
    }
}
