using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public virtual bool AutoInteract => false;
    // If locked, player can't interact
    [SerializeField] private bool locked = false;
    public bool Locked {
        get => locked;
        set {
            GetComponent<Collider2D>().enabled = !value;
            locked = value;
        }
    }

    public abstract void Interact(Player player);
}
