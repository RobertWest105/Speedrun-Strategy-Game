using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PauseMenu : MonoBehaviour {

    //These varables will be given values in the Unity editor
    public GameObject menu;
    public GameObject quitConfirmation;
    public Button loadBtn;

    SaveLoad saveGame;

    // Update is called once per frame
    void Update() {
        //Open pause menu with 1
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            if(!menu.activeInHierarchy) {
                menu.SetActive(true);
            } else menu.SetActive(false);
        }

        //If save files exist, they can be loaded
        if(File.Exists(Application.dataPath + "SpeedrunStrategyUnits.sav")) {
            if(File.Exists(Application.dataPath + "SpeedrunStrategyMap.sav")) {
                loadBtn.interactable = true;
            }
        }
    }

    public void save() {
        //Ensure only one saved game is present at a time
        if(File.Exists(Application.dataPath + "SpeedrunStrategyUnits.sav")) {
            File.Delete(Application.dataPath + "SpeedrunStrategyUnits.sav");
        }
        if(File.Exists(Application.dataPath + "SpeedrunStrategyMap.sav")) {
            File.Delete(Application.dataPath + "SpeedrunStrategyMap.sav");
        }

        //Save the current state of the game
        saveGame = new SaveLoad();
        saveGame.save();
        loadBtn.interactable = true;
    }

    public void load() {
        //Load the last saved game
        loadBtn.interactable = false;
        saveGame = new SaveLoad();
        saveGame.load();
    }

    public void endTurn() {
        menu.SetActive(false);
        if(!GameManager.twoPlayer) {
            //Start enemy turn early
            for(int i = 0; i < GameManager.instance.enemies.Count; i++) {
                GameManager.instance.enemies[i].AI();
            }
        }else {
            GameManager.setCurrentTurn(!GameManager.getCurrentTurn());
        }
        Unit.movedAllies.Clear();
    }

    public void showHelp() {
        string helpFile = "/Speedrun Strategy Instructions.htm";
        Application.OpenURL("C:/Users/Robert/Documents/Unity/Speedrun Strategy" + helpFile);
    }

    public void showQuitConfirmation() {
        menu.SetActive(false);
        quitConfirmation.SetActive(true);
    }

    public void returnToMenu() {
        quitConfirmation.SetActive(false);
        menu.SetActive(true);
    }

    public void quit() {
        Application.Quit();
    }

    public void returnToGame() {
        menu.SetActive(false);
    }
}