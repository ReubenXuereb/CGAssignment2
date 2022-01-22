using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Storage;

public class BackgroundImage: MonoBehaviour
{
    void Start()
    {
       string[] imageNames = { "bluemoon.png", "Lake.jpg", "looking.jpg", "north.jpg", "tower.jpg"};

        int randombackground = Random.Range(0, 3);
        
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storeRef = storage.GetReferenceFromUrl("gs://cgassignment2-de6ca.appspot.com");
        StorageReference img = storeRef.Child(imageNames[randombackground]);

        GameObject background = new GameObject();
        background.transform.parent = GameObject.Find("BackgroundImage").transform;
        background.AddComponent<SpriteRenderer>();

        StartCoroutine(downloadBackgorundImages(img, background));
    }

    IEnumerator downloadBackgorundImages(StorageReference reference, GameObject backgroundImage)
    {
        yield return new WaitForSeconds(0f);
        const long maxSize = 1 * 5096 * 5096;
        reference.GetBytesAsync(maxSize).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
            }
            else
            {
                byte[] fileContent = task.Result;
                Texture2D texture = new Texture2D(1024, 1024);
                texture.LoadImage(fileContent);
                Sprite mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                backgroundImage.GetComponent<SpriteRenderer>().sprite = mySprite;
                backgroundImage.name = "Background";
                backgroundImage.transform.position = new Vector3(0f, 0f, 1f);
                //go.transform.localScale = new Vector3(1f, 2.2f, 0f);
            }
        });
    }
}