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

    public Sprite GetBase(RunePart rune)
    {
        switch (rune)
        {
            case RunePart.Up:
            default:
                return vBase;
            case RunePart.Right:
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
               repeat  = Mathf.Min(repeat, right.Length);
                return right[repeat];
            case RunePart.Down:
                switch (previousRune)
                {
                    case RunePart.Right:
                    default:
                        repeat = Mathf.Min(repeat, bottomLeft.Length);
                        return bottomLeft[repeat];
                    case RunePart.Left:
                        repeat = Mathf.Min(repeat, bottomRight.Length);
                        return bottomRight[repeat];
                }
            case RunePart.Left:
                repeat = Mathf.Min(repeat, left.Length);
                return left[repeat];
        }
    }
}