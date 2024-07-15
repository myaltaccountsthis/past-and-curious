using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct LaserData {
    public Tuple<float, int, int, float>[] Cells;
}

public class LaserRoom : MonoBehaviour
{
    public Room room;
    
    private List<LaserData> laserData;
    private SpriteRenderer laserPrefab;
    private Transform wallPrefab;
    public bool Activated {get; private set;}
    public bool Done {get; private set;}
    public bool GameFinished {get; private set;}

    private static readonly Color waitingColor = new(1, 0, 0, .25f);
    private static readonly Color activeColor = Color.red;
    
    // Audio Sources
    public AudioSource laser;
    public AudioSource mainMusic;
    public AudioSource bossMusic;

    void Awake() {
        // LaserData[] fileLaserData = laserFile.text.Trim().Split('\n').Select(line => new LaserData() {Cells = line.Trim().Split(' ').Select(pair => pair.Split(',')).Select(pair => new Tuple<float, int, int, float>(float.Parse(pair[0]), int.Parse(pair[1]), int.Parse(pair[2]), pair.Length < 4 ? -1 : float.Parse(pair[3]))).ToArray()}).ToArray();
        // laserData = new LaserData[fileLaserData.Length * 2 - 1];
        LoadLaserData();
        laserPrefab = Resources.Load<SpriteRenderer>("Prefabs/Entities/Laser");
        wallPrefab = Resources.Load<Transform>("Prefabs/LaserWall");
    }

    void Start() {
        Activated = false;
        Done = false;
        GameFinished = false;
    }

    private void LoadLaserData() {
        laserData = new();
        List<Tuple<float, int, int, float>> arr = new();
        {
            // rows top to bottom
            for (int i = 12; i > 0; i--)
            {
                for (int j = 0; j < 13; j++)
                    arr.Add(new((12 - i) * 0.3f, j, i, -1));
            }
            // rows bottom to top
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 13; j++)
                    arr.Add(new(2 + (i + 12) * 0.3f, j, i, -1));
            }
            laserData.Add(new LaserData() {Cells = arr.ToArray()});
            arr.Clear();
        }
        {
            // squares in to out
            arr.Add(new(0, 6, 6, -1));
            for (int i = 1; i < 6; i++)
            {
                for (int j = 0; j < 2 * i; j++)
                {
                    arr.Add(new(i * 0.3f, 6 - i + j, 6 + i, -1));
                    arr.Add(new(i * 0.3f, 6 + i, 6 + i - j, -1));
                    arr.Add(new(i * 0.3f, 6 + i - j, 6 - i, -1));
                    arr.Add(new(i * 0.3f, 6 - i, 6 - i + j, -1));
                }
            }
            // squares out to in
            for (int i = 6; i > 0; i--)
            {
                for (int j = 0; j < 2 * i; j++)
                {
                    arr.Add(new((6 - i + 6) * 0.3f + 1.5f, 6 - i + j, 6 + i, -1));
                    arr.Add(new((6 - i + 6) * 0.3f + 1.5f, 6 + i, 6 + i - j, -1));
                    arr.Add(new((6 - i + 6) * 0.3f + 1.5f, 6 + i - j, 6 - i, -1));
                    arr.Add(new((6 - i + 6) * 0.3f + 1.5f, 6 - i, 6 - i + j, -1));
                }
            }
            laserData.Add(new LaserData() {Cells = arr.ToArray()});
            arr.Clear();
        }
        {
            // alternating checker board
            float t = 0;
            for (int i = 0; i < 7; i++)
            {
                float delay = 1 - i * 0.1f;
                for (int j = i % 2; j < 13 * 13; j += 2)
                {
                    arr.Add(new(t, j % 13, j / 13, delay));
                }
                t += delay + 1;
            }
            laserData.Add(new LaserData() {Cells = arr.ToArray()});
            arr.Clear();
        }
        {
            // sweep to side
            List<List<Tuple<float, int, int, float>>> directions = new() { new(), new(), new(), new() };
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    directions[0].Add(new(0, i, j, 2.5f)); // left
                    directions[1].Add(new(0, j, i, 2.5f)); // top
                    directions[2].Add(new(0, 12 - i, j, 2.5f)); // right
                    directions[3].Add(new(0, j, 12 - i, 2.5f)); // bottom
                }
            }
            List<int> toShuffle = new() { 0, 0, 1, 1, 2, 2, 3, 3 };
            for (int i = 0; i < toShuffle.Count - 1; i++)
            {
                int index = UnityEngine.Random.Range(i + 1, toShuffle.Count);
                (toShuffle[index], toShuffle[i]) = (toShuffle[i], toShuffle[index]);
            }
            float t = 0;
            for (int i = 0; i < toShuffle.Count; i++)
            {
                float delay = 4.2f - 0.4f * i;
                for (int j = 0; j < directions[toShuffle[i]].Count; j++)
                {
                    Tuple<float, int, int, float> dirArr = directions[toShuffle[i]][j];
                    arr.Add(new(t, dirArr.Item2, dirArr.Item3, delay));
                }
                t += delay + 3f;
            }
            laserData.Add(new LaserData() {Cells = arr.ToArray()});
            arr.Clear();
        }

        // Put random data in between
        List<LaserData> newLaserData = new();
        for (int i = 0; i < laserData.Count; i++)
        {
            newLaserData.Add(laserData[i]);
            if (i < laserData.Count - 1)
                newLaserData.Add(GetRandomLaserData());
        }
        laserData = newLaserData;
    }

    private LaserData GetRandomLaserData() {
        List<Tuple<float, int, int, float>> arr = new();
        int[] tiles = new int[13 * 13];
        for (int i = 0; i < 13 * 13; i++)
            tiles[i] = i;
        // random tiles
        float t = 0;
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < tiles.Length - 1; j++) {
                int index = UnityEngine.Random.Range(j + 1, tiles.Length);
                (tiles[j], tiles[index]) = (tiles[index], tiles[j]);
            }
            int count = Mathf.FloorToInt((.3f + i*.1f) * 13 * 13);
            float delay = 1.2f;
            for (int j = 0; j < count; j++)
                arr.Add(new(t, tiles[j] % 13, tiles[j] / 13, delay));
            t += delay + 1;
        }
        return new LaserData() { Cells = arr.ToArray() };
    }

    public void OnInteract(Action callback) {
        if (GameFinished)
            return;
        if (Done) {
            // Activate finished sequence
            GameFinished = true;
            callback();
            return;
        }
        if (Activated)
            return;
        Activated = true;
        Instantiate(wallPrefab, room.transform);
        StartCoroutine(ActivateLaserSequence(callback));
        bossMusic.gameObject.SetActive(true);
        mainMusic.gameObject.SetActive(false);
    }

    IEnumerator ActivateLaserSequence(Action callback) {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < laserData.Count; i++) {
            float t = 0;
            float defaultDelay = 1.5f - i * .3f;
            float lastDelay = defaultDelay;
            foreach (Tuple<float, int, int, float> cell in laserData[i].Cells)
            {
                while (cell.Item1 > t)
                {
                    yield return null;
                    t += Time.deltaTime;
                }
                
                lastDelay = cell.Item4 == -1 ? defaultDelay : cell.Item4;
                StartCoroutine(ActivateLaser(Instantiate(laserPrefab, room.transform), cell, lastDelay));
            }

            yield return new WaitForSeconds(2 + lastDelay);
        }
        Done = true;
        Debug.Log("Done");
        // Finished lasers
        // TODO open next room or finish game smth
        callback();
    }

    IEnumerator ActivateLaser(SpriteRenderer renderer, Tuple<float, int, int, float> cell, float delay)
    {
        renderer.color = waitingColor;
        renderer.transform.SetParent(room.transform);
        renderer.transform.position = transform.position + new Vector3(cell.Item2 + .5f, cell.Item3 + .5f);
        yield return new WaitForSeconds(delay);
        renderer.GetComponent<Killbrick>().Locked = false;
        renderer.color = activeColor;
        // Destroy the laser .5 seconds after activating
        yield return new WaitForSeconds(.6f);
        AudioSource.PlayClipAtPoint(laser.clip, laser.transform.position);
        Destroy(renderer.gameObject);
    }
}
