using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    public Image image;
    public Entity currentEntity;

    public void DisplayEntity(Entity entity) {
        currentEntity = entity;
        image.sprite = entity.GetComponent<SpriteRenderer>().sprite;
        image.color = entity.GetComponent<SpriteRenderer>().color;
        image.enabled = true;
    }

    public void RemoveEntity() {
        currentEntity = null;
        image.enabled = false;
    }
}
