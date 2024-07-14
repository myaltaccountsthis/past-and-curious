using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Entity
{
    public Entity pastGate;
    public Entity presentGate;
    [SerializeField]
    private Sprite otherSprite;

    public override void Interact(Player player)
    {
        if (Locked) return;
        Locked = true;
        pastGate.gameObject.SetActive(false);
        presentGate.gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().sprite = otherSprite;
    }
}
