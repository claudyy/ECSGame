using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FindTargetSystem : JobComponentSystem
{
    private struct UnitDataTeam1
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<MoveForward> moves;
        public ComponentDataArray<Faction1> factions;
        public ComponentDataArray<Attacker> attackers;
        public SubtractiveComponent<Town> unit;
        public ComponentDataArray<Health> health;

    }
    private struct TownDataTeam1
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Faction1> factions;
        public ComponentDataArray<Town> towns;
        public SubtractiveComponent<Unit> unit;

    }
    private struct UnitDataTeam2
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<Unit> units;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<MoveForward> moves;
        public ComponentDataArray<Faction2> factions;
        public SubtractiveComponent<Town> unit;
        public ComponentDataArray<Attacker> attackers;
        public ComponentDataArray<Health> health;

    }
    private struct TownDataTeam2
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Faction2> factions;
        public ComponentDataArray<Town> towns;
        public SubtractiveComponent<Unit> unit;

    }
    private struct TargetJob : IJobParallelFor
    {
        public int targetLength;
        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public EntityArray targetEntities;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Position> targetPositions;
        public ComponentDataArray<Unit> units;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Health> targetHealths;
        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public ComponentDataArray<Position> unitPositions;
        [NativeDisableParallelForRestriction]
        public ComponentDataArray<MoveForward> Umoves;
        public ComponentDataArray<Attacker> attackers;


        [ReadOnly]
        public int townLength;
        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public EntityArray townTargets;
        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public ComponentDataArray<Position> townPositions;

        public void Execute(int index)
        {
            var unit = units[index];
            
            if (units[index].targetIndex != -1)
            {
                var targetIndex = -1;
                var targetIsTown = 0;
                for (int i = 0; i < targetEntities.Length; i++)
                {

                    if (unit.targetIndex != targetEntities[i].Index)
                        continue;
                    targetIndex = i;

                    break;
                }
                for (int i = 0; i < townTargets.Length; i++)
                {
                    if (unit.targetIndex == townTargets[i].Index)
                        targetIsTown = 1;

                }
                if (targetIndex == -1)
                {
                    var u = units[index];
                    u.targetIndex = -1;
                    units[index] = u;
                    return;
                }
                if(targetIsTown == 0)
                    return;
            }

            for (int u = 0; u < targetLength; u++)
            {
                if (targetHealths[u].isFlying == 1 && attackers[index].canAttackFlying == 0)
                    continue;
                var dist = GetDist(unitPositions[index].Value, targetPositions[u].Value);
                if(dist < unit.agroRange )
                {
                    unit.targetIndex = targetEntities[u].Index;
                    units[index] = unit;
                    Umoves[index] = new MoveForward { dir = GetDir(unitPositions[index].Value, targetPositions[u].Value),speed= Umoves[index].speed };
                    return;
                }
            }

            for (int t = 0; t < townLength; t++)
            {
                //make he check what town to target when you have multitples targets
                var dir = GetDir(unitPositions[index].Value, townPositions[t].Value);
                if(math.abs(unitPositions[index].Value.x- townPositions[t].Value.x) > 2)
                    dir.y = 0;
                Umoves[index] = new MoveForward { dir =  dir, speed = Umoves[index].speed };
                unit.targetIndex = townTargets[t].Index;
                units[index] = unit;
                return;
            }
        }

        private float GetDist(float3 posA,float3 posB)
        {
            return Mathf.Sqrt(Mathf.Pow(posA.x- posB.x, 2) + Mathf.Pow(posA.y - posB.y, 2));
        }
        private float Magnitude(float3 v)
        {
            return Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2));

        }
        private float3 GetDir(float3 posA, float3 posB)
        {
            var dir = posB - posA;
            return dir / Magnitude(dir);
        }
    }
    [Inject] UnitDataTeam1 team1Unit;
    [Inject] TownDataTeam1 team1Town;
    [Inject] UnitDataTeam2 team2Unit;
    [Inject] TownDataTeam2 team2Town;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var team1 = new TargetJob {
            targetEntities = team2Unit.entities,
            targetLength = team2Unit.Length,
            targetPositions = team2Unit.positions,
            units = team1Unit.units,
            unitPositions = team1Unit.positions,
            Umoves = team1Unit.moves,
            townLength = team2Town.Length,
            townTargets = team2Town.entities,
            townPositions = team2Town.positions,
            targetHealths = team2Unit.health,
            attackers = team1Unit.attackers
        }.Schedule(team1Unit.Length, 256, inputDeps);
        var team2 = new TargetJob
        {
            targetEntities = team1Unit.entities,
            targetLength = team1Unit.Length,
            targetPositions = team1Unit.positions,
            units = team2Unit.units,
            unitPositions = team2Unit.positions,
            Umoves = team2Unit.moves,
            townLength = team1Town.Length,
            townTargets = team1Town.entities,
            townPositions = team1Town.positions,
            targetHealths = team1Unit.health,
            attackers = team2Unit.attackers
        }.Schedule(team2Unit.Length, 256, team1);
        return team2;
    }
}
