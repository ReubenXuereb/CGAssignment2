using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Extensions;
using Firebase.Database;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField roomeCodeText;

    [SerializeField] Button startGameBtn;

    [SerializeField] TMP_Text player2text;
    [SerializeField] public Sprite[] showSpriteP1;
    [SerializeField] public Sprite[] showSpriteP2;
    [SerializeField] GameObject rockButton;
    [SerializeField] GameObject paperButton;
    [SerializeField] GameObject scissorsButton;
    public SpriteRenderer spriteRefP1;
    public SpriteRenderer spriteRefP2;
    public string p1Choice;
    public string p2Choice;
    public bool switchButtonsOnOff;
    public int playerWinner;
    public int rounds = 5;
    public float timer;
    
    GameObject Player1;
    GameObject Player2;

    //DLC_Script DLC;

    FirebaseConfig firebseConfig;

    public void Awake() 
    {
        if (SceneManager.GetActiveScene().name == "RoomLobby")
        {
            roomeCodeText.text = FirebaseConfig.roomKey;
            GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().player2JoinedListener();
        }
        if(SceneManager.GetActiveScene().name == "Game")
        {
            GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().updatePlayerListener();
        }
    }

    void Start() 
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            Player1 = GameObject.Find("Player1");
            Player2 = GameObject.Find("Player2");
            rockButton = GameObject.Find("RockButton");
            paperButton = GameObject.Find("PaperButton");
            scissorsButton = GameObject.Find("ScissorsButton");
        }
        if(SceneManager.GetActiveScene().name == "GameOver")
        {
            getPlayerData();
           
        }
    }
    public void getPlayerData()
    {
        FirebaseConfig.myDB.Child("Rooms").Child(FirebaseConfig.roomKey).Child("objects").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                print("something went wrong");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Player p1 = new Player(snapshot.Child("_player1").Child("playerName").Value.ToString(), "", "", int.Parse(snapshot.Child("_player1").Child("playerName").Value.ToString()), 0);
                Player p2 = new Player(snapshot.Child("_player2").Child("playerName").Value.ToString(), "", "", 0, 0);
                //Player p2Moves = new Player("", "", "", 0, snapshot.Child("_player2").Child("moves").Value.ToString());

                Objects playerData = new Objects(p1, p2);
                print("p1 name: " + playerData._player1.playerName);
                print("p2 name: " + playerData._player2.playerName);

                Debug.Log(playerData._player1.score);
                Debug.Log(playerData._player2.score);
                if (playerData._player1.score > playerData._player2.score)
                {

                    GameObject.Find("WinnerText").GetComponent<Text>().text = "Winner is: " + playerData._player1.playerName;
                    print("hdimt");
                }
                else if (playerData._player2.score > playerData._player1.score)
                {

                    GameObject.Find("WinnerText").GetComponent<Text>().text = "Winner is: " + playerData._player2.playerName;
                }
                else if (playerData._player1.score == playerData._player2.score)
                {

                    GameObject.Find("WinnerText").GetComponent<Text>().text = "Draw";
                }
                else
                {
                    Debug.Log("gol else:  " +playerData._player1.score);
                }
            }
        });
    }
    public void showPlayButton() {

        if (SceneManager.GetActiveScene().name == "RoomLobby")
        {
            if (FirebaseConfig.mainPlayer)
            {
                startGameBtn.gameObject.SetActive(true);
            }

            if (FirebaseConfig.player2joined) 
            {
                player2text.text = "Player 2";
            }
        }
    }

    public void StartGame() 
    {
        StartCoroutine(GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().StartGame());
    }

   
    public void buttonClicked(string rps)
    {
        switchButtonsOnOff = false;
        GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().setPlayerChoice(rps);
    }


    public void switchRPSButtons(bool condition)
    {
        rockButton.SetActive(condition);
        paperButton.SetActive(condition);
        scissorsButton.SetActive(condition);
    }

    public int rpsLogic(string p1Choice, string p2Choice)
    {
        if(p1Choice == "Rock" && p2Choice == "Paper")
        {
            playerWinner = 2;
        }
        if(p1Choice == "Rock" && p2Choice == "Scissors")
        {
            playerWinner = 1;
        }
        if (p1Choice == "Paper" && p2Choice == "Scissors")
        {
            playerWinner = 2;
        }
        if (p1Choice == "Paper" && p2Choice == "Rock")
        {
            playerWinner = 1;
        }
        if (p1Choice == "Scissors" && p2Choice == "Paper")
        {
            playerWinner = 1;
        }
        if (p1Choice == "Scissors" && p2Choice == "Rock")
        {
            playerWinner = 2;
        }
        if (p1Choice == p2Choice)
        {
            playerWinner = 0;
        }
        return playerWinner;
    }

    public void p1ScoreText(int score)
    {
        GameObject.Find("P1Score").GetComponent<Text>().text = "P1: " + score;
    } 
    public void p2ScoreText(int score)
    {
        GameObject.Find("P2Score").GetComponent<Text>().text = "P2: " + score;
    }

    public void roundText(int round)
    {
        GameObject.Find("CurrentRoundText").GetComponent<Text>().text = "Round: " + round;
    }

    public void changeSprite(string p1Choice, string p2Choice)
    {
        if (FirebaseConfig.mainPlayer)
        {
            if (p1Choice == "Rock")
            {
                GameObject.Find("Player1").GetComponent<SpriteRenderer>().sprite = showSpriteP1[0];
            }
            if (p1Choice == "Paper")
            {
                GameObject.Find("Player1").GetComponent<SpriteRenderer>().sprite = showSpriteP1[1];
            }
            if (p1Choice == "Scissors")
            {
                GameObject.Find("Player1").GetComponent<SpriteRenderer>().sprite = showSpriteP1[2];
            }
        }
        else if(!FirebaseConfig.mainPlayer)
        {
            if (p2Choice == "Rock")
            {
                GameObject.Find("Player2").GetComponent<SpriteRenderer>().sprite = showSpriteP2[0];
            }
            if (p2Choice == "Paper")
            {
                GameObject.Find("Player2").GetComponent<SpriteRenderer>().sprite = showSpriteP2[1];
            }
            if (p2Choice == "Scissors")
            {
                GameObject.Find("Player2").GetComponent<SpriteRenderer>().sprite = showSpriteP2[2];
            }
        }
    }

    public void storeButton()
    {
        SceneManager.LoadScene("Store");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("Welcome");
    }

    public void PlayAgain()
    {
        Destroy(GameObject.Find("FirebaseConfig"));
        SceneManager.LoadScene("Welcome");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
