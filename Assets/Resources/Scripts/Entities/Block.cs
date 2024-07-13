using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Entity
{
    public override void Interact(Player player) {
        player.CollectEntity(this);
    }
}