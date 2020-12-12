using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    //Variables to be given values in Unity's inspector
    public GameObject basicOptions, advancedOptions;
    public Button generateMapBtn;
    public Text widthVal, heightVal, mountVal, forestVal;

    bool sizeChosen, mountChosen, forestChosen;
    public static bool twoPlayer { get; private set; }
    public static int width { get; private set; }
    public static int height { get; private set; }
    public static int mountPercent { get; private set; }
    public static int forestPercent { get; private set; }

    //Preset values for generation paramaters
    readonly int wSmall = 18, wMed = 36, wLarge = 54,
                 hSmall = 10, hMed = 20, hLarge = 30,
                 mountLow = 25, mountMed = 35, mountHigh = 45,
                 forestLow = 10, forestMed = 20, forestHigh = 30;

    //Basic menu functions
    //set map generation parameters to specific values based on the option chosen
    public void setMapSize(string choice) {
        if(choice != null) sizeChosen = true;
        switch(choice) {
            case "small":
                width = wSmall;
                height = hSmall;
                break;
            case "medium":
                width = wMed;
                height = hMed;
                break;
            case "large":
                width = wLarge;
                height = hLarge;
                break;
        }
        enableGenerate(); //check if one option in each set has been chosen
    }

    public void setMountainAmount(string choice) {
        if(choice != null) mountChosen = true;
        switch(choice) {
            case "low":
                mountPercent = mountLow;
                break;
            case "medium":
                mountPercent = mountMed;
                break;
            case "high":
                mountPercent = mountHigh;
                break;
        }
        enableGenerate(); //check if one option in each set has been chosen
    }

    public void setForestAmount(string choice) {
        if(choice != null) forestChosen = true;
        switch(choice) {
            case "low":
                forestPercent = forestLow;
                break;
            case "medium":
                forestPercent = forestMed;
                break;
            case "high":
                forestPercent = forestHigh;
                break;
        }
        enableGenerate(); //check if one option in each set has been chosen
    }

    void enableGenerate() {
        if(basicOptions.activeInHierarchy) { //check if the basic menu is currently active
            if(sizeChosen && mountChosen && forestChosen) {
                //make the generate map button clickable if one option in each set is chosen
                generateMapBtn.interactable = true;
            }
        }
    }

    public void goToAdvacedOptions() {
        basicOptions.SetActive(false);
        advancedOptions.SetActive(true);
    }

    public void widthBarChanged(float value) {
        width = (int) value;
        widthVal.text = width.ToString();
    }

    public void heightBarChanged(float value) {
        height = (int) value;
        heightVal.text = height.ToString();
    }

    public void mountBarChanged(float value) {
        mountPercent = (int) value;
        mountVal.text = mountPercent.ToString();
    }

    public void forestBarChanged(float value) {
        forestPercent = (int) value;
        forestVal.text = forestPercent.ToString();
    }

    public void goToBasicOptions() {
        advancedOptions.SetActive(false);
        basicOptions.SetActive(true);
    }

    //Functions for both menus
    public void twoPlayerMode() {
        twoPlayer = true;
    }

    public void generateBtnClicked() {
        SceneManager.LoadScene("Game"); //start the game!
    }

    public void showHelp() {
        string helpFile = "/Speedrun Strategy Instructions.htm";
        Application.OpenURL("C:/Users/Robert/Documents/Unity/Speedrun Strategy" + helpFile);
    }

    public void quit() {
        Application.Quit();
    }
}
