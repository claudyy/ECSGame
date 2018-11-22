using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class Spell_Unit : BaseSpell
{
    public int unitCount =6;
    public int health =4;
    public float speed = 1;
    public int damage = 1;
    public float damageRadius =0;
    public float attackRate = 1;
    public float attackRadius = 0.5f;
    public float agroRadius = 1;
    public bool isFlying;
    public bool isRanged;
    public MeshInstanceRenderer renderer;
    [TextArea]
    public string devNote;
    public override CastSpell CreateSpell()
    {
        var spell = base.CreateSpell();

        spell.unitCount = unitCount;

        return spell;
    }

    public Unit GetUnitData()
    {
        return new Unit {
            agroRange = agroRadius,
            targetIndex = -1
        };
    }
    public Attacker GetAttackerData()
    {
        return new Attacker
        {
            attackRange = attackRadius,
            damage = damage,
            damageRadius = damageRadius,
            attackRate = attackRate,
            canAttackFlying = isRanged ? 1:0
        };
}
}
