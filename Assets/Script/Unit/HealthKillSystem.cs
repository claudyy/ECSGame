using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

public class HealthKillBarrier : BarrierSystem
{

}
public class HealthKillSystem : JobComponentSystem
{
    private struct KillJob : IJobProcessComponentDataWithEntity<Health>
    {
        public EntityCommandBuffer.Concurrent buffer;
        public void Execute(Entity entity, int index, ref Health data)
        {
            if (data.cur > 0)
                return;
            buffer.DestroyEntity(0,entity);
        }
    }
    [Inject] HealthKillBarrier barrier;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new KillJob {buffer = barrier.CreateCommandBuffer().ToConcurrent()}.Schedule(this,inputDeps);
    }
}
