using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    public DataManager dataManager;
    public GameObject past, present;
    public float walkSpeed;
    public bool currentlyInPast;

    // Other components
    private Camera mainCamera;
    private new Rigidbody2D rigidbody;

    // Player stats
    private bool canSwitch;
    private Vector2 otherPosition;

    // Touching Entities
    private HashSet<Entity> touchingEntities = new HashSet<Entity>();

    void Awake() {
        mainCamera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        currentlyInPast = false;
        canSwitch = true;
        otherPosition = new();
    }

    void Update() {
        // Check for player inputs
        if (Input.GetKeyDown(KeyCode.Q) && canSwitch) {
            canSwitch = false;
            // Change room idk, it might yield
            // switch positions
            currentlyInPast = !currentlyInPast;
            otherPosition = transform.position;
            transform.position = past.transform.position;
            if (currentlyInPast) {
                past.SetActive(true);
                present.SetActive(false);
            }
            else {
                past.SetActive(false);
                present.SetActive(true);
            }

            canSwitch = true;
        }

        // Do player movement
        Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.magnitude > 0)
            input.Normalize();
        Vector2 newPos = transform.position + Time.deltaTime * walkSpeed * (Vector3)input;
        rigidbody.MovePosition(newPos);
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        

        // Do entity interaction
        if (Input.GetKeyDown(KeyCode.E)) {
            foreach (Entity entity in touchingEntities) {
                entity.Interact(this);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if(entity != null) {
            touchingEntities.Add(entity);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if(entity != null) {
            touchingEntities.Remove(entity);
        }
    }

}
