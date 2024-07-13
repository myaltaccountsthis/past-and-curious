using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbrick : Entity
{
    public override bool AutoInteract => true;

    public override void Interact(Player player) {
        player.Respawn();
    }
}
