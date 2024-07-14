using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentCollectible : Entity
{
    public Vector2 newPosition;
    public Sprite otherSprite;
    
    public override void Interact(Player player) {
        return;
    }
}
