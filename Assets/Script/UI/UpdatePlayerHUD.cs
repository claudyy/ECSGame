using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[AlwaysUpdateSystem]
public class UpdatePlayerHUD : ComponentSystem
{
    private UIManager ui_manager;
    public void SetupSceneReference()
    {
        ui_manager = GameObject.FindObjectOfType<UIManager>();
    }
    public struct PlayerData
    {
        public readonly int Length;
        public EntityArray Entity;
        public ComponentDataArray<PlayerInput> Input;
        public ComponentDataArray<Mana> mana;
    }
    [Inject]
    PlayerData data;
    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            ui_manager.UpdateMana(data.mana[i],i == 0);
        }
    }
}
