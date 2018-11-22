using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePlayerInput : BaseInput
{

    protected override void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        spellTarget.transform.position = mousePos;
        if (ui.over == false &&  Input.GetMouseButtonDown(0))
        {
            spawn = true;
        }
        base.Update();

    }
}
