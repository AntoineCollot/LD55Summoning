using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageRune : MonoBehaviour
{
    Image vBase;
    [SerializeField] Image[] lines;
    LanguageAlphabet alphabet;

    [SerializeField] List<RunePart> defaultRune;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
        Draw(defaultRune);
    }

    void Init()
    {
        vBase = GetComponent<Image>();
        alphabet = Resources.Load<LanguageAlphabet>("Alphabet");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(!Application.isPlaying)
            Draw(defaultRune);
    }
#endif

    public void Draw(List<RunePart> runeParts)
    {
        Clear();

        gameObject.SetActive(true);
        if (!LanguageAlphabet.IsRuneValid(runeParts))
        {
            Debug.Log("<color=#FF4444>Rune isn't valid</color>");
            return;
        }

        vBase.enabled = true;
        //Base
        int runeID = 0;
        vBase.sprite = alphabet.GetBase(runeParts[runeID], runeParts.Count>1);
        if (runeParts[runeID] == RunePart.Down)
            runeID++;
        runeID++;

        //Lines
        int lineID = 0;
        RunePart previousRune = RunePart.Right;
        while (runeID < runeParts.Count)
        {
            LanguageAlphabet.ParseRune(runeParts, runeID, out RunePart displayPart, out int repeat);

            lines[lineID].enabled = true;
            lines[lineID].sprite = alphabet.GetLine(displayPart, repeat, previousRune);

            //Move next
            previousRune = displayPart;
            lineID++;
            runeID++;
            //Do not process the repeats
            runeID += repeat;

            //Check if end draw (can happen because of down input)
            if (lineID >= lines.Length)
                break;
        }
    }

    public void Clear()
    {
        if (vBase == null || alphabet == null)
            Init();
        vBase.enabled = false;
        foreach (Image line in lines)
        {
            line.enabled = false;
        }
        gameObject.SetActive(false);
    }
}

public enum RunePart { Up, Right, Down, Left }
