using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockDoor : Collector
{
    public override void Interact(Player player) {
        if (locked) return;
        bool success = player.DropEntity(this);
        if (success) {
            locked = true;
            presentCollectible.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
