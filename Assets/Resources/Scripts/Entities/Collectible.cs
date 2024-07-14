using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : Entity
{
    public override void Interact(Player player) {
        if (Locked) return;
        player.CollectEntity(this);
    }

    public abstract void OnDrop(Collector collector);
}
