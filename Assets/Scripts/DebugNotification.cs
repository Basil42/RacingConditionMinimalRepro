using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNotification : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("addressable scene object awake");
    }
    void Start()
    {
        Debug.Log("addresaable scvene object started");
    }

    bool hasNotified = false;
    // Update is called once per frame
    void Update()
    {
        if (!hasNotified)
        {
            Debug.Log("Update called on the notifier in the addressable scene.");
            hasNotified = true;
        }
    }
}
