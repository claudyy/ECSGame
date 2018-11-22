using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class AttackCleanSystem : JobComponentSystem
{
    private struct Data
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<Attack> attacks;
    }
    private struct AttackJob : IJobParallelFor
    {
        public EntityCommandBuffer.Concurrent commandBuffer;
        public EntityArray entities;
        public ComponentDataArray<Attack> attacks;

        public void Execute(int index)
        {
            if (attacks[index].duration > 0)
                return;
            commandBuffer.RemoveComponent(0, entities[index], typeof(Attack));
        }
    }
    [Inject] Data data;
    [Inject] AttackCleanBarrier barrier;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new AttackJob {
            commandBuffer = barrier.CreateCommandBuffer().ToConcurrent(),
            entities = data.entities,
            attacks = data.attacks
        }
        .Schedule(data.Length,256,inputDeps);
    }
}
public class AttackCleanBarrier : BarrierSystem
{

}