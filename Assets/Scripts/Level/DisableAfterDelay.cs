using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    public float delay;

    void OnEnable()
    {
        Invoke("DisableDelayed", delay);
    }

    void DisableDelayed()
    {
        gameObject.SetActive(false);

    }
}
