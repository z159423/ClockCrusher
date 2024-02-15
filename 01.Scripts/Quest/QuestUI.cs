using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class QuestUI : MonoBehaviour
{
    [SerializeField] Transform layoutParent;
    [SerializeField] Button[] hideBtns;
    [SerializeField] GameObject noQuestText;

    [SerializeField] List<QuestSlot> questSlots = new List<QuestSlot>();

    private void Start()
    {
        foreach (var btn in hideBtns)
            btn.onClick.AddListener(OnClickHide);
    }

    public void Init()
    {
        GenearteQuestSlots();
        UpdateUI();
    }

    public void GenearteQuestSlots()
    {
        foreach (var saveData in QuestManager.instance.activeQuestList)
        {
            var find = QuestManager.instance.FindQuest(saveData.questType);

            //모두 클리어한 퀘스트일경우 생성 안됨
            if (QuestManager.instance.FindQuestSaveData(saveData.questType).currentLevel <= find.goalRequest.Length)
            {
                var slot = Instantiate(Resources.Load<GameObject>("UI/Quest Slot"), layoutParent);
                questSlots.Add(slot.GetComponentInChildren<QuestSlot>());

                slot.GetComponentInChildren<QuestSlot>().Init(saveData.questType);
            }
        }

        SortSlots();
    }

    public void UpdateUI(bool clear = false)
    {
        if (clear)
        {
            for (int i = 0; i < questSlots.Count; i++)
            {
                Destroy(questSlots[i].gameObject);
            }

            questSlots.Clear();

            GenearteQuestSlots();
        }
        else
        {
            questSlots.ForEach((n) => n.UpdateUI());
        }

        SortSlots();

        noQuestText.SetActive((QuestManager.instance.activeQuestList.Count() == 0));
    }

    public void SortSlots()
    {
        // var sortedList = questSlots.OrderBy((n) => ((float)QuestManager.instance.FindQuestSaveData(n._GetType).currentLevel / (float)QuestManager.instance.FindQuest(n._GetType).goalRequest[QuestManager.instance.FindQuestSaveData(n._GetType).currentLevel])).ToList();

        foreach (var slot in questSlots)
        {
            if (QuestManager.instance.FindQuestSaveData(slot._GetType).complete)
                slot.transform.SetAsFirstSibling();
        }
    }

    public void OnClickHide()
    {
        QuestManager.instance.HideQuestUI();
        Destroy(gameObject);
    }
}
