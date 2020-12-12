using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class Ally : Unit {

    void OnMouseOver() {
        Ally selectedAlly = GameManager.instance.selectedAlly;
        Enemy selectedEnemy = GameManager.instance.selectedEnemy;
        if(Input.GetMouseButtonDown(0)) { //Check for left-click on ally
            if(selectedEnemy != null && selectedEnemy.validSpaces.ContainsKey(position) && InGameMenus.attacking) {
                //The selected enemy attacks this ally - will only happen in 2 player
                selectedEnemy.attack(this, false);
                selectedEnemy.hideValidSpaces();
            }else {
                selected = this;
                allTiles = MapGenerator.allTiles;
                if(selectedAlly != null && selectedAlly != this) {
                    selectedAlly.hideValidSpaces();
                }
                //If this ally hasn't been moved, show spaces it can move to
                if(!movedAllies.Contains(this) && !InGameMenus.unitsUnclickable) {
                    findValidSpaces(mov, position, 0, false);
                    allTiles[position].showMovableSpaces(validSpaces, this);
                }
            }
        }
    }

    public override void findValidSpaces(int move, Coord currentSpace, int callNo, bool attack) {
        //Set this ally as the selectedAlly on first call to attack(), not counterattack (2nd call)
        if(!attack) {
            GameManager.instance.setSelectedAlly(this);
        }

        base.findValidSpaces(move, currentSpace, callNo, attack);
    }

    public override void moveUnit(Coord destination) {
        base.moveUnit(destination);

        //Open the after-move options after moving this ally
        InGameMenus menus = GameObject.Find("GameManager").GetComponent<InGameMenus>();
        menus.openAfterMoveOptions();
    }

    public override void defeated() {
        base.defeated();
        GameManager.instance.allyUnits.Remove(this.gameObject);
        GameManager.instance.allies.Remove(this);

        //Check if all allies are defeated, if so enemy team wins
        if(GameManager.instance.allyUnits.Count == 0) {
            SceneManager.LoadScene("End");
        }
    }
}