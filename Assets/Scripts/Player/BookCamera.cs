using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookCamera : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    BookController bookController;

    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        bookController = FindObjectOfType<BookController>();
        bookController.onBookEvent.AddListener(OnBookEvent);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (bookController != null)
        {
            bookController.onBookEvent.RemoveListener(OnBookEvent);
        }
    }

    private void OnBookEvent()
    {
        if (bookController.HasBook)
            Invoke("EnableDelayed", 0.4f);
        else
        {
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }

    void EnableDelayed()
    {
        gameObject.SetActive(true);
    }
}
