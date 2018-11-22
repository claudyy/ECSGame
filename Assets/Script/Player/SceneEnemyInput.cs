using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnemyInput : BaseInput
{
    public Rect spawnPosition;
    float cooldown;
    Vector2 targetPos;
    Vector2 fromPos;
    protected override void Update()
    {
        var pos =spellTarget.transform.position;
        pos = Vector2.Lerp(targetPos, fromPos, cooldown);
        spellTarget.transform.position = pos;


        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            cooldown = Random.Range(1,5);
            selectedSpell = Random.Range(0, curSpells.Count);
            spawn = true;
            targetPos.x = Random.Range(spawnPosition.xMin, spawnPosition.xMax);
            targetPos.y = Random.Range(spawnPosition.yMin, spawnPosition.yMax);
            fromPos = spellTarget.transform.position;
        }
        base.Update();
    }
}
