using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Player 
{
    public string playerName;
    public string rps;
    public string uniqueID;
    public float score;

    public Player(string playerName, string rps, string uniqueID, float score)
    {
        this.playerName = playerName;
        this.rps = rps;
        this.uniqueID = uniqueID;
        this.score = 0;
    }
}

[Serializable]
public class Objects
{
    public Player _player1;
    public Player _player2;

    public Objects(Player player1, Player player2)
    {
        _player1 = player1;
        _player2 = player2;
    }
}

[Serializable]
public class GameRoom
{
    public Objects objects;
    public bool gameHasStarted;
    public bool gameWon;
    public string winner;
    public int currentRound;
    public bool isRoundOver;

    public GameRoom(Objects objects, bool gameHasStarted, bool gameWon, string winner, int currentRound, bool isRoundOver)
    {
        this.objects = objects;
        this.gameHasStarted = gameHasStarted;
        this.gameWon = gameWon;
        this.winner = winner;
        this.currentRound = currentRound;
        this.isRoundOver = isRoundOver;
    }
}

public class FirebaseConfig : MonoBehaviour
{
    [SerializeField] TMP_InputField joinRoomCode;

    public Player player1;
    public Player player2;

    public static Vector3 connectedPlayerPos;

    public static String roomKey;
    public static bool mainPlayer;
    public static bool gameStarted;
    public static bool joinPlayer;
    public static bool player2joined;
    public int currentRound;
    public static int P1score;
    public static int P2Score;
    public static string winner;
    public bool doneOnce;

    public static DatabaseReference myDB;
    // Start is called before the first frame update
    void Start()
    {
        mainPlayer = false;
        joinPlayer = false;
        player2joined = false;
        myDB = FirebaseDatabase.DefaultInstance.RootReference;
        DontDestroyOnLoad(this.gameObject);
        
    }

    public void createGame() 
    {
        mainPlayer = true;
        
        StartCoroutine(CreateRoom());
    }
    public void joinRoom()
    {
        joinPlayer = true;
        player2joined = true;
        StartCoroutine(JoinRoom(joinRoomCode.text));
        gameHasStartedListener(joinRoomCode.text);
    }
    public IEnumerator CreateRoom() 
    {
        string key = myDB.Child("Rooms").Child("Objects").Push().Key;
        player1 = new Player("Player1", "", key, 0);
        Objects objects = new Objects(player1, null);

        GameRoom room = new GameRoom(objects, false, false, "", 0, false);
        string json = JsonUtility.ToJson(room);
        //print(json);

        roomKey = myDB.Child("Rooms").Push().Key;
        print(roomKey);
        yield return myDB.Child("Rooms").Child(roomKey).SetRawJsonValueAsync(json);
        SceneManager.LoadScene("RoomLobby");
    }

    public IEnumerator JoinRoom(string roomCode) 
    {
        string key = myDB.Child("Rooms").Child("Objects").Push().Key;
        player2 = new Player("Player2", "", key, 0);
        string json = JsonUtility.ToJson(player2);
        yield return myDB.Child("Rooms").Child(roomCode).Child("objects").Child("_player2").SetRawJsonValueAsync(json);
        roomKey = roomCode;
        SceneManager.LoadScene("RoomLobby");
    }

    public IEnumerator StartGame() 
    {
        yield return myDB.Child("Rooms").Child(roomKey).Child("gameHasStarted").SetValueAsync(true);
        print("gameHasStarted set to True");
        SceneManager.LoadScene("Game");
    }
    public void gameHasStartedListener(string roomcode)
    {
        //print(roomcode);
        myDB.Child("Rooms").Child(roomcode).Child("gameHasStarted").ValueChanged += handleGameStarted;
    }

    void handleGameStarted(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        gameStarted = (bool)args.Snapshot.Value;
        print(gameStarted);
        if (gameStarted)
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void player2JoinedListener()
    {

        myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player2").Child("playerName").ValueChanged += handlePlayer2Joined;
    }

    static void handlePlayer2Joined(object sender, ValueChangedEventArgs args) 
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Value.ToString() != "")
        {
            player2joined = true;
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.showPlayButton();
        }
    }

   
    public void setPlayerChoice(string choice)
    {
        if (mainPlayer)
        {
            myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player1").Child("rps").SetValueAsync(choice);
            print("jien qed nahdem ta p1");
        }
        else
        {
            myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player2").Child("rps").SetValueAsync(choice);
            print("jien qed nahdem ta p2");
        }
    }

    public void updatePlayerListener()
    {
        if (mainPlayer)
        {
            myDB.Child("Rooms").Child(roomKey).Child("objects").ValueChanged += handlePlayerChoice;
            print("detected change");
        }
        else
        {
            myDB.Child("Rooms").Child(roomKey).Child("objects").ValueChanged += handlePlayerChoice;
            print("detected change p2");

        }
        myDB.Child("Rooms").Child(roomKey).Child("isRoundOver").ValueChanged += handleIfRoundOver;
        myDB.Child("Rooms").Child(roomKey).Child("currentRound").ValueChanged += handleRounds;
        myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player1").Child("score").ValueChanged += handleP1Score;
        myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player2").Child("score").ValueChanged += handleP2Score;
    }

    void handleP1Score(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        GameObject.Find("GameManager").GetComponent<GameManager>().p1ScoreText(int.Parse(args.Snapshot.Value.ToString()));
    }

    void handleP2Score(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        GameObject.Find("GameManager").GetComponent<GameManager>().p2ScoreText(int.Parse(args.Snapshot.Value.ToString()));
    }

    void handleRounds(object sender, ValueChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if(int.Parse(args.Snapshot.Value.ToString()) > GameObject.Find("GameManager").GetComponent<GameManager>().rounds)
        {
            if(player1.score > player2.score)
            {
                winner = player1.playerName;
            }
            if (player2.score > player1.score)
            {
                winner = player2.playerName;
            }
            if (player2.score == player1.score)
            {
                winner = "Draw";
            }
            SceneManager.LoadScene("GameOver");
        }
    }

    void handleIfRoundOver(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if ((bool)args.Snapshot.Value)
        {
            doneOnce = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().switchRPSButtons(true);
            myDB.Child("Rooms").Child(roomKey).Child("isRoundOver").SetValueAsync(false);
        }
    }

    void handlePlayerChoice(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string p1Choice = args.Snapshot.Child("_player1").Child("rps").Value.ToString();
        string p2Choice = args.Snapshot.Child("_player2").Child("rps").Value.ToString();
        //GameObject whoWonText = GameObject.Find("WhoWonRoundText");
       // whoWonText.SetActive(false);

        if (p1Choice != "" && p2Choice != "" && mainPlayer && !doneOnce)
        {
            doneOnce = true;
            print("both players have done a move");
            int winner = GameObject.Find("GameManager").GetComponent<GameManager>().rpsLogic(p1Choice, p2Choice);
           
            if (winner == 1)
            {
                player1.score++;
                myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player1").Child("score").SetValueAsync(player1.score);
               //whoWonText.SetActive(true);
                //GameObject.Find("WhoWonRoundText").GetComponent<Text>().text = "Player 1 won this round";
            }
            if (winner == 2)
            {
                player2.score++;
                myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player2").Child("score").SetValueAsync(player2.score);
                print("thalkt go p2");
                //whoWonText.SetActive(true);
               //GameObject.Find("WhoWonRoundText").GetComponent<Text>().text = "Player 1 won this round";
            }
            if (winner == 0)
            {
                print("Draw");
                //whoWonText.SetActive(true);
               // GameObject.Find("WhoWonRoundText").GetComponent<Text>().text = "Draw";
            }
            currentRound++;
            //whoWonText.SetActive(false);
            myDB.Child("Rooms").Child(roomKey).Child("currentRound").SetValueAsync(currentRound);
            myDB.Child("Rooms").Child(roomKey).Child("isRoundOver").SetValueAsync(true);
            myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player1").Child("rps").SetValueAsync("");
            myDB.Child("Rooms").Child(roomKey).Child("objects").Child("_player2").Child("rps").SetValueAsync("");
        }
    }
  
}
