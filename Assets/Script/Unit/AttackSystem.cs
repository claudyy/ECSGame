using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AttackSystem : JobComponentSystem
{
    private struct Data{
        public readonly int Length;
        public ComponentDataArray<Attack> attacks;
        public ComponentDataArray<Attacker> attackers;
        public ComponentDataArray<Position> positions;
    }
    private struct TargetData
    {
        public EntityArray entities;
        public ComponentDataArray<Health> healths;
        public ComponentDataArray<Position> positions;

    }
    private struct AttackJob : IJobParallelFor
    {
        public ComponentDataArray<Attack> attacks;
        public ComponentDataArray<Attacker> attackers;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> positions;
        [NativeDisableParallelForRestriction]
        public EntityArray entities;
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Health> healths;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> targetPositions;

        public float deltaTime;
        public void Execute(int index)
        {
            var a = attacks[index];
            a.duration -= deltaTime;
            attacks[index] = a;
            if (a.duration > 0)
                return;
            var targetIndex = -1;
            if (a.targetIndex == -1)
                return;
            for (int i = 0; i < entities.Length; i++)
            {
                if (a.targetIndex != entities[i].Index)
                    continue;
                targetIndex = i;

                break;
            }
            if (targetIndex == -1)
                return;
            var dist = GetDist(positions[index].Value, targetPositions[targetIndex].Value);
            if (dist < attackers[index].attackRange +.4f)//extra save value so target doesnt always work out of range but maybe this needs to be balanced per unit
            {
                var h = healths[targetIndex];
                h.cur -= a.damage;
                healths[targetIndex] = h;
                if (h.cur > 0)
                    return;
            }

        }
        private float GetDist(float3 posA, float3 posB)
        {
            return Mathf.Sqrt(Mathf.Pow(posA.x - posB.x, 2) + Mathf.Pow(posA.y - posB.y, 2));
        }
    }
    [Inject] Data data;
    [Inject] TargetData targetData;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new AttackJob {
            deltaTime = Time.deltaTime,
            attacks = data.attacks,
            positions = data.positions,
            entities = targetData.entities,
            healths = targetData.healths,
            attackers = data.attackers,
            targetPositions = targetData.positions

        }.Schedule(data.Length, 256, inputDeps);
    }
}
