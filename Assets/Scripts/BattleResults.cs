using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleResults : MonoBehaviour {

    //These variables will be given values in the Unity editor
    public GameObject winScreen, loseScreen, player1Win, player2Win;

    //Show the correct end screen depending on the mode the game is in
    //and which team won
    public void showEndScreen(bool twoPlayer, string winningTeam) {
        if(!twoPlayer) {
            if(winningTeam == "Ally") {
                winScreen.SetActive(true);
            }else if(winningTeam == "Enemy") {
                loseScreen.SetActive(true);
            }
        }else {
            if(winningTeam == "Ally") {
                player1Win.SetActive(true);
            }else if(winningTeam == "Enemy") {
                player2Win.SetActive(true);
            }
        }
    }

    //Destroy everything on the map and restart the game
    public void playAgainBtnClicked() {
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");

        for(int i = 0; i < allTiles.Length ; i++) {
            Destroy(allTiles[i]);
        }
        for(int i = 0; i < allUnits.Length; i++) {
            Destroy(allUnits[i]);
        }

        //Load the start menu
        SceneManager.LoadScene(0);
    }

    public void quitBtnClicked() {
        Application.Quit();
    }

	// Use this for initialization
	void Start () {
        //When this scene is loaded, decide which team won and show appropriate screen
        bool twoPlayer = GameManager.twoPlayer;
        string winningTeam = "";
        if(GameManager.instance.allyUnits.Count == 0) {
            winningTeam = "Enemy";
        } else if(GameManager.instance.enemyUnits.Count == 0) {
            winningTeam = "Ally";
        }

        if(winningTeam == "") {
            showEndScreen(twoPlayer, "Ally");
        }
    }
}