using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Emulate the behaviour of a fire light by making its range and intensity change over time in flashes.
/// </summary>
public class FireLightBlinking : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float rangeAmount = 0.1f;
    [SerializeField, Range(0, 1)] float intensityAmount = 0.1f;
    [SerializeField] float frequency;

    Light fireLight;
    float nextValue;
    float t;
    float startValue;
    float baseRange;
    float baseIntensity;

    private void Awake()
    {
        fireLight = GetComponent<Light>();
        baseRange = fireLight.range;
        baseIntensity = fireLight.intensity;
        nextValue = Random.Range(-0.5f, 0.5f);
        frequency *= Random.Range(0.9f, 1.1f);
    }

    private void Update()
    {
        t += Time.deltaTime * frequency;
        if(t>= 1)
        {
            t = 0;
            startValue = nextValue;
            nextValue = Random.Range(-0.5f, 0.5f);
        }
        float currentValue = Mathf.Lerp(startValue, nextValue, t);

        fireLight.range = baseRange + currentValue * rangeAmount;
        fireLight.intensity = baseIntensity + currentValue * intensityAmount;
    }
}
