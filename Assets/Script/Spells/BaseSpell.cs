using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Spell")]
public class BaseSpell : ScriptableObject
{
    public int spellCost = 1;
    public Sprite UiSprite;

    public virtual CastSpell CreateSpell()
    {
        return new CastSpell { castIndex = LanePushBootstrap.GetSpellIndex(this)};
    }
}
