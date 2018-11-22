using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LanePushBootstrap
{
    public static EntityArchetype playerArchetype;
    public static LanePushSetting settings;
    public static EntityArchetype unitArchetype;
    public static EntityArchetype towerArchetype;
    public static EntityArchetype TowerArchetype;
    public const int unitMultipler = 3;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Debug.Log("Init before Scene");
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        // Create player archetype
        playerArchetype = entityManager.CreateArchetype(
            typeof(PlayerInput),
            typeof(Mana),
            typeof(CastSpell));
        unitArchetype = entityManager.CreateArchetype(
            typeof(Unit),
            typeof(MoveForward),
            typeof(Position),
            typeof(Health),
            typeof(Attacker));
        towerArchetype = entityManager.CreateArchetype(
            typeof(Position),
            typeof(Rotation),
            typeof(Scale),
            typeof(Town),
            typeof(Health));
    }
    static List<BaseSpell> spellDatas;
    internal static BaseSpell GetSpell(int castIndex)
    {
        if (spellDatas == null)
            spellDatas = Resources.LoadAll<BaseSpell>("").ToList();
        return spellDatas[castIndex];
    }
    public static int GetSpellIndex(BaseSpell spell)
    {
        if (spellDatas == null)
            spellDatas = Resources.LoadAll<BaseSpell>("").ToList();
        return spellDatas.IndexOf(spell);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitAfterLoad()
    {
        Debug.Log("Init after Scene");
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeWithScene();
    }
    public static void InitializeWithScene()
    {
        Debug.Log("Init with scene");

        World.Active.GetOrCreateManager<UpdatePlayerHUD>().SetupSceneReference();
        World.Active.GetOrCreateManager<PlayerInputSystem>().SetupSceneReference();

        NewGame();

    }
    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }
    public static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity player = entityManager.CreateEntity(playerArchetype);
        Entity player2 = entityManager.CreateEntity(playerArchetype);

        entityManager.SetComponentData(player, new Mana { max = 10 });
        entityManager.SetComponentData(player2, new Mana { max = 10 });

        Entity towerPlayer = entityManager.CreateEntity(towerArchetype);
        Entity towerPlayer2 = entityManager.CreateEntity(towerArchetype);

        entityManager.AddSharedComponentData(towerPlayer, Bootstrap_Tower.unitRenderer);
        entityManager.SetComponentData(towerPlayer, new Position { Value = new float3(-8.0f, 0.0f, 0.0f) });
        entityManager.SetComponentData(towerPlayer, new Rotation { Value = quaternion.LookRotation(new float3(0.0f, -1.0f, 0.0f), new float3(0.0f, 0.0f, -1.0f)) });
        entityManager.SetComponentData(towerPlayer, new Scale { Value = new float3(0.1f, 0.1f, 0.1f) });
        entityManager.SetComponentData(towerPlayer, new Health { cur = 100,max = 100 });
        entityManager.AddComponent(towerPlayer, typeof(Faction1));

        entityManager.AddSharedComponentData(towerPlayer2, Bootstrap_Tower.unitRenderer);
        entityManager.SetComponentData(towerPlayer2, new Position { Value = new float3(8.0f, 0.0f, 0.0f) });
        entityManager.SetComponentData(towerPlayer2, new Rotation { Value = quaternion.LookRotation(new float3(0.0f, -1.0f, 0.0f), new float3(0.0f, 0.0f, -1.0f)) });
        entityManager.SetComponentData(towerPlayer2, new Scale { Value = new float3(0.1f, 0.1f, 0.1f) });
        entityManager.SetComponentData(towerPlayer2, new Health { cur = 100, max = 100 });
        entityManager.AddComponent(towerPlayer2,typeof(Faction2));

    }
}
