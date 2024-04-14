using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Rune", menuName = "ScriptableObjects/Rune", order = 1)]
public class ScriptableRune : ScriptableObject
{
    public List<RunePart> runeParts;
}