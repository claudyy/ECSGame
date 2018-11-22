using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
public class UI_Player : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Image manaDisplay;
    public TextMeshProUGUI manaText;
    public List<UI_Spell> spellDisplay;
    Action<int> onSelect;
    public bool playerTeam;
    BaseInput baseInput;
    public bool over;
    internal void UpdateMana(Mana mana)
    {
        manaDisplay.fillAmount = (float)mana.cur / mana.max;
        manaText.text = mana.cur + " / " + mana.max;
    }

    internal void SetInput(BaseInput baseInput)
    {
        this.baseInput = baseInput;
        UpdateSpells();
        if(baseInput is ScenePlayerInput)
        {
            onSelect += baseInput.SelectSpell;
        }
        for (int i = 0; i < spellDisplay.Count; i++)
        {
            spellDisplay[i].onClick += Select;
            spellDisplay[i].UpdateSpell(baseInput.curSpells[i], baseInput.selectedSpell == i);
        }
    }
    public void UpdateSpells()
    {
        for (int i = 0; i < spellDisplay.Count; i++)
        {
            spellDisplay[i].UpdateSpell(baseInput.curSpells[i], baseInput.selectedSpell == i);
        }
    }
    public void Select(UI_Spell button)
    {
        var index = spellDisplay.IndexOf(button);
        if (onSelect != null)
            onSelect(index);
        UpdateSpells();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }
}
