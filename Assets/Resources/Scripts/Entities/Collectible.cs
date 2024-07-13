using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : Entity
{
    public override void Interact(Player player) {
        player.CollectEntity(this);
    }
}
