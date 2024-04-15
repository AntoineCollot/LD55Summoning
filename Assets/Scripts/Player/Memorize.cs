using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memorize : MonoBehaviour
{
    public LanguageRune runePrefab;
    public Transform layout;
    public GameObject memorizePrompt;
    List<LanguageRune> spawnedRunes = new List<LanguageRune>();
    BookController bookController;

    InputMap inputs;

    // Start is called before the first frame update
    void Start()
    {
        bookController = FindObjectOfType<BookController>();
        bookController.onBookEvent.AddListener(OnBookEvent);

        inputs = new InputMap();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Memorize.performed += OnMemorize;
    }

    private void OnBookEvent()
    {
        memorizePrompt.SetActive(bookController.HasBook && bookController.pickedUpBook.rune != null);
    }

    private void OnMemorize(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (bookController.HasBook)
        {
            MemorizeRune(bookController.pickedUpBook.rune);
        }
    }

    public void MemorizeRune(ScriptableRune rune)
    {
        if (rune == null)
            return;

        LanguageRune newRune = Instantiate(runePrefab, layout);
        newRune.transform.SetAsFirstSibling();
        newRune.Draw(rune.runeParts);
        spawnedRunes.Add(newRune);

        if (spawnedRunes.Count > 5)
        {
            Destroy(spawnedRunes[0].gameObject);
            spawnedRunes.RemoveAt(0);
        }
    }
}
