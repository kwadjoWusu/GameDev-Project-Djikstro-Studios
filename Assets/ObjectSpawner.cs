using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType
    {
        Coin, Enemy, None
    }
    public Tilemap tilemap;
    public GameObject[] objectPrefabs; // 0= coin, 1=enemy
    public float coinProbibility = 0.2f;

    public float enemyProbibility = 0.1f;
    public float maxObjects = 5;

    public float coinLifeTime = 10f;
    public float enemyLifeTime = 10f; // Added enemy lifetime parameter
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();

    private bool isSpawning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
    }

    // Update is called once per frame
    void Update()
    {
        if (!tilemap.gameObject.activeInHierarchy)
        {
            //Level change
            LevelChange();
        }

        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }
    
    private void LevelChange()
    {
        tilemap = GameObject.Find("Platforms").GetComponent<Tilemap>();
        GatherValidPositions();
        // Destroy spawned objects
        DestoryAllSpaenedObjects();
    }

    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        isSpawning = true;
        while (ActiveObjectCount() < maxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;
        if (randomChoice <= enemyProbibility)
        {
            return ObjectType.Enemy;
        }
        else if (randomChoice <= (enemyProbibility + coinProbibility))
        {
            return ObjectType.Coin;
        }
        else
        {
            return ObjectType.None;
        }
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 0.1f);
    }

    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;
        while (!validPositionFound && validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPositions.Count);
            Vector3 potentialPosition = validSpawnPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }
            validSpawnPositions.RemoveAt(randomIndex);
        }
        
        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            
            // Only spawn objects for Coin and Enemy types, not for None
            if (objectType != ObjectType.None)
            {
                GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
                spawnObjects.Add(gameObject);

                // Apply appropriate lifetime based on object type
                if (objectType == ObjectType.Coin)
                {
                    StartCoroutine(DestoryObjectAfterTime(gameObject, coinLifeTime));
                }
                else if (objectType == ObjectType.Enemy)
                {
                    StartCoroutine(DestoryObjectAfterTime(gameObject, enemyLifeTime));
                }
            }
        }
    }

    private IEnumerator DestoryObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject)
        {
            spawnObjects.Remove(gameObject);
            validSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    private void DestoryAllSpaenedObjects()
    {
        foreach (GameObject gameObject in spawnObjects)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        spawnObjects.Clear();
    }

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 1.5f, 0);
                    validSpawnPositions.Add(place);
                }
            }
        }
    }
}