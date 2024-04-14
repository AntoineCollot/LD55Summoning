using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests;
    List<Quest> onGoingQuests;
    float timeSinceLastQuest;

    InvocationCircle invocation;

    Phone phone;
    Quest ringingQuest;

    [Header("Quest Info")]
    [SerializeField] GameObject questInfoCanvas;
    [SerializeField] Image questImage;

    // Start is called before the first frame update
    void Start()
    {
        phone = FindObjectOfType<Phone>();
        invocation = FindObjectOfType<InvocationCircle>();
        onGoingQuests = new List<Quest>();
        phone.onPhoneEvent.AddListener(OnPhoneEvent);
        invocation.onRuneEvent.AddListener(OnRuneEvent);
    }

    private void Update()
    {
        if (!phone.isRinging)
            timeSinceLastQuest += Time.deltaTime;

        if (timeSinceLastQuest >= quests[0].maxIntervalSinceLastQuest)
        {
            //Move first quest to ongoing
            RingQuest(quests[0]);
            quests.RemoveAt(0);
            timeSinceLastQuest = 0;

            //Check if end
            if (quests.Count == 0)
                enabled = false;
        }
    }

    void RingQuest(Quest quest)
    {
        ringingQuest = quest;
        phone.StartRinging();
    }

    private void OnPhoneEvent()
    {
        if (phone.isPickedUp)
        {
            StartQuest(ringingQuest);
        }
        else
        {
            questInfoCanvas.SetActive(false);
        }
    }

    void StartQuest(Quest quest)
    {
        questInfoCanvas.SetActive(true);
        questImage.sprite = quest.image;
        quest.breakableObj.Show();
        SFXManager.PlaySound(GlobalSFX.Smoke);
        onGoingQuests.Add(quest);
    }

    private void OnRuneEvent()
    {
        //Check if solution
        for (int i = 0; i < onGoingQuests.Count; i++)
        {
            if (onGoingQuests[i].Match(invocation.inputRunes))
            {
                EndOnGoingQuest(i);
                return;
            }
        }
    }

    void EndOnGoingQuest(int id)
    {
        onGoingQuests[id].breakableObj.FixObject();
        onGoingQuests.RemoveAt(id);

        invocation.SummoningSuccess();

        if (onGoingQuests.Count == 0)
            timeSinceLastQuest = 10000;
    }
}

[System.Serializable]
public struct Quest
{
    public BreakableObject breakableObj;
    public float maxIntervalSinceLastQuest;
    public Sprite image;
    public List<ScritptableRuneSentence> availableSentences;

    public bool Match(List<RunePart> runes)
    {
        foreach (ScritptableRuneSentence sentence in availableSentences)
        {
            if (sentence.Match(runes))
                return true;
        }
        return false;
    }
}