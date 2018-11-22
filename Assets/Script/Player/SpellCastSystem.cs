using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpellCastSystem : ComponentSystem
{
    private struct Data
    {
        public readonly int Length;
        public ComponentDataArray<CastSpell> spells;
        public ComponentDataArray<PlayerInput> inputs;
        public ComponentDataArray<Mana> manas;
    }
    [Inject] Data data;
    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            var cs = data.spells[i];
            var pi = data.inputs[i];
            var m = data.manas[i];
            cs.cooldown -= Time.deltaTime;
            var spellData = LanePushBootstrap.GetSpell(cs.castIndex);
            if (cs.cooldown < 0 && pi.spawn == 1 && m.cur >= spellData.spellCost)
            {
                pi.spawn = 0;
                cs.cooldown = .2f;
                m.cur -= spellData.spellCost;
                var spellUnit = spellData as Spell_Unit;
                if(spellUnit != null)
                {
                    for (int c = 0; c < spellUnit.unitCount * LanePushBootstrap.unitMultipler; c++)
                    {
                        var em = PostUpdateCommands;
                        em.CreateEntity(LanePushBootstrap.unitArchetype);
                        var pos = new float3(pi.spellX + UnityEngine.Random.insideUnitCircle.x, pi.spellY + UnityEngine.Random.insideUnitCircle.y, 0);
                        em.SetComponent(new Position { Value = pos });
                        em.SetComponent(new MoveForward { dir = new float3(1, 0, 0), speed = spellUnit.speed });
                        em.SetComponent(new Health { cur = spellUnit.health, max = spellUnit.health, isFlying =spellUnit.isFlying ? 1: 0});
                        em.SetComponent(spellUnit.GetUnitData());
                        em.SetComponent(spellUnit.GetAttackerData());
                        if (pi.faction == 1)
                            em.AddComponent(new Faction1());
                        else
                            em.AddComponent(new Faction2());
                        em.AddSharedComponent(spellUnit.renderer);
                    }
                }
                
                
            }
            data.spells[i] = cs;
            data.manas[i] = m;

        }
    }
}
