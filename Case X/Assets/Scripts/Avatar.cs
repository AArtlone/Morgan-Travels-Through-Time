using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public static Avatar Instance;
    public int MovementSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
