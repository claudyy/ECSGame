using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem
{
    BaseInput[] player;
    public void SetupSceneReference()
    {
        player = GameObject.FindObjectsOfType<BaseInput>();
    }
    private struct Data {
        public readonly int Length;
        public ComponentDataArray<PlayerInput> players;
        public ComponentDataArray<CastSpell> casts;
        public ComponentDataArray<Mana> mana;
    }
    [Inject] Data playerData;
    protected override void OnUpdate()
    {
        for (int i = 0; i < playerData.Length; i++)
        {
            PlayerInput pi;
            pi.spellX = player[i].spellPos.x;
            pi.spellY = player[i].spellPos.y;
            pi.spawn = player[i].spawn && player[i].selectedSpell != -1 ? 1 : 0;
            pi.spellIndex = player[i].selectedSpell;
            pi.faction = player[i].faction;
            CastSpell cast;
            if(pi.spellIndex != -1)
            {
                var spell = player[i].curSpells[pi.spellIndex];
                cast = spell.CreateSpell();
                if (pi.spawn == 1 && playerData.mana[i].cur >= spell.spellCost)
                {
                    player[i].ShuffleSpell(pi.spellIndex);
                    player[i].selectedSpell = -1;
                    player[i].ui.UpdateSpells();
                }
            }
            else
            {
                cast = new CastSpell();
            }
            
            player[i].spawn = false;
            playerData.players[i] = pi;
            playerData.casts[i] = cast;
        }
    }
}
