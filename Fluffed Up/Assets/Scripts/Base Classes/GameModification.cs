using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModification
{
    public List<Upgrade> followingMods;
    public DropTables.Rarity rarity;

    public GameModification(List<Upgrade> followingMods, DropTables.Rarity rarity)
    {
        this.followingMods = followingMods;
        this.rarity = rarity;
    }
}
