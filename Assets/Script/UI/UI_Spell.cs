using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UI_Spell : MonoBehaviour, IPointerClickHandler
{
    public Action<UI_Spell> onClick;
    public Image highlight;
    public Image display;
    public TextMeshProUGUI cost;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (onClick != null)
            onClick(this);
    }

    internal void UpdateSpell(BaseSpell baseSpell, bool h)
    {
        highlight.gameObject.SetActive(h);
        display.sprite = baseSpell.UiSprite;
        cost.text = baseSpell.spellCost.ToString();
    }
}
