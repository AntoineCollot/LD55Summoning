using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Phone : MonoBehaviour
{
    Animator anim;
    public bool isRinging { get; private set; }
    public bool isPickedUp { get; private set; }
    float lastPickUpTime;

    public UnityEvent onPhoneEvent = new UnityEvent();
    CompositeStateToken isUsingPhoneToken;

    InputMap inputs;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        inputs = new InputMap();
        inputs.Gameplay.Enable();
        isUsingPhoneToken = new CompositeStateToken();
        PlayerState.Instance.freezePositionState.Add(isUsingPhoneToken);
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isPickedUp)
            Release();
        else
            PickUp();
    }

    public void StartRinging()
    {
        isRinging = true;
        anim.SetBool("IsRinging", true);
        StartCoroutine(RingSFX());
    }

    public void StopRinging()
    {
        isRinging = false;

        anim.SetBool("IsRinging", false);
    }

    IEnumerator RingSFX()
    {
        while (isRinging)
        {
            SFXManager.PlaySound(GlobalSFX.PhoneRing);
            yield return new WaitForSeconds(2);
        }
    }

    public void PickUp()
    {
        if (!isRinging)
            return;

        anim.SetBool("IsPickedUp", true);
        StopRinging();
        isPickedUp = true;
        lastPickUpTime = Time.time;
        isUsingPhoneToken.SetOn(true);
        onPhoneEvent.Invoke();

        SFXManager.PlaySound(GlobalSFX.PhonePickUp);
    }

    public void Release()
    {
        if (Time.time < lastPickUpTime + 1)
            return;

        SFXManager.PlaySound(GlobalSFX.PhonePickUp);
        isUsingPhoneToken.SetOn(false);
        isPickedUp = false;
        anim.SetBool("IsPickedUp", false);
        onPhoneEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        anim.SetFloat("Hover", 1);
        inputs.Gameplay.Interact.performed += OnInteract;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        inputs.Gameplay.Interact.performed -= OnInteract;
        anim.SetFloat("Hover", 0);
    }
}
