using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UpgradeFlags : MonoBehaviour
{
    private Dictionary<string, bool> upgrades = new Dictionary<string, bool>()
    {
        { "Flesh Wound", false },
        { "Restock", false },
    };

    public void setUpgradeFlag(string flag, bool val)
    {
        upgrades[flag] = val;
    }

    public bool getUpgradeFlag(string flag)
    {
        return upgrades[flag];
    }
}
