using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [HideInInspector] public bool visited;
    public int index;
    public Transform spawnLocation;
    private List<GameObject> children;

    void Awake() {
        Debug.Assert(GetComponent<Collider2D>() != null, "Room must have a Collider2D component.");
        children = new List<GameObject>();
        foreach (Transform child in transform) {
            if (!child.gameObject.activeSelf)
                continue;
            children.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    public void Activate() {
        // Destroy previous children that are active
        foreach (Transform child in transform) {
            if (child.gameObject.activeSelf)
                Destroy(child.gameObject);
        }
        // Instantiate new children
        foreach (GameObject child in children) {
            Instantiate(child, child.transform.position, Quaternion.identity, transform).SetActive(true);
        }
    }

    void Start() {
        visited = false;
        Activate();
    }
}
