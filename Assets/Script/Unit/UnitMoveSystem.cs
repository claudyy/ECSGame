using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public class UnitMoveBarrier : BarrierSystem
{

}
public class UnitMoveUpdate : JobComponentSystem
{
    private struct DataTeam1
    {
        public readonly int Length;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<MoveForward> moves;
        public ComponentDataArray<Faction1> fs;
    }
    private struct DataTeam2
    {
        public readonly int Length;
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<MoveForward> moves;
        public ComponentDataArray<Faction1> fs;
    }
    private struct DataTarget
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<Position> positions;

    }
    private struct UnitMoveJob : IJobParallelFor
    {
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<MoveForward> moves;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> unitPositions;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public EntityArray targetEntities;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> targetPositions;
        public void Execute(int index)
        {
            if(units[index].targetIndex != -1)
            {
                var targetIndex = -1;
                for (int i = 0; i < targetEntities.Length; i++)
                {
                    if (units[index].targetIndex != targetEntities[i].Index)
                        continue;
                    targetIndex = i;
                    break;
                }
                if (targetIndex != -1)
                {
                    moves[index] =new MoveForward {dir = math.normalize(targetPositions[targetIndex].Value - unitPositions[index].Value), speed = moves[index].speed };
                }
            }

        }
    }
    [Inject] DataTeam1 dataTeam1;
    [Inject] DataTeam2 dataTeam2;
    [Inject] DataTarget dataTarget;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var team1 = new UnitMoveJob {
            units = dataTeam1.units,
            moves = dataTeam1.moves,
            unitPositions = dataTeam1.positions,
            targetEntities = dataTarget.entities,
            targetPositions = dataTarget.positions
        }.Schedule(dataTeam1.Length,256,inputDeps);

        return  new UnitMoveJob
        {
            units = dataTeam1.units,
            moves = dataTeam1.moves,
            unitPositions = dataTeam1.positions,
            targetEntities = dataTarget.entities,
            targetPositions = dataTarget.positions
        }.Schedule(dataTeam1.Length, 256, team1);
    }
}
