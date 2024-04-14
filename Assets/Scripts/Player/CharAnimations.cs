using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimations : MonoBehaviour
{
    CharController controller;
    Animator anim;
    
    public Vector3 currentDirection = Vector3.forward;
    Vector3 targetDirection;
    public float rotationSpeed = 360;

    //Summoning
    bool isSummoning;
    float sUp, sDown, sRight, sLeft, sIdle;
    float refSUp, refSDown, refSRight, refSLeft, refSIdle;
    bool isIdle;
    RunePart runeDir;
    const float SUMMONING_SMOOTH = 0.075f;
    private void Awake()
    {
        controller = GetComponentInParent<CharController>();
        anim = GetComponent<Animator>();
        currentDirection.Normalize();
    }

    void Update()
    {
        anim.SetFloat("MoveSpeed", controller.NormalizedMoveSpeed);

        if(!PlayerState.Instance.freezePositionState.IsOn)
        {
            if (controller.NormalizedMoveSpeed > 0.1f)
                targetDirection = controller.MoveDirection;
        }

        float rotationStep = Mathf.Deg2Rad * rotationSpeed * Time.deltaTime;
        currentDirection = Vector3.RotateTowards(currentDirection, targetDirection, rotationStep, 0);
        currentDirection.y = 0;
        transform.LookAt(transform.position + currentDirection);

        if (isSummoning)
            UpdateSummoningParams();
    }

    public void PickUpBook(bool value)
    {
        anim.SetBool("HasBook", value);
        LookDirection(Vector3.forward);
    }

    public void LookCamera()
    {
        targetDirection = Camera.main.transform.position -transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();
    }

    public void LookDirection(Vector3 direction)
    {
        targetDirection = direction;
    }

    public void SetSummoningDir(RunePart runeDir)
    {
        if(!isSummoning)
        {
            isSummoning = true;
            anim.SetBool("IsSummoning", true);
        }
        if(runeDir != this.runeDir || isIdle)
        { 
            this.runeDir = runeDir;
            switch (runeDir)
            {
                case RunePart.Up:
                    SFXManager.PlaySound(GlobalSFX.SUp);
                    break;
                case RunePart.Right:
                    SFXManager.PlaySound(GlobalSFX.SRight);
                    break;
                case RunePart.Down:
                    SFXManager.PlaySound(GlobalSFX.SDown);
                    break;
                case RunePart.Left:
                    SFXManager.PlaySound(GlobalSFX.SLeft);
                    break;
            }
        }
        isIdle = false;
    }

    public void SetSummoningIdle()
    {
        if (!isSummoning)
        {
            isSummoning = true;
            anim.SetBool("IsSummoning", true);
        }
        isIdle = true;
    }

    public void ExitSummoning()
    {
        isSummoning = false;
        anim.SetBool("IsSummoning", false);
    }

    void UpdateSummoningParams()
    {
        float sUpTarget = 0;
        float sDownTarget = 0;
        float sRightTarget = 0;
        float sLeftTarget = 0;
        float sIdleTarget = 0;

        if (isIdle)
            sIdleTarget = 1;
        else
        {
            switch (runeDir)
            {
                case RunePart.Up:
                default:
                    sUpTarget = 1;
                    break;
                case RunePart.Right:
                    sRightTarget = 1;
                    break;
                case RunePart.Down:
                    sDownTarget = 1;
                    break;
                case RunePart.Left:
                    sLeftTarget = 1;
                    break;
            }
        }

        sIdle = Mathf.SmoothDamp(sIdle, sIdleTarget, ref refSIdle, SUMMONING_SMOOTH);
        sUp = Mathf.SmoothDamp(sUp, sUpTarget, ref refSUp, SUMMONING_SMOOTH);
        sRight = Mathf.SmoothDamp(sRight, sRightTarget, ref refSRight, SUMMONING_SMOOTH);
        sDown = Mathf.SmoothDamp(sDown, sDownTarget, ref refSDown, SUMMONING_SMOOTH);
        sLeft = Mathf.SmoothDamp(sLeft, sLeftTarget, ref refSLeft, SUMMONING_SMOOTH);

        anim.SetFloat("SummoningIdle", sIdle);
        anim.SetFloat("SummoningUp", sUp);
        anim.SetFloat("SummoningRight", sRight);
        anim.SetFloat("SummoningDown", sDown);
        anim.SetFloat("SummoningLeft", sLeft);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, currentDirection * 10);
        Gizmos.color = Color.red;
        if (controller != null && controller.NormalizedMoveSpeed > 0.1f)
            Gizmos.DrawRay(transform.position, controller.MoveDirection * 10);
    }
#endif
}
