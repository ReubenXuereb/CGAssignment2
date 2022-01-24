using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Firebase.Storage;
using Firebase.Extensions;
using System.Threading.Tasks;

public class TextFileUploader : MonoBehaviour
{
    //private int _score = 27;
    private float _duration = 87.5f;
    private string _date = "14/12/2021 08:55";

    private string _matchId;
    private string _winner;
    private int _p1Moves;
    private int _p2Moves;


    // Start is called before the first frame update
    void Start()
    {
        _matchId = FirebaseConfig.roomKey;
        _winner = FirebaseConfig.winner;
        _p1Moves = FirebaseConfig.p1Moves;
        _p2Moves = FirebaseConfig.p2Moves;
       // _score = 
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.RootReference;
        string DataString = "MatchID: " + _matchId.ToString() + "\nWinner: " + _winner.ToString() + "\nP1 moves: " + _p1Moves.ToString() + "\nP2 moves: " + _p2Moves.ToString();
        Debug.Log(DataString);
        byte[] data = Encoding.ASCII.GetBytes(DataString);
        StartCoroutine(UploadTextFile(data, storageRef));
    }

    private IEnumerator UploadTextFile(byte[] data, StorageReference reference)
    {
        
        StorageReference textFileRef = reference.Child("Text files").Child("test.txt");

        
        yield return textFileRef.PutBytesAsync(data)
    .ContinueWithOnMainThread((task) => {
        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log(task.Exception.ToString());
           
        }
        else
        {
            StorageMetadata metadata = task.Result;
            string md5Hash = metadata.Md5Hash;
            Debug.Log("Finished uploading...");
            //Debug.Log("md5 hash = " + md5Hash);
        }
    });
    }
}