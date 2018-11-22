using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FindAttackTargetSystem : JobComponentSystem
{
    private struct FindAttackJob : IJobParallelFor
    {
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Attacker> attackers;
        //public ComponentDataArray<MoveForward> moveForward;
        public EntityArray entities;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public EntityArray targetEntities;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> targetPositions;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Health> targetHealts;
        [ReadOnly]
        public EntityCommandBuffer.Concurrent commandBuffer;


        public float time;
        public int jobIndex;
        public void Execute(int index)
        {
            var a = attackers[index];

            var targetIndex = -1;

            for (int t = 0; t < targetEntities.Length; t++)
            {
                if (targetHealts[t].isFlying == 1 && a.canAttackFlying == 0)
                    continue;
                var dist = GetDist(positions[index].Value, targetPositions[t].Value);
                if (dist < a.attackRange)
                {
                    targetIndex = targetEntities[t].Index;
                    break;
                }
            }
            if (targetIndex == -1)
                return;
            commandBuffer.AddComponent(jobIndex, entities[index], new Attack {duration = a.attackRate, damage = a.damage,targetIndex = targetIndex});
        }

        private float GetDist(float3 posA, float3 posB)
        {
            return Mathf.Sqrt(Mathf.Pow(posA.x - posB.x, 2) + Mathf.Pow(posA.y - posB.y, 2));
        }
    }
    private struct Data1
    {
        public readonly int Length;
        public EntityArray entityArray;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Attacker> attackers;
        public ComponentDataArray<Faction1> faction;
        public SubtractiveComponent<Attack> attacks;
    }
    private struct Data2
    {
        public readonly int Length;
        public EntityArray entityArray;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Attacker> attackers;
        public ComponentDataArray<Faction2> faction;
        public SubtractiveComponent<Attack> attacks;
    }
    private struct TargetsData1
    {
        public readonly int Length;
        public EntityArray entityArray;
        public ComponentDataArray<Health> healths;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Faction1> faction;

    }
    private struct TargetsData2
    {
        public readonly int Length;
        public EntityArray entityArray;
        public ComponentDataArray<Health> healths;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Faction2> faction;

    }
    [Inject] Data1 data1;
    [Inject] TargetsData1 targetData1;
    [Inject] Data2 data2;
    [Inject] TargetsData2 targetData2;
    [Inject] FindAttackTargetBarrier barrier;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job= new FindAttackJob {
            positions = data1.positions,
            entities = data1.entityArray,
            attackers = data1.attackers,
            targetEntities = targetData2.entityArray,
            targetPositions = targetData2.positions,
            targetHealts = targetData2.healths,
            time =Time.time,
            jobIndex = 0,
            commandBuffer = barrier.CreateCommandBuffer().ToConcurrent()

        }.Schedule(data1.Length,256,inputDeps);
        return new FindAttackJob
        {
            positions = data2.positions,
            entities = data2.entityArray,
            attackers = data2.attackers,
            targetEntities = targetData1.entityArray,
            targetPositions = targetData1.positions,
            targetHealts = targetData1.healths,
            time = Time.time,
            jobIndex = 0,
            commandBuffer = barrier.CreateCommandBuffer().ToConcurrent()

        }.Schedule(data2.Length, 256, job);
    }

}
public class FindAttackTargetBarrier : BarrierSystem
{

}

