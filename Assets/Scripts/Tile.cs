using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour{

    public Coord point { get; private set; }
    public int moveCost { get; private set; }
    
    //These are given values in the Unity editor
    public Sprite normal, allyRange, enemyRange;

    readonly int infinity = 999;

    //Essentially a constructor
    //Objects of classes inheriting from MonoBehaviour can't be instantiated with a constructor
    //Instead GetComponent<T>() is needed and a function that acts like a constructor must be called
    public void setupTile(Coord gridPos, GameObject tileVal) {
        point = gridPos;
        transform.position = gridPos.worldPos;
        if(tileVal.name == "grass") {
            moveCost = 1;
        } else if(tileVal.name == "forest") {
            moveCost = 2;
        } else if(tileVal.name == "mountain") {
            moveCost = infinity;
        }
    }

    public void showMovableSpaces(Dictionary<Coord, int> movableSpaces, Unit selected) {
        if(moveCost < infinity) {
            //Debug.Log(point.X + ", " + point.Y + " Cost: " + moveCost);

            //Set movable tiles to blue if an ally is selected and red if enemy
            foreach(Coord movableSpace in movableSpaces.Keys) {
                Tile t = MapGenerator.allTiles[movableSpace];
                if(selected.GetType() == typeof(Ally)) {
                    t.GetComponent<SpriteRenderer>().sprite = t.allyRange;
                }else if(selected.GetType() == typeof(Enemy)) {
                    t.GetComponent<SpriteRenderer>().sprite = t.enemyRange;
                } else {
                    t.GetComponent<SpriteRenderer>().sprite = t.normal;
                }
            }
        }
    }

    void OnMouseOver() {
        if(Input.GetMouseButtonDown(0)){
            //Validate clicked tile
            if(GameManager.twoPlayer) {
                //In 2 player mode, check for which turn it is and which colur tile is clicked to decide whether to move an ally or an enemy
                if(GameManager.instance.checkTurnEnd() && GetComponent<SpriteRenderer>().sprite == allyRange && !InGameMenus.attacking) {
                    GameManager.instance.selectedAlly.moveUnit(point);
                }else if(!GameManager.getCurrentTurn() && GetComponent<SpriteRenderer>().sprite == enemyRange && !InGameMenus.attacking) {
                    GameManager.instance.selectedEnemy.moveUnit(point);
                }
            }else {
                //In 1 player mode, only ally movement needs validating
                if(GameManager.instance.checkTurnEnd() && GetComponent<SpriteRenderer>().sprite == allyRange && !InGameMenus.attacking) {
                    GameManager.instance.selectedAlly.moveUnit(point);
                }
            }
        }
    }
}
