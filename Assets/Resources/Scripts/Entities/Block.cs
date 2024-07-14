using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Collectible
{
    public override void OnDrop(Collector collector)
    {
        transform.position = collector.transform.position;
        gameObject.SetActive(true);
        Locked = true;
    }
}