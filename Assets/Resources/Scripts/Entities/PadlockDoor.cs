using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockDoor : Collector
{
    public override void Interact(Player player) {
        if (Locked) return;
        bool success = player.DropEntity(this);
        if (success) {
            Locked = true;
            presentCollectible.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
