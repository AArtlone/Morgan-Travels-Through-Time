using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class JsonManager : MonoBehaviour
{
    public static JsonManager Instance;

    private void Awake()
    {
        // If we have an instance of this singleton somewhere else
        // in the game, then we want to destroy the existing one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            // We want to be able to access the game information from any scene
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        
    }
}