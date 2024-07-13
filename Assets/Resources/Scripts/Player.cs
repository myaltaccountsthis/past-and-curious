using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public variables
    public DataManager dataManager;
    public GameObject past, present;
    public TextMeshProUGUI interactText;
    public float walkSpeed;
    public bool currentlyInPast;
    public EntityUI entityUI;
    public InfoUI infoUI;
    public Room startingRoom, startingRoomPast;

    // Other components
    private Camera mainCamera;
    private new Rigidbody2D rigidbody;

    // Player stats
    private bool canSwitch;
    private bool canInteract;
    private Vector2 otherPosition;
    private Collectible currentEntity;
    private Collectible otherEntity;
    private Room currentRoom;
    private Room otherRoom;

    // Touching Entities
    private HashSet<Entity> touchingEntities;

    void Awake() {
        mainCamera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        currentlyInPast = false;
        canSwitch = true;
        otherPosition = new();
        touchingEntities = new();
        currentRoom = startingRoom;
        otherRoom = startingRoomPast;
    }

    void Update() {
        // Check for player inputs
        if (Input.GetKeyDown(KeyCode.Q) && canSwitch) {
            SwitchDimensions();
        }

        // Do player movement
        Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input.magnitude > 1)
            input.Normalize();
        Vector2 newPos = transform.position + Time.deltaTime * walkSpeed * (Vector3)input;
        rigidbody.MovePosition(newPos);
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        

        // Do entity interaction
        if (Input.GetKeyDown(KeyCode.E) && canInteract) {
            // Only interact with the first entity, then hide UI
            foreach (Entity entity in touchingEntities) {
                entity.Interact(this);
                CloseInteractText();
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Entity entity)) {
            if (entity.locked)
                return;
            if (entity.AutoInteract) {
                entity.Interact(this);
                return;
            }
            touchingEntities.Add(entity);
            OpenInteractText();
        }
        else if (collider.TryGetComponent(out Room room)) {
            // Try to enter a new room (don't reactivate previous rooms)
            if (!room.visited) {
                room.Activate();
                room.visited = true;
            }
            currentRoom = room;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Entity entity)) {
            if (entity.locked)
                return;
            if (entity.AutoInteract)
                return;
            touchingEntities.Remove(entity);
            if (touchingEntities.Count == 0)
                CloseInteractText();
        }
    }

    private void SwitchDimensions() {
        canSwitch = false;
        CloseInteractText();
        // Change room idk, it might yield
        // switch positions
        currentlyInPast = !currentlyInPast;
        (transform.position, otherPosition) = (otherPosition, transform.position);
        (currentEntity, otherEntity) = (otherEntity, currentEntity);
        (currentRoom, otherRoom) = (otherRoom, currentRoom);
        if (currentEntity != null) {
            entityUI.DisplayEntity(currentEntity);
        }
        else {
            entityUI.RemoveEntity();
        }
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

    private void OpenInteractText() {
        if (canInteract)
            return;
        canInteract = true;
        LeanTween.moveY(interactText.rectTransform, 20, 0.35f).setEaseOutQuad();
    }

    private void CloseInteractText() {
        if (!canInteract)
            return;
        canInteract = false;
        LeanTween.moveY(interactText.rectTransform, -40, 0.35f).setEaseOutQuad();
    }

    // Collecting a box/thing into inventory
    public bool CollectEntity(Collectible entity) {
        if (currentEntity == null) {
            currentEntity = entity;
            entityUI.DisplayEntity(entity);
            entity.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    // For signs or items that display info
    public void DisplayEntityInfo(Sign entity) {
        infoUI.DisplayInfo(entity);
    }

    // Using an entity (could be dropping)
    public void DropEntity(Collector entity) {
        if (currentEntity != null) {
            currentEntity.transform.position = entity.transform.position;
            currentEntity.gameObject.SetActive(true);
            currentEntity.locked = true;
            entity.locked = true;
            currentEntity = null;
            entityUI.RemoveEntity();
        }
    }

    public void Respawn() {
        currentRoom.Activate();
        transform.position = currentRoom.spawnLocation.position;
    }
}
