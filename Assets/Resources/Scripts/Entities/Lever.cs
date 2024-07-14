using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Entity
{
    public PresentCollectible pastGate;
    public PresentCollectible presentGate;
    public int score;
    [SerializeField]
    private Sprite otherSprite;

    public override void Interact(Player player)
    {
        if (Locked) return;
        Locked = true;
        // pastGate.GetComponent<SpriteRenderer>().sprite = pastGate.otherSprite;
        // pastGate.GetComponent<BoxCollider2D>().enabled = false;
        // presentGate.GetComponent<SpriteRenderer>().sprite = presentGate.otherSprite;
        // presentGate.GetComponent<BoxCollider2D>().enabled = false;
        pastGate.gameObject.SetActive(false);
        presentGate.gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().sprite = otherSprite;
        player.AddScore(score);
    }
}
