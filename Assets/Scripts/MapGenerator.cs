using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour{

    public GameObject[] tileTypes = new GameObject[3]; //0=field, 1=mount, 2=forest

    int smooths = 3;

    public float tileSize {
        get { return tileTypes[0].GetComponent<SpriteRenderer>().bounds.size.x; }
    }

    public Vector3 originPos { get; private set; }

    int width, height, mountPercent, forestPercent;

    public static int[,] map { get; private set; }

    string seed;

    public static Dictionary<Coord, Tile> allTiles;

    public void generateMap(int w, int h, int mount, int forest) {

        width = w; height = h; mountPercent = mount; forestPercent = forest; 
        map = new int[width, height];

        allTiles = new Dictionary<Coord, Tile>();

        //Debug.Log("w= " + width + ", h= " + height + ", mount= " + mountPercent + ", forest= " + forestPercent);

        //Seed the random number generator based on the time since starting the game
        //so the seed is always different
        seed = Time.realtimeSinceStartup.ToString();
        System.Random rng = new System.Random(seed.GetHashCode());

        createMountains(rng);

        for(int i = 0; i < smooths; i++) {
            smoothMountains();
        }

        addBorder();

        createForests(rng);

        drawMap(map, width, height);
    }

    //Add mountains in random positions
    void createMountains(System.Random rng) {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                map[x, y] = (rng.Next(0, 100) < mountPercent) ? 1 : 0;
            }
        }
    }

    //Make tiles more like what's next to them
    void smoothMountains() {
        int neighbourMounts;
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                neighbourMounts = countSurroundingMounts(x, y);
                if(neighbourMounts > 2) {
                    //If more than 2 mountains are next to (x, y), make it a mountain
                    map[x, y] = 1;
                }else if(neighbourMounts < 2) {
                    //If less than 2 mountains are next to (x, y), make it a field
                    map[x, y] = 0;
                }
            }
        }
    }

    //Return number of mountains in spaces directly up, down, left and right from (x, y)
    int countSurroundingMounts(int gridX, int gridY) {
        int surroundingMounts = 0;
        for(int x = gridX-1; x <= gridX+1; x++) {
            for(int y = gridY-1; y <= gridY+1; y++) {
                if(x >= 0 && x < width && y >= 0 && y < height) {
                    if(!(x == gridX && y == gridY) && (x == gridX || y == gridY)) {
                        surroundingMounts += map[x, y];
                    }
                }
            }
        }
        return surroundingMounts;
    }

    //Create a border of thickness 1 around the edge of the map
    void addBorder() {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                map[0, y] = 1;
                map[x, height - 1] = 1;
                map[width - 1, y] = 1;
                map[x, 0] = 1;
            }
        }
    }

    //Add forest tiles in random non-mountain positions
    void createForests(System.Random rng) {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(map[x, y] != 1 && rng.Next(0, 100) < forestPercent) {
                    map[x, y] = 2;
                }
            }
        }
    }

    //Draw tiles on-screen based on the values in the map array
    public void drawMap(int[,] map, int width, int height) {
        allTiles.Clear();
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                Coord point = new Coord(x, y);
                GameObject tileVal = tileTypes[map[x, y]];
                Tile newTile = Instantiate(tileVal).GetComponent<Tile>();
                newTile.setupTile(point, tileVal);
                allTiles.Add(point, newTile);
            }
        }
        //Setup the camera based on map size
        CameraManager cam = Camera.main.GetComponent<CameraManager>();
        Coord endTile = new Coord(width - 1, height - 1);
        Vector3 endTilePos = endTile.worldPos;
        cam.setCameraLimits(endTilePos, tileSize);
    }

    //For deleting the map before loading a saved one
    public void deleteMap() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tiles) {
            Destroy(tile);
        }
    }
}