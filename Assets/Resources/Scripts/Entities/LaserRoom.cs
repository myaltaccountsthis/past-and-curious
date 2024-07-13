using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct LaserData {
    public Tuple<float, int, int>[] Cells;
}

public class LaserRoom : MonoBehaviour
{
    public Room room;
    public TextAsset laserFile;
    
    private LaserData[] laserData;
    private SpriteRenderer laserPrefab;
    private static readonly Color waitingColor = new(1, 0, 0, .25f);
    private static readonly Color activeColor = Color.red;

    void Awake() {
        laserData = laserFile.text.Split('\n').Select(line => new LaserData() {Cells = line.Trim().Split(' ').Select(pair => pair.Split(',')).Select(pair => new Tuple<float, int, int>(float.Parse(pair[0]), int.Parse(pair[1]), int.Parse(pair[2]))).ToArray()}).ToArray();
        laserPrefab = Resources.Load<SpriteRenderer>("Prefabs/Entities/Laser");
    }

    void Start() {
        StartCoroutine(ActivateLasers());
    }

    IEnumerator ActivateLasers() {
        for (int i = 0; i < laserData.Length; i++) {
            yield return new WaitForSeconds(3);
            Debug.Log($"Starting {i}");
            int t = 0;
            float delay = LeanTween.easeOutQuad(2, .5f, (i + 1f) / laserData.Length);
            foreach (Tuple<float, int, int> cell in laserData[i].Cells) {
                if (cell.Item1 > t)
                    yield return new WaitForSeconds(cell.Item1 - t);
                StartCoroutine(ActivateLaser(Instantiate(laserPrefab, room.transform), cell, delay));
            }
        }
        yield return new WaitForSeconds(2);
        Debug.Log("Done");
        // Finished lasers
        // TODO open next room or finish game smth
    }

    IEnumerator ActivateLaser(SpriteRenderer renderer, Tuple<float, int, int> cell, float delay) {
        renderer.color = waitingColor;
        renderer.transform.SetParent(room.transform);
        renderer.transform.position = transform.position + new Vector3(cell.Item2 + .5f, cell.Item3 + .5f);
        yield return new WaitForSeconds(delay);
        renderer.color = activeColor;
        yield return new WaitForSeconds(delay / 2);
        Destroy(renderer.gameObject);
    }
}
