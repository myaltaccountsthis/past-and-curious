using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : Entity
{
    public override void Interact(Player player) {
        player.DropEntity(this);
    }
}
