using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Collectible
{
    public override void OnDrop(Collector collector)
    {
        locked = true;
    }
}
