using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] GameObject smokeFX;
    SkinnedMeshRenderer meshRenderer;

    [Header("Cat")]
    public bool isCat;
    public Texture2D aliveCat;

    private void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshRenderer.SetBlendShapeWeight(0, 100);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (smokeFX != null)
            smokeFX.SetActive(true);
    }

    public void FixObject()
    {
        if (smokeFX != null)
        {
            smokeFX.SetActive(true);
        }
        SFXManager.PlaySound(GlobalSFX.SummoningSuccess);
        SFXManager.PlaySound(GlobalSFX.Smoke);

        StartCoroutine(FixAnim());
    }

    IEnumerator FixAnim()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;

            meshRenderer.SetBlendShapeWeight(0, Curves.QuadEaseInOut(100, 0, Mathf.Clamp01(t)));

            yield return null;
        }

        if (isCat)
        {
            Material[] mats = meshRenderer.materials;
            mats[1].SetTexture("_Albedo", aliveCat);
            meshRenderer.materials = mats;
        }
    }
}