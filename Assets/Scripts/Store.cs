using Firebase.Extensions;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{

    int coins = 1000;
    public Slider slider;
    Sprite mySprite;
    GameObject background;
    private static float byteTransferred;
    private static float byteCount;


    private void Start()
    {
        background = new GameObject();
        background.AddComponent<SpriteRenderer>();
    }
    // Update is called once per frame
   
    public void addCoins()
    {
        coins += 500;
        GameObject.Find("WalletText").GetComponent<Text>().text = "Wallet: " + coins + " coins";
    }

    public void Item1Button()
    {
        if(coins >= 250)
        {
            coins -= 250;
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            StorageReference sr = storage.GetReferenceFromUrl("gs://cgassignment2-de6ca.appspot.com");
            StorageReference reference = sr.Child("Lake.jpg");
            GameObject.Find("WalletText").GetComponent<Text>().text = "Wallet: " + coins+ " coins";
            GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().addPurchases("Lake");


            downloadBackgorundImages(reference);
        }
    }
    public void Item2Button()
    {
        if (coins >= 500)
        {
            coins -= 500;
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            StorageReference sr = storage.GetReferenceFromUrl("gs://cgassignment2-de6ca.appspot.com");
            StorageReference reference = sr.Child("tower.jpg");
            GameObject.Find("WalletText").GetComponent<Text>().text = "Wallet: " + coins + " coins";
            GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().addPurchases("Tower");


            downloadBackgorundImages(reference);
        }
    }

    public void Item3Button()
    {
        if (coins >= 750)
        {
            coins -= 750;
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            StorageReference sr = storage.GetReferenceFromUrl("gs://cgassignment2-de6ca.appspot.com");
            StorageReference reference = sr.Child("looking.jpg");
            GameObject.Find("WalletText").GetComponent<Text>().text = "Wallet: " + coins + " coins";
            GameObject.Find("FirebaseConfig").GetComponent<FirebaseConfig>().addPurchases("Looking");


            downloadBackgorundImages(reference);
        }
    }


    private void downloadBackgorundImages(StorageReference reference)
    {
        const long maxAllowedSize = 1 * 5096 * 5096;
        reference.GetBytesAsync(maxAllowedSize, new StorageProgress<DownloadState>(state =>
        {
            byteTransferred = state.BytesTransferred;
            byteCount = state.TotalByteCount;


            slider.value = ((byteTransferred / byteCount) * 100);


            Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.",
                     state.BytesTransferred,
                     state.TotalByteCount
                    ));

        })).ContinueWithOnMainThread(task => {


            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContent = task.Result;
                slider.value = 0;
                Debug.Log("Finished!");
                // Load the image into Unity

                //Create Texture
                Texture2D texture = new Texture2D(1024, 1024);
                texture.LoadImage(fileContent);

                //Create Sprite
                mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                background.GetComponent<SpriteRenderer>().sprite = mySprite;
                background.name = "Background";
                background.transform.position = new Vector3(0f, 0f, 1f);
                //background.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
            }
        });
    }
}
