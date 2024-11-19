using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void UseSkill();

    bool CanUseSkill();

    List<Upgrade> GetFollowingUprages();

    DropTables.Rarity GetRarity();

    void SetCharacter(CharacterClass character);
}
