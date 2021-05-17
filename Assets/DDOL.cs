// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public void Awake()
    {
        // Make sure the game object this game manger is on , stays when new scenens are loaded.
        DontDestroyOnLoad(gameObject);
        Debug.Log("DontDestroyOnLoad(gameObject); Exectuted.");
    }
}
