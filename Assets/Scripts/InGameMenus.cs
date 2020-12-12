using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InGameMenus : MonoBehaviour {

    [SerializeField]
    GameObject afterMoveOptions;

    [SerializeField]
    GameObject unitStats;
    public static bool unitsUnclickable;

    public Text[] statText = new Text[8];

    Ally selectedAlly;
    Enemy selectedEnemy;

    public static bool attacking;

    Unit selected;
    List<Unit> allUnits = new List<Unit>();

    public void waitBtnClicked() {
        if(afterMoveOptions.activeInHierarchy) {
            afterMoveOptions.SetActive(false);
            unitsUnclickable = false;

            bool twoPlayer = GameManager.twoPlayer;
            bool currentTurn = GameManager.getCurrentTurn();

            //Make the wait button apply to the current team being controlled
            if(!twoPlayer || (twoPlayer && currentTurn)) {
                selectedAlly = GameManager.instance.selectedAlly;
                Unit.movedAllies.Add(selectedAlly);
                currentTurn = GameManager.instance.checkTurnEnd();
            } else if(twoPlayer && !currentTurn) {
                selectedEnemy = GameManager.instance.selectedEnemy;
                Unit.movedEnemies.Add(selectedEnemy);
                currentTurn = GameManager.instance.checkTurnEnd();
            }

            //Execute enemy AI if it's enemy turn
            if(!twoPlayer && !currentTurn) {
                for(int i = 0; i < GameManager.instance.enemies.Count; i++) {
                    GameManager.instance.enemies[i].AI();
                }
            }
        }
    }

    public void attackBtnClicked() {
        if(afterMoveOptions.activeInHierarchy) {
            attacking = true;
            afterMoveOptions.SetActive(false);
            unitsUnclickable = false;

            bool twoPlayer = GameManager.twoPlayer;
            bool currentTurn = GameManager.getCurrentTurn();

            //Make the attack button apply to the team being controlled
            if(!twoPlayer || (twoPlayer && currentTurn)) {
                selectedAlly = GameManager.instance.selectedAlly;
                Unit.movedAllies.Add(selectedAlly);
                selectedAlly.selectTarget();
            }else if(twoPlayer && !currentTurn) {
                selectedEnemy = GameManager.instance.selectedEnemy;
                Unit.movedEnemies.Add(selectedEnemy);
                selectedEnemy.selectTarget();
            }
        }
    }

    public void openAfterMoveOptions() {
        if(unitStats.activeInHierarchy) {
            unitStats.SetActive(false);
        }
        if(!afterMoveOptions.activeInHierarchy) {
            unitsUnclickable = true;
            afterMoveOptions.SetActive(true);
        }
    }

    public void openUnitStats() {
        if(!afterMoveOptions.activeInHierarchy) {
            Unit selected = Unit.selected;
            showStats(selected);
        }
    }

    //Helper function to show the stats of the current unit
    void showStats(Unit selected) {
        if(selected != null) {
            string[] selectedStats;
            selectedStats = selected.getStats();
            unitStats.SetActive(true);

            //Make stats text correct
            for(int i = 0; i < statText.Length; i++) {
                statText[i].text = selectedStats[i].ToString();
            }

            //Set the team text to blue if ally or red if enemy
            statText[6].color = statText[6].text == "Ally" ? Color.blue : Color.red;
        }
    }

    public void previousUnit() {
        int selectedIndex = allUnits.IndexOf(selected);
        selected = allUnits[wrap(selectedIndex - 1, allUnits.Count)];
        showStats(selected);
    }

    public void nextUnit() {
        int selectedIndex = allUnits.IndexOf(selected);
        selected = allUnits[wrap(selectedIndex + 1, allUnits.Count)];
        showStats(selected);
    }

    //Helper function to make indexes wrap around if < 0 or > length
    int wrap(int value, int length) {
        if(value < 0) {
            return length - Mathf.Abs(value);
        } else {
            return value % length;
        }

    }

    public void closeUnitStats() {
        unitStats.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        List<Ally> allies = GameManager.instance.allies;
        List<Enemy> enemies = GameManager.instance.enemies;

        //Fill unitStats list with each unit's stats (allies and enemies)
        for(int i = 0; i < allies.Count; i++) {
            allUnits.Add(allies[i]);
        }
        for(int i = 0; i < enemies.Count; i++) {
            allUnits.Add(enemies[i]);
        }

        selected = Unit.selected;
    }
	
	// Update is called once per frame
	void Update () {
        //Keyboard shortcuts
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if(!unitStats.activeInHierarchy) {
                openUnitStats();
            } else {
                closeUnitStats();
            }
        }

        if(Input.GetKeyDown(KeyCode.Q)) {
            if(unitStats.activeInHierarchy) {
                previousUnit();
            }
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            if(unitStats.activeInHierarchy) {
                nextUnit();
            }
        }

        if(Input.GetKeyDown(KeyCode.X)) {
            if(afterMoveOptions.activeInHierarchy) {
                attackBtnClicked();
            }
        }
        if(Input.GetKeyDown(KeyCode.Z)) {
            if(afterMoveOptions.activeInHierarchy) {
                waitBtnClicked();
            }
        }
	}
}