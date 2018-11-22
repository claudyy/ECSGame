using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



public struct PlayerInput : IComponentData
{
    public float spellX;
    public float spellY;
    public int spawn;
    public int spellIndex;
    public int faction;
}
public struct Mana : IComponentData
{
    public int cur;
    public int max;
    public float lastUpdate;


}
[System.Serializable]
public struct CastSpell : IComponentData
{
    public float cooldown;
    public int unitCount;
    public int spellCost;
    public int castIndex;
}
public enum TargetType
{
    unit = 0,
    town = 1
}
public struct Unit : IComponentData
{
    public int targetIndex;
    public int targetType;
    public int isFlying;
    public int isRanged;
    public float agroRange;
}
public struct Tower : IComponentData
{
}
public struct Town : IComponentData
{
}
public struct Faction1 : IComponentData
{
}
public struct Faction2: IComponentData
{
}
public struct MoveForward : IComponentData
{
    public float3 dir;
    public float speed;
}
public struct Health : IComponentData
{
    public int cur;
    public int max;
    public int isFlying;
}
public struct Attack : IComponentData
{
    public float startTime;
    public float duration;
    public int targetIndex;
    public int damage;
}
public struct Attacker : IComponentData
{
    public int damage;
    public float damageRadius;
    public float attackRate;
    public float attackRange;
    public int canAttackFlying;
}