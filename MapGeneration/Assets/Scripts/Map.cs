using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

    [SerializeField]
    private GameObject[] tiles = new GameObject[2];
    private GameObject[,] tileMap;

    private float size = 20.0f;
    private int[,] kernel = new int[,] { { 1, 1, 1 }, { 1, 0, 1 }, { 1, 1, 1 } };
    private int[,] map;
    private static int tileAmount, tileTotal;
    private const float updateRate = 0.0000000000001f;
    public int mapSizeX = 100, mapSizeY = 60;
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int ranFillProcent = 60;

    void Awake () {
        // map information
        tileTotal = mapSizeX * mapSizeY;
        map = new int[mapSizeX, mapSizeY];
        tileMap = new GameObject[mapSizeX, mapSizeY];
        // Creates the initial map
        FillMap();
        FixMap(1);
        IllustrateMap();
	}

    void Update()
    {
        // Press M to create a new map
        if (Input.GetKeyDown(KeyCode.M))
        {
            FillMap();
            FixMap(1);
            StartCoroutine(IllustrateMap());
        }
    }

    // --- <Summary Begin> ---
    // Takes a string, and gets a HashCode from it. 
    // To create a map of values between 0 and the amount of different tiles.
    // --- <Summary End> ---
    private void FillMap()
    {
        // Sets the seed before setting up the map
        if(useRandomSeed || seed.ToString() == string.Empty)
            seed = System.DateTime.Now.ToString();

        Random.seed = seed.GetHashCode();

        for (int x = 0; x < mapSizeX; x++)
            for (int y = 0; y < mapSizeY; y++)
            {
                int prng = Random.Range(0, 100);
                if (x == 0 ||  x == mapSizeX - 1) // Set X border to 0
                    map[x, y] = 0;
                else if(y == 0 || y == mapSizeY - 1) // Set Y border to 0
                    map[x, y] = 0;
                else if (prng < ranFillProcent) // Set map position to 0 if the psudeo random number is less than ranFill%
                    map[x, y] = 0;
                else if (prng >= ranFillProcent) // Set map position to 1 if the psudeo random number is greater than or equal to  ranFill%
                    map[x, y] = 1;
            }
    }

    // --- <Summary Begin> ---
    // Does a Kernel comparison with the other tiles around it.
    // All depends on how the kernel has been setup.
    // --- <Summary End> ---
    private void FixMap(int _x)
    {
        // _x decides how many times this filtering should run through the map.
        if (_x <= 0)
            _x = 1;
        // Filters the map _x times
        for (int i = 0; i < _x; i++)
            for (int x = 1; x < mapSizeX - 1; x++)
                for (int y = 1; y < mapSizeY - 1; y++)
                {
                    int sum = 0;
                    for (int kx = -1; kx <= 1; kx++)
                        for (int ky = -1; ky <= 1; ky++)
                            sum += map[x + kx, y + ky] * kernel[kx + 1, ky + 1];
                    if (sum >= 4)
                        map[x, y] = 1;
                    else
                        map[x, y] = 0;
                }
    }

    // --- <Summary Begin> ---
    // Creates and updates the map with the correct texture, on every gameobject.
    // --- <Summary End> ---
    private IEnumerator IllustrateMap()
    {
        // First time it is run, and need to create all the tiles, before it can adjusts them.
        if (tileAmount < tileTotal) 
        {
            for (int x = 0; x < mapSizeX; x++)
                for (int y = 0; y < mapSizeY; y++)
                {
                    GameObject temp = null;
                    temp = Instantiate(tiles[map[x, y]], new Vector3((this.transform.position.x - mapSizeX / 2.0f) + x, this.transform.position.y, (this.transform.position.z - mapSizeY / 2.0f) + y), Quaternion.identity) as GameObject;
                    temp.transform.SetParent(this.transform);
                    tileMap[x, y] = temp;
                    tileAmount++;
                }
        }
        else // Changes the map if it already has been created once.
        {
            for (int x = 0; x < mapSizeX; x++)
                for (int y = 0; y < mapSizeY; y++)
                {
                    if (tileMap[x, y].tag != tiles[map[x, y]].tag)
                    {
                        SwitchGameObject(tileMap[x, y], tiles[map[x, y]]);
                        // Comment this out if it is too slow at updating.
                        yield return new WaitForSeconds(updateRate);
                    }
                }
        }
    }

    // --- <Summary Begin> ---
    // Updates the Gameobject with the correct coresponding with the tile numbers.
    // --- <Summary End> ---
    private void SwitchGameObject(GameObject oriObj, GameObject newObj)
    {
        GameObject temp = null;
        temp = Instantiate(newObj, oriObj.transform.position, Quaternion.identity) as GameObject;
        temp.transform.SetParent(this.transform);
        Destroy(oriObj);
    }
}
