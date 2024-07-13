using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
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
    }

    public override void Interact(Player player)
    {
        throw new System.NotImplementedException("Player cannot interact with itself.");
    }
}
