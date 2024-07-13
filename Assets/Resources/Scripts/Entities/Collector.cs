using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : Entity
{
    public Collectible requiredCollectible;
    [SerializeField]
    protected PresentCollectible presentCollectible;

    public override void Interact(Player player) {
        if (locked) return;
        bool success = player.DropEntity(this);
        if (success) {
            locked = true;
            presentCollectible.transform.position = presentCollectible.newPosition;
        }
    }
}
