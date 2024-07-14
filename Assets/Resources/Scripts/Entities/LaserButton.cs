using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserButton : Entity
{
    public Sprite greenSprite;

    private SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Interact(Player player)
    {
        LaserRoom laserController = transform.parent.GetComponentInChildren<LaserRoom>();
        Locked = true;
        laserController.OnInteract(() => {
            if (laserController.GameFinished)
                player.OnWin();
            else if (laserController.Done) {
                spriteRenderer.sprite = greenSprite;
                Locked = false;
            }
        });
    }
}
