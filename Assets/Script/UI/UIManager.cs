using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UI_Player player;
    public UI_Player enemy;
    public void UpdateMana(Mana mana,bool p)
    {
        if (p)
            player.UpdateMana(mana);
        else
            enemy.UpdateMana(mana);
    }


}
