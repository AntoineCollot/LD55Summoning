using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvocationCircle : MonoBehaviour
{
    Material mat;
    bool isHovered;
    CompositeStateToken isSummoningState;
    float power;
    float refPower;
    float summoning;
    float refSummoning;
    const float SMOOTH_POWER = 0.1f;
    const float SMOOTH_SUMMONING = 0.1f;
    CharAnimations animations;

    [Header("Colors")]
    [ColorUsage(false, true), SerializeField] Color errorColor;
    Color baseColor;

    //Inputs
    [Header("Inputs")]
    InputMap inputs;
    RunePart lastFrameRuneDir;
    float dirProgress;
    const float HOLD_DIR_TIME = 0.5f;

    //Rune
    [Header("Runes")]
    [SerializeField] LanguageRune[] displayRunes;
    public List<RunePart> inputRunes;
    bool hasDirBeenValdiated;
    bool areRunesValid;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponentInChildren<MeshRenderer>().material;
        isSummoningState = new CompositeStateToken();
        PlayerState.Instance.freezePositionState.Add(isSummoningState);
        animations = PlayerState.Instance.transform.GetComponentInChildren<CharAnimations>();

        baseColor = mat.GetColor("_PowerColor");

        inputs = new InputMap();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Interact.performed += OnInteract;
        ClearRunesDisplay();
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isHovered)
            return;

        if (!isSummoningState.IsOn)
            EnterSummoningState();
        else
            ExitSummoningState();
    }

    // Update is called once per frame
    void Update()
    {
        float powerTarget = 0;
        float summoningTarget = 0;
        if (isHovered)
            powerTarget = 1;
        if (isSummoningState.IsOn)
            summoningTarget = 1;

        power = Mathf.SmoothDamp(power, powerTarget, ref refPower, SMOOTH_POWER);
        summoning = Mathf.SmoothDamp(summoning, summoningTarget, ref refSummoning, SMOOTH_SUMMONING);

        mat.SetFloat("_Power", power);
        mat.SetFloat("_Summoning", summoning);

        if(isSummoningState.IsOn)
        {
            ParseSummoningInput(inputs.Gameplay.Move.ReadValue<Vector2>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        isHovered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        isHovered = false;
    }

    public void EnterSummoningState()
    {
        inputRunes = new List<RunePart>();
        isSummoningState.SetOn(true);
        PlayerState.Instance.GetComponentInChildren<CharController>().LerpToPosition(transform.position);
        animations.LookCamera();
        animations.SetSummoningIdle();
        SFXManager.PlaySound(GlobalSFX.StartSummoning);
        areRunesValid = true;
    }

    public void ExitSummoningState()
    {
        inputRunes = null;
        mat.SetFloat("_FillInput", 0);
        mat.SetColor("_PowerColor", baseColor);
        isSummoningState.SetOn(false);
        animations.ExitSummoning();
        ClearRunesDisplay();
        SFXManager.PlaySound(GlobalSFX.ExitSummoning);
    }

    void ParseSummoningInput(Vector2 dir)
    {
        if (!isSummoningState.IsOn)
            return;

        //Idle
        if (dir.magnitude < 0.5f)
        {
            animations.SetSummoningIdle();
            dirProgress = 0;
            mat.SetFloat("_FillInput", 0);
            hasDirBeenValdiated = false;
        }
        else
        {
            //X
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x > 0)
                    UseSummoningInput(RunePart.Right);
                else
                    UseSummoningInput(RunePart.Left);
            }
            //Y
            else
            {
                if (dir.y > 0)
                    UseSummoningInput(RunePart.Up);
                else
                    UseSummoningInput(RunePart.Down);
            }
        }
    }

    void UseSummoningInput(RunePart runeDir)
    {
        if (lastFrameRuneDir == runeDir)
        {
            dirProgress += Time.deltaTime / HOLD_DIR_TIME;
        }
        else
        {
            dirProgress = 0;
            hasDirBeenValdiated = false;
        }

        if(dirProgress>1)
            ValidateDir(runeDir);
        else
            mat.SetFloat("_FillInput", dirProgress);
            
        animations.SetSummoningDir(runeDir);

        lastFrameRuneDir = runeDir;
    }

    void ValidateDir(RunePart runeDir)
    {
        if (hasDirBeenValdiated)
            return;

        hasDirBeenValdiated = true;
        mat.SetFloat("_FillInput", 0);
        inputRunes.Add(runeDir);

        ClearRunesDisplay();
        if (LanguageAlphabet.TryParseSentence(inputRunes, out List<List<RunePart>> runes))
        {
            SFXManager.PlaySound(GlobalSFX.SummoningInput);
            for (int i = 0; i < runes.Count; i++)
            {
                displayRunes[i].Draw(runes[i]);
            }
        }
        else if(areRunesValid)
        {
            //Error
            SFXManager.PlaySound(GlobalSFX.SummoningError);
            mat.SetColor("_PowerColor", errorColor);
            areRunesValid = false;
        }
    }

    void ClearRunesDisplay()
    {
        for (int i = 0; i < displayRunes.Length; i++)
        {
            displayRunes[i].Clear();
        }
    }
}