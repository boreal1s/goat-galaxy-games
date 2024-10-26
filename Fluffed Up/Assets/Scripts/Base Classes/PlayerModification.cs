using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModification
{
    public List<Upgrade> followingMods;
    public DropTables.Rarity rarity;

    public PlayerModification(List<Upgrade> followingMods, DropTables.Rarity rarity)
    {
        this.followingMods = followingMods;
        this.rarity = rarity;
    }
}
