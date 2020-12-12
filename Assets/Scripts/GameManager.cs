using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance { get; private set; }
    public static MapGenerator mapGen { get; private set; }

    public static bool twoPlayer { get; private set; }
    public static void setTwoPlayer(bool _twoPlayer) {
        twoPlayer = _twoPlayer;
    }

    public static readonly int numAllies = 6;

    public List<GameObject> allyUnits = new List<GameObject>(numAllies);
    public List<Ally> allies = new List<Ally>(numAllies);

    public List<GameObject> enemyUnits = new List<GameObject>(numAllies);
    public List<Enemy> enemies = new List<Enemy>(numAllies);

    public Ally selectedAlly { get; private set; }
    public void setSelectedAlly(Ally selected) {
        selectedAlly = selected;
    }

    public Enemy selectedEnemy { get; private set; }
    public void setSelectedEnemy(Enemy selected) {
        selectedEnemy = selected;
    }

    static readonly bool allyTurn = true;
    static readonly bool enemyTurn = false;
    static bool isAllyTurn = allyTurn;

    public static bool getCurrentTurn() {
        return isAllyTurn;
    }

    public static void setCurrentTurn(bool _isAllyTurn) {
        isAllyTurn = _isAllyTurn;
    }

    public static int width { get; private set; }
    public static int height { get; private set; }
    public static int mountPercent { get; private set; }
    public static int forestPercent { get; private set; }

    // Use this for initialization
    void Awake () {
        //Singleton
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
    }
	
    void Start() {
        //Get user-chosen parmaters from StartMenu 
        width = StartMenu.width;
        if(width == 0) {
            width = 36; //Default width
        }

        height = StartMenu.height;
        if(height == 0) {
            height = 20; //Default height
        }

        mountPercent = StartMenu.mountPercent;
        if(mountPercent == 0) {
            mountPercent = 35; //Default mountPercent
        }

        forestPercent = StartMenu.forestPercent;
        if(forestPercent == 0) {
            forestPercent = 20; //Default forestPercent
        }

        twoPlayer = StartMenu.twoPlayer;

        //Generate the map based on the user-chosen paramaters
        mapGen = gameObject.GetComponent<MapGenerator>();
        mapGen.generateMap(width, height, mountPercent, forestPercent);

        //Deploy the allies and enemies in bottom-left and top-right corners respectively
        Coord[] flattenedMap = flattenMap();
        deployAllies(flattenedMap);
        flattenedMap = flattenMap();
        deployEnemies(flattenedMap);
    }

    //Turn the 2D map array into a 1D array
    Coord[] flattenMap() {
        Coord[] flattenedMap = new Coord[width * height];
        int i = 0;
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                flattenedMap[i] = new Coord(x, y);
                i++;
            }
            if(i >= flattenedMap.Length) {
                break;
            }
        }
        return flattenedMap;
    }

    //Spawn allies in bottom-left corner in a group and 3 per row
    void deployAllies(Coord[] flattenedMap) {
        //If a space isn't a mountain, put next unit in allyUnits there
        int allyIndexer = 0;
        int rowAllyCount = 0;
        for(int spaceIndexer = 0; spaceIndexer < flattenedMap.Length; spaceIndexer++) {
            if(rowAllyCount >= 3) {
                spaceIndexer += (width-3);
                rowAllyCount = 0;
            }
            if(MapGenerator.allTiles[flattenedMap[spaceIndexer]].moveCost < 999 && allyIndexer < allyUnits.Count) {
                GameObject nextAlly = (GameObject) Instantiate(allyUnits[allyIndexer], flattenedMap[spaceIndexer].worldPos, Quaternion.identity);
                string allyClass = nextAlly.GetComponent<SpriteRenderer>().sprite.name;
                allies[allyIndexer] = nextAlly.GetComponent<Ally>();
                allies[allyIndexer].setupUnit(allyClass, flattenedMap[spaceIndexer]);
                rowAllyCount++;
                allyIndexer++;
            }
        }
    }

    //Spawn enemies in top-left corner in a group and 3 per row
    void deployEnemies(Coord[] flattenedMap) {
        int enemyIndexer = 0;
        int rowEnemyCount = 0;
        for(int spaceIndexer = flattenedMap.Length - 1; spaceIndexer >= 0; spaceIndexer--) {
            if(rowEnemyCount >= 3) {
                spaceIndexer -= (width - 3);
                rowEnemyCount = 0;
            }
            if(MapGenerator.allTiles[flattenedMap[spaceIndexer]].moveCost < 999 && enemyIndexer < enemyUnits.Count) {
                GameObject nextEnemy = (GameObject) Instantiate(enemyUnits[enemyIndexer], flattenedMap[spaceIndexer].worldPos, Quaternion.identity);
                string enemyClass = nextEnemy.GetComponent<SpriteRenderer>().sprite.name;
                enemies[enemyIndexer] = nextEnemy.GetComponent<Enemy>();
                enemies[enemyIndexer].setupUnit(enemyClass, flattenedMap[spaceIndexer]);
                rowEnemyCount++;
                enemyIndexer++;
            }
        }
    }

    //Check if all units in a team have moved and if so, change turn
    public bool checkTurnEnd() {
        if(Unit.movedAllies.Count == allyUnits.Count) {
            //Debug.Log("Enemy turn begins");
            Unit.movedAllies.Clear();
            isAllyTurn = enemyTurn;
        } else if(Unit.movedEnemies.Count == enemyUnits.Count) {
            //Debug.Log("Ally turn begins");
            Unit.movedEnemies.Clear();
            isAllyTurn = allyTurn;
        }
        return isAllyTurn;
    }

    // Update is called once per frame
    void Update() {
        //If the right mouse button is pressed, change all unit range tiles back to normal
        if(Input.GetMouseButtonDown(1) && (selectedAlly != null || selectedEnemy != null)) {
            foreach(Tile tile in MapGenerator.allTiles.Values) {
                tile.GetComponent<SpriteRenderer>().sprite = tile.normal;
            }
        }
    }
}