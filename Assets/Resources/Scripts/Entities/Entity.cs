using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public bool inPast;
    public bool inPresent;

    

    public abstract void Interact(Player player);
}
