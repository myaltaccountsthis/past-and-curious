using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Entity
{
    public Killbrick[] lights;
    public int score;
    [SerializeField]
    private Sprite otherSprite;

    public override void Interact(Player player)
    {
        if (Locked) return;
        Locked = true;
        foreach (Killbrick light in lights)
        {
            light.GetComponent<SpriteRenderer>().color = new(1, 1, 1, .2f);
        }
        GetComponent<SpriteRenderer>().sprite = otherSprite;
        player.AddScore(score);
    }
}
