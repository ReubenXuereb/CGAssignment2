using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;


public class DLC_Script : MonoBehaviour
{
    public Sprite square;
    public Sprite circle;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://connected-gaming-hassign1.appspot.com/DLC");
        StorageReference square = storageRef.Child("Square.png");
        StorageReference circle = storageRef.Child("Circle.png");
        DownloadImageP1(square);
        DownloadImageP2(circle);
    }

    private void DownloadImageP1(StorageReference reference)
    {
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 1 * 1024 * 1024;
        reference.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;
                Debug.Log("Finished downloading!");
                //Convert the byte image into sprite
                //load the image into Unity
                //Create Texture

                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(fileContents);

                square = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            }
        });

    }


    public void DownloadImageP2(StorageReference reference)
    {
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 1 * 1024 * 1024;
        reference.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;
                Debug.Log("Finished downloading!");
                //Convert the byte image into sprite
                //load the image into Unity
                //Create Texture

                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(fileContents);

                circle = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            }
        });
    }

}
