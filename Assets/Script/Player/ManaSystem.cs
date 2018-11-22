using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class ManaSystem : JobComponentSystem
{
    private ComponentGroup componentGroup;
    protected override void OnCreateManager()
    {
        componentGroup = GetComponentGroup(ComponentType.Create<Mana>());
        base.OnCreateManager();
    }
    private struct ManaJob : IJobProcessComponentData<Mana>
    {
        public float curTime;
        public void Execute(ref Mana data)
        {
            if (curTime -data.lastUpdate > 1 && data.cur < data.max)
            {
                data.cur++;
                data.lastUpdate = curTime;
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new ManaJob {curTime =Time.time }.Schedule(this,inputDeps);
    }
}
