using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public virtual bool AutoInteract => false;
    // If locked, player can't interact
    public bool locked = false;

    public abstract void Interact(Player player);
}
