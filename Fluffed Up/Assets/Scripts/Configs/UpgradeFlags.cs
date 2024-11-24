using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UpgradeFlags : MonoBehaviour
{
    private Dictionary<string, Upgrade> upgrades = new Dictionary<string, Upgrade>()
    {
        { "Flesh Wound", null },
        { "Restock", null },
    };

    public void setUpgradeFlag(string flag, Upgrade upgrade)
    {
        upgrades[flag] = upgrade;
    }

    public Upgrade getUpgradeFlag(string flag)
    {
        return upgrades[flag];
    }
}
