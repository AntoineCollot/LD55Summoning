using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Alphabet", menuName = "ScriptableObjects/Alphabet", order = 1)]
public class LanguageAlphabet : ScriptableObject
{
    [Header("Bases")]
    public Sprite vBase;
    public Sprite vBaseNo;

    [Header("Lines")]
    public Sprite[] left;
    public Sprite[] right;
    public Sprite[] bottomLeft;
    public Sprite[] bottomRight;

    public const int MAX_REPEATS = 3;
    public const int MAX_LINES = 4;
    public const int MAX_RUNES = 4;

    public Sprite GetBase(RunePart rune)
    {
        switch (rune)
        {
            case RunePart.Up:
            default:
                return vBase;
            case RunePart.Down:
                return vBaseNo;
        }
    }

    public Sprite GetLine(RunePart rune, int repeat, RunePart previousRune = RunePart.Right)
    {
        switch (rune)
        {
            case RunePart.Up:
            default:
                Debug.Log("Can't return a up rune as line");
                return right[0];
            case RunePart.Right:
               repeat  = Mathf.Min(repeat, right.Length - 1);
                return right[repeat];
            case RunePart.Down:
                switch (previousRune)
                {
                    case RunePart.Left:
                        repeat = Mathf.Min(repeat, bottomLeft.Length - 1);
                        return bottomLeft[repeat];
                    case RunePart.Right:
                    default:
                        repeat = Mathf.Min(repeat, bottomRight.Length - 1);
                        return bottomRight[repeat];
                }
            case RunePart.Left:
                repeat = Mathf.Min(repeat, left.Length-1);
                return left[repeat];
        }
    }

    public static bool IsRuneValid(List<RunePart> rune)
    {
        if(rune == null || rune.Count == 0)
            return false;

        if (rune[0] == RunePart.Left || rune[0] == RunePart.Right)
            return false;

        int start = 1;
        if (rune[0] == RunePart.Down)
        {
            if (rune.Count < 2 || rune[1] != RunePart.Up)
                return false;

            start = 2;
        }

        //Check that there is no more up part or too much components
        int lines = 0;
        int partID = start;
        while(partID < rune.Count)
        {
            ParseRune(rune, partID, out RunePart displayPart, out int repeat);

            //Check no up
            if (displayPart == RunePart.Up)
                return false;

            lines++;
            partID++;
            partID += repeat;
        }
        if (lines > MAX_LINES)
            return false;

        return true;
    }

    public static void ParseRune(List<RunePart> rune, int id, out RunePart displayPart, out int repeat)
    {
        displayPart = rune[id];

        //Repeats
        repeat = 0;
        int maxRepeat = MAX_REPEATS;
        if (displayPart == RunePart.Down)
            maxRepeat--;
        for (int r = 1; r < maxRepeat; r++)
        {
            if (id + r >= rune.Count)
                break;

            if (rune[id + r] != displayPart)
                break;
            //if repeat, count and move to next rune
            repeat++;
        }
    }

    public static bool TryParseSentence(List<RunePart> dirs, out List<List<RunePart>> runes)
    {
        runes = new List<List<RunePart>>();
        if (dirs == null || dirs.Count == 0 || dirs[0] == RunePart.Left || dirs[0] == RunePart.Right)
            return false;

        int i = 0;
        while(true)
        {
            List<RunePart> currentRune = new List<RunePart>() { dirs[i] };
            i++;

            //if down
            if (dirs[i] == RunePart.Down)
            {
                //Expect up
                if (dirs[i]!= RunePart.Up)
                    return false;

                currentRune.Add(dirs[i]);
                i++;
            }

            //Process all parts of this rune
            while (i + 1 < dirs.Count && dirs[i+1] != RunePart.Up)
            {
                currentRune.Add(dirs[i]);
                i++;
            }

            //Check if end
            if (i >= dirs.Count)
            {
                runes.Add(currentRune);
                break;
            }

            //Check if we add the last rune (if not down, cause down is used to no the next rune)
            if (dirs[i] != RunePart.Down)
            {
                currentRune.Add(dirs[i]);
                i++;
            }

            //Add rune and move next
            runes.Add(currentRune);

            if (runes.Count >= MAX_RUNES)
                break;
        }

        //Check if valid
        foreach (List<RunePart> rune in runes)
        {
            if (!IsRuneValid(rune))
                return false;
        }

        return true;
    }
}