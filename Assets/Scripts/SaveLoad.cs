using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad {

    public int[,] map { get; private set; }

    public SaveLoad() {
        map = MapGenerator.map;
    }

	public void save() {
        List<Ally> allies = GameManager.instance.allies;
        List<Enemy> enemies = GameManager.instance.enemies;

        List<UnitData> unitStats = new List<UnitData>();
        //Fill unitStats list with each unit's stats (allies and enemies)
        for(int i = 0; i < allies.Count; i++) {
            unitStats.Add(new UnitData(allies[i]));
        }
        for(int i = 0; i < enemies.Count; i++) {
            unitStats.Add(new UnitData(enemies[i]));
        }

        BinaryFormatter bf = new BinaryFormatter();

        //Save unit stats
        FileStream unitsFile = File.Create(Application.dataPath + "SpeedrunStrategyUnits.sav");
        bf.Serialize(unitsFile, unitStats);
        unitsFile.Close();

        //Save map data
        FileStream mapFile = File.Create(Application.dataPath + "SpeedrunStrategyMap.sav");
        bf.Serialize(mapFile, map);
        mapFile.Close();
    }

    public void load() {
        //Check if save files exist
        if(File.Exists(Application.dataPath + "SpeedrunStrategyUnits.sav")) {
            if(File.Exists(Application.dataPath + "SpeedrunStrategyMap.sav")) {
                BinaryFormatter bf = new BinaryFormatter();

                //Load unit stats
                FileStream unitsFile = File.Open(Application.dataPath + "SpeedrunStrategyUnits.sav", FileMode.Open);
                List<UnitData> units = (List<UnitData>) bf.Deserialize(unitsFile);
                unitsFile.Close();

                //Load map data
                FileStream mapFile = File.Open(Application.dataPath + "SpeedrunStrategyMap.sav", FileMode.Open);
                map = (int[,]) bf.Deserialize(mapFile);
                mapFile.Close();

                //Delete the files used to load data
                File.Delete(Application.dataPath + "SpeedrunStrategyUnits.sav");
                File.Delete(Application.dataPath + "SpeedrunStrategyMap.sav");

                //Debug.Log("Loaded");

                //Delete current map tiles and units
                MapGenerator loadedMapGen = GameObject.Find("GameManager").GetComponent<MapGenerator>();
                loadedMapGen.deleteMap();
                Unit.deleteUnits();

                //Load the new map
                loadedMapGen.drawMap(map, map.GetLength(0), map.GetLength(1));

                //Set game mode to what it was in loaded game
                GameManager.setTwoPlayer(units[0].twoPlayer);

                //Get a list of all units
                List<GameObject> allyUnits = GameManager.instance.allyUnits;
                List<GameObject> enemyUnits = GameManager.instance.enemyUnits;
                List<GameObject> allUnits = new List<GameObject>();
                for(int i = 0; i < allyUnits.Count; i++) {
                    allUnits.Add(allyUnits[i]);
                }
                for(int i = 0; i < enemyUnits.Count; i++) {
                    allUnits.Add(enemyUnits[i]);
                }

                for(int i = 0; i < units.Count; i++) {
                    //Get things needed to instantiate units
                    GameObject currentUnit = searchForUnit(allUnits, units[i]);
                    Coord position = new Coord(units[i].position.X, units[i].position.Y);

                    //Instantiate unit and set it up with correct values
                    GameObject loadedUnit = (GameObject) MonoBehaviour.Instantiate(currentUnit, position.worldPos, Quaternion.identity);
                    if(loadedUnit.GetComponent<Unit>().GetType() == typeof(Ally)) {
                        Ally loadedAlly = loadedUnit.GetComponent<Ally>();
                        GameManager.instance.allies[i] = loadedAlly;
                        GameManager.instance.allies[i].setupUnit(units[i].unitClass, position);
                    }else if(loadedUnit.GetComponent<Unit>().GetType() == typeof(Enemy)) {
                        Enemy loadedEnemy = loadedUnit.GetComponent<Enemy>();
                        GameManager.instance.enemies[i % GameManager.numAllies] = loadedEnemy;
                        GameManager.instance.enemies[i % GameManager.numAllies].setupUnit(units[i].unitClass, position);
                    }
                }
            }
        }
    }

    //Find a unit in allUnits with the same name as target
    GameObject searchForUnit(List<GameObject> allUnits, UnitData target) {
        for(int i = 0; i < allUnits.Count; i++) {
            if(allUnits[i].GetComponent<SpriteRenderer>().sprite.name.Equals(target.unitClass)) {
                return allUnits[i];
            }
        }
        return null;
    }
}