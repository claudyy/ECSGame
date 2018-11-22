using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInput : MonoBehaviour
{
    public Vector2 spellPos;
    public int selectedSpell;
    public bool spawn;
    public Transform spellTarget;

    public List<BaseSpell> curSpells;
    public List<BaseSpell> spells;
    public int faction;
    public UI_Player ui;

    protected virtual void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            curSpells.Add(GetSpell());
        }
        ui.SetInput(this);
    }

    protected BaseSpell GetSpell()
    {
        return spells[Random.Range(0, spells.Count)];
    }

    protected virtual void Update()
    {
        spellPos = spellTarget.position;
    }
    public void SelectSpell(int i)
    {
        selectedSpell = i;
    }

    public void ShuffleSpell(int spellIndex)
    {
        curSpells[spellIndex] = GetSpell();
        ui.UpdateSpells();
    }
}
