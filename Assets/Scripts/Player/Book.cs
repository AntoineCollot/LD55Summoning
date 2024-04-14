using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public Material pageMat;
    public MeshRenderer bookRenderer;
    const float PICK_UP_DELAY = 0.4f;

    public Material[] SharedMaterials
    {
        get
        {
            Material[] bookMat = bookRenderer.sharedMaterials;
            return new Material[3] { bookMat[0], bookMat[1], pageMat };
        }
    }

    public void HideBook(bool value)
    {
        if (value)
        {
            CancelInvoke();
            Invoke("HideDelayed", PICK_UP_DELAY);
        }
        else
        {
            CancelInvoke();
            bookRenderer.enabled = true;
        }
    }

    public void Hover(bool value)
    {
        if (value)
            bookRenderer.gameObject.layer = LayerMask.NameToLayer("BookOutline");
        else
            bookRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    void HideDelayed()
    {
        bookRenderer.enabled = false;
    }
}