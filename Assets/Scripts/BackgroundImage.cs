using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Storage;

public class BackgroundImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] imageNames = { "bluemoon.png", "montains.png", "sea.jpg" };

        int randomBackground = Random.Range(0, 3);
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference sr = storage.GetReferenceFromUrl("gs://cgassignment2-de6ca.appspot.com");
        StorageReference img = sr.Child(imageNames[randomBackground]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
