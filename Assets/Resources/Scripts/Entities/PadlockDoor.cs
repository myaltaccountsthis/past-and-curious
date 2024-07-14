using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockDoor : Collector
{
    public PresentCollectible pastCollectible;
    public override void Interact(Player player) {
        if (Locked) return;
        bool success = player.DropEntity(this);
        if (success) {
            Locked = true;
            pastCollectible.gameObject.SetActive(false);
            presentCollectible.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
