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
   


    // Start is called before the first frame update
    void Start()
    {
        
        GameObject.Find("GameManager").GetComponent<FirebaseConfig>().uploadTextFileData();
    }

   
}