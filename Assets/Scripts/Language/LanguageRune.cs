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
    void Start()
    {
        vBase = GetComponent<Image>();
        alphabet = Resources.Load<LanguageAlphabet>("Alphabet");
        Draw(defaultRune);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Draw(defaultRune);
    }
#endif

    public void Draw(List<RunePart> runeParts)
    {
        Clear();

        if (runeParts == null || runeParts.Count == 0)
            return;

        //Base
        int runeID = 0;
        vBase.sprite = alphabet.GetBase(runeParts[runeID]);
        if (runeParts[runeID] == RunePart.Down)
            runeID++;
        runeID++;

        //Lines
        int lineID = 0;
        RunePart previousRune = RunePart.Right;
        while (runeID < runeParts.Count)
        {
            RunePart currentRune = runeParts[runeID];

            //Repeats
            int repeat = 0;
            for (int r = 1; r < LanguageAlphabet.MAX_REPEATS + 1; r++)
            {
                if (runeID + r >= runeParts.Count)
                    break;

                //if repeat, count and move to next rune
                if (runeParts[runeID + r] == currentRune)
                {
                    repeat++;
                    runeID++;
                }
            }

            lines[lineID].enabled = true;
            lines[lineID].sprite = alphabet.GetLine(currentRune, repeat, previousRune);
            previousRune = currentRune;
        }
    }

    public void Clear()
    {
        vBase.enabled = false;
        foreach (Image line in lines)
        {
            line.enabled = false;
        }
    }
}

public enum RunePart { Up, Right, Down, Left }
