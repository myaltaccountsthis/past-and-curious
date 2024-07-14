using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : Entity
{
    public Collectible requiredCollectible;
    public int score;
    [SerializeField]
    protected PresentCollectible presentCollectible;

    public override void Interact(Player player) {
        if (Locked) return;
        bool success = player.DropEntity(this);
        if (success) {
            Locked = true;
            presentCollectible.transform.position = presentCollectible.newPosition;
            player.AddScore(score);
        }
    }
}
