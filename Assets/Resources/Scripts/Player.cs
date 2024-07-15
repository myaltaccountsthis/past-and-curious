using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    // Public variables
    public DataManager dataManager;
    public GameObject past, present;
    public TextMeshProUGUI interactText;
    public Image cover;
    public float walkSpeed;
    public bool currentlyInPast;
    public EntityUI entityUI;
    public InfoUI infoUI;
    public Room startingRoom, startingRoomPast;
    public EndScreen endScreen;
    public Sprite deadSprite;

    // Other components
    private Camera mainCamera;
    private PixelPerfectCamera pixelPerfectCamera;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Player stats
    private bool canSwitch;
    private bool canInteract;
    private bool isAlive;
    private bool inDeathAnimation;
    private Vector2 otherPosition;
    private Collectible currentEntity;
    private Collectible otherEntity;
    private Room currentRoom;
    private Room otherRoom;

    private static readonly Color pastColor = Color.green;
    private static readonly Color presentColor = Color.cyan;

    // Touching Entities
    private HashSet<Entity> touchingEntities;
    
    // Passcode Room 1
    public Sign[] noteRoom1 = new Sign[4];
    public Passcode passcode1;
    
    // Passcode Room 2
    public Sign[] noteRoom2 = new Sign[5];
    public Passcode passcode2;
    
    // Audio Sources
    public AudioSource itemPickup;
    public AudioSource itemDrop;
    public AudioSource laserDeath;
    public AudioSource timeWarp;
    public AudioSource mainMusic;
    public AudioSource bossMusic;

    private string[] pangrams =
    {
        "sphinx of black quartz, ? judge my vow…", "waltz, bad nymph, for ? quick jigs vex…",
        "these ? jackdaws love my big sphinx of quartz", "pack my box with ? dozen of my favorite liquor jugs"
    };

    void Awake() {
        mainCamera = Camera.main;
        pixelPerfectCamera = mainCamera.GetComponent<PixelPerfectCamera>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set up Passcode Room 1
        passcode1.code = Random.Range(0, 1000000).ToString("D6");
        noteRoom1[Random.Range(0, 4)].text = passcode1.code;
        
        // Set up Passcode Room 2
        Dictionary<char, int> dict = new Dictionary<char, int>();
        passcode2.code = "";
        noteRoom2[0].text = "";
        
        for (int i = 0; i < 4; i++)
        {
            char letter;
            do
            {
                letter = (char) Random.Range('a', 'z' + 1);
            } while (dict.ContainsKey(letter));
            
            int digit = Random.Range(2, 10);
            noteRoom2[0].text += char.ToUpper(letter);
            passcode2.code += digit;
            dict.Add(letter, digit);
        }

        pangrams = pangrams.OrderBy(_ => Guid.NewGuid()).ToArray();
        KeyValuePair<char, int>[] pairs = dict.OrderBy(_ => Guid.NewGuid()).ToArray();
        for (int i = 0; i < 4; i++)
        {
            char c = pairs[i].Key;
            int val = pairs[i].Value;
            string p = pangrams[i];
            int j = p.IndexOf(c);
            noteRoom2[i + 1].text = (p.Substring(0, j) + Char.ToUpper(c) + p.Substring(j + 1)).Replace("?", "" + val);
        }
    }

    void Start() {
        currentlyInPast = false;
        canSwitch = true;
        isAlive = true;
        inDeathAnimation = false;
        touchingEntities = new();
        currentRoom = startingRoom;
        otherRoom = startingRoomPast;
        transform.position = currentRoom.spawnLocation.position;
        otherPosition = otherRoom.spawnLocation.position;
    }

    void Update() {
        // Check for player inputs
        if (Input.GetKeyDown(KeyCode.Q) && canSwitch && !inDeathAnimation) {
            StartCoroutine(SwitchDimensions());
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
        }

        // STOP PLAYER INTERACTION IF SWITCHING DIMENSIONS OR DEAD
        if (canSwitch && !inDeathAnimation) {
            // Do player movement
            Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            animator.SetFloat("Horizontal", input.x);
            animator.SetFloat("Vertical", input.y);
            if (input.magnitude > 1)
                input.Normalize();
            Vector2 newPos = transform.position + Time.deltaTime * walkSpeed * (Vector3)input;
            rigidbody.MovePosition(newPos);

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

        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Entity entity)) {
            if (entity.Locked)
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
                if (currentRoom == null || room.index > currentRoom.index)
                    currentRoom = room;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Entity entity)) {
            if (entity.Locked)
                return;
            if (entity.AutoInteract)
                return;
            touchingEntities.Remove(entity);
            if (touchingEntities.Count == 0)
                CloseInteractText();
        }
    }

    private IEnumerator SwitchDimensions() {
        // canSwitch should be true initially
        canSwitch = false;
        touchingEntities.Clear();
        CloseInteractText();

        // Do funny effects
        timeWarp.time = 0.7f;
        timeWarp.Play();
        float t = 0, duration = .7f;
        float cameraOldSize = mainCamera.orthographicSize, cameraLargeSize = cameraOldSize * 1.5f, cameraSmallSize = .1f, cameraZoomOutSize = cameraOldSize * 5;
        pixelPerfectCamera.enabled = false;
        // Zoom out a bit
        while (t < duration) {
            t += Time.deltaTime;
            mainCamera.orthographicSize = LeanTween.easeOutCirc(cameraOldSize, cameraLargeSize, t / duration);
            yield return null;
        }
        // Zoom in a bit
        t = 0;
        duration = .5f;
        while (t < duration) {
            t += Time.deltaTime;
            mainCamera.orthographicSize = LeanTween.easeInCirc(cameraLargeSize, cameraSmallSize, t / duration);
            yield return null;
        }
        // Zoom out a lot + make coveer opaque
        t = 0;
        duration = 1.5f;
        Color startColor = currentlyInPast ? pastColor : presentColor, endColor = currentlyInPast ? presentColor : pastColor;
        cover.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        cover.gameObject.SetActive(true);
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = new Color(startColor.r, startColor.g, startColor.b, LeanTween.easeOutSine(0, 1, t / duration));
            mainCamera.orthographicSize = LeanTween.easeOutSine(cameraSmallSize, cameraZoomOutSize, t / duration);
            yield return null;
        }
        t = 0;

        // switch positions and data
        currentlyInPast = !currentlyInPast;
        (transform.position, otherPosition) = (otherPosition, transform.position);
        (currentEntity, otherEntity) = (otherEntity, currentEntity);
        (currentRoom, otherRoom) = (otherRoom, currentRoom);
        // Reload the room if it was previously loaded
        if (currentRoom != null && currentRoom.visited)
            currentRoom.Activate(); 
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

        // Change cover color
        duration = 1.5f;
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(startColor, endColor, t / duration);
            yield return null;
        }
        // Zoom back in + make cover transparent
        t = 0;
        duration = 1.5f;
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = new Color(endColor.r, endColor.g, endColor.b, LeanTween.easeOutQuad(1, 0, Mathf.Min(1, t / duration * 2)));
            mainCamera.orthographicSize = LeanTween.easeOutBack(cameraZoomOutSize, cameraOldSize, t / duration, .5f);
            yield return null;
        }
        cover.gameObject.SetActive(false);
        mainCamera.orthographicSize = cameraOldSize;
        pixelPerfectCamera.enabled = true;

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
        if (currentEntity == null)
        {
            itemPickup.time = 0.2f;
            itemPickup.Play();
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
    public bool DropEntity(Collector entity) {
        if (currentEntity != null && currentEntity == entity.requiredCollectible)
        {
            itemDrop.time = 0.4f;
            itemDrop.Play();
            currentEntity.OnDrop(entity);
            currentEntity = null;
            entityUI.RemoveEntity();
            return true;
        }
        return false;
    }

    public void Respawn() {
        if (!isAlive)
            return;
        if (inDeathAnimation) {
            Debug.Log("Tried to respawn while in death animation. Ignoring.");
            return;
        }
        if (!canSwitch) {
            Debug.Log("Tried to respawn while switching dimensions. Ignoring.");
            return;
        }

        laserDeath.Play();
        isAlive = false;
        inDeathAnimation = true;
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 0);
        dataManager.gameData.score -= 100;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine() {
        // Disable animations and set sprite to dead
        animator.enabled = false;
        spriteRenderer.sprite = deadSprite;
        // Funny effects
        float t = 0, duration = 1f;
        Color startColor = new(1, 0, 0, 0), middleColor = Color.red, endColor = Color.black;
        cover.color = startColor;
        cover.gameObject.SetActive(true);
        // Tween to red
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(startColor, middleColor, t / duration);
            yield return null;
        }
        // Tween to black
        t = 0;
        duration = 1.5f;
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(middleColor, endColor, LeanTween.easeInOutSine(0, 1, t / duration));
            yield return null;
        }
        cover.color = endColor;
        // Do respawn functionality
        currentRoom.Activate();
        transform.position = currentRoom.spawnLocation.position;
        animator.enabled = true;
        isAlive = true;
        // Wait and tween to transparent
        yield return new WaitForSeconds(1);
        Color transparent = new(0, 0, 0, 0);
        t = 0;
        duration = 1f;
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(endColor, transparent, t / duration);
            yield return null;
        }
        // Done with sequence
        cover.gameObject.SetActive(false);
        inDeathAnimation = false;
    }

    /// <summary>
    /// Adds score to the player's score, where more time spent means less score
    /// </summary>
    public void AddScore(int score) {
        dataManager.gameData.score += Mathf.Max(score - Mathf.FloorToInt(dataManager.gameData.time / 2), score / 2);
    }

    public void OnWin() {
        canSwitch = false;
        AddScore(5000);
        endScreen.UpdateUI(dataManager.gameData.score, dataManager.gameData.time);
        bossMusic.Stop();
        mainMusic.Play();
    }
}
