using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RuneSentence", menuName = "ScriptableObjects/Sentence", order = 1)]
public class ScritptableRuneSentence : ScriptableObject
{
    public List<ScriptableRune> runes;
    List<RunePart> Parts
    {
        get
        {
            List<RunePart> parts = new List<RunePart>();
            foreach (ScriptableRune rune in runes)
            {
                parts.AddRange(rune.runeParts);
            }
            return parts;
        }
    }


    [ContextMenu("Print")]
    void Print()
    {
        string input = "";
        for (int i = 0; i < Parts.Count; i++)
        {
            input += Parts[i] + ", ";
        }
        Debug.Log(input);
    }

    public bool Match(List<RunePart> inputRunes)
    {
        if (inputRunes == null)
            return false;

#if UNITY_EDITOR
        Debug.Log("Trying to match: ");
        Print();
#endif

        List<RunePart> parts = Parts;

        if (inputRunes.Count != parts.Count)
        {
#if UNITY_EDITOR
            Debug.Log("No matching counts");
#endif
            return false;
        }

        for (int i = 0; i < inputRunes.Count; i++)
        {
#if UNITY_EDITOR
            // Debug.Log($"Checking {inputRunes[i]}/{parts[i]}");
#endif
            if (inputRunes[i] != parts[i])
                return false;
        }

        return true;
    }
}
