using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : Entity
{
    public string title;
    public string text;
    
    public override void Interact(Player player) {
        player.DisplayEntityInfo(this);
    }
}
