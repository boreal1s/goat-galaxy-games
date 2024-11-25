using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Dodge = 0,
}

public interface ISkill
{
    void UseSkill();

    void ResetSkill();

    bool CanUseSkill();

    List<Upgrade> GetFollowingUprages();

    DropTables.Rarity GetRarity();

    void SetCharacter(ref PlayerController character);

    SkillType GetSkillType();

    float GetSkillValue();

}
