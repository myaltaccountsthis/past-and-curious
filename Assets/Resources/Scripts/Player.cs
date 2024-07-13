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

    // Other components
    private Camera mainCamera;
    private new Rigidbody2D rigidbody;

    // Player stats
    private bool canSwitch;
    private bool canInteract;
    private Vector2 otherPosition;
    private Entity currentEntity;
    public Entity CurrentEntity {
        get { return currentEntity; }
    }

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
    }

    void Update() {
        // Check for player inputs
        if (Input.GetKeyDown(KeyCode.Q) && canSwitch) {
            canSwitch = false;
            // Change room idk, it might yield
            // switch positions
            currentlyInPast = !currentlyInPast;
            (transform.position, otherPosition) = (otherPosition, transform.position);
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
        if(collider.TryGetComponent(out Entity entity)) {
            if (entity.AutoInteract) {
                entity.Interact(this);
                return;
            }
            touchingEntities.Add(entity);
            OpenInteractText();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out Entity entity)) {
            if (entity.AutoInteract)
                return;
            touchingEntities.Remove(entity);
            if (touchingEntities.Count == 0)
                CloseInteractText();
        }
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
    public bool CollectEntity(Entity entity) {
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
    public void UseEntity() {
        if (currentEntity != null) {
            currentEntity.transform.position = transform.position;
            currentEntity.gameObject.SetActive(true);
            currentEntity = null;
            entityUI.RemoveEntity();
        }
    }

}
