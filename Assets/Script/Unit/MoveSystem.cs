using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class MoveSystem : JobComponentSystem
{
    private struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<MoveForward> moves;
        public SubtractiveComponent<Attack> subtractive;
    }
    private struct MoveJob : IJobParallelFor
    {
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<MoveForward> moves;
        public float deltaTime;
        public void Execute(int index)
        {
            var pos = positions[index].Value;
            pos += moves[index].dir * deltaTime *moves[index].speed;
            positions[index] = new Position{Value = pos};
        }
    }
    [Inject]Data data;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new MoveJob { deltaTime=Time.deltaTime, units  = data.units ,positions = data.positions,moves = data.moves}.Schedule(data.Length,256,inputDeps);
    }
}
