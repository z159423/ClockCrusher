using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class QuestSaveData
{
    public QuestType type;
    public int currentLevel;
    public int currentProgress;

    public bool complete = false;

    public void ImproveProgress()
    {
        if (complete)
            return;

        currentProgress++;

        if (currentProgress >= QuestManager.instance.FindQuest(type).goalRequest[currentLevel])
            CompleteQuest();
    }

    [Button]
    public void ForceComplete()
    {
        currentProgress = QuestManager.instance.FindQuest(type).goalRequest[currentLevel];

        CompleteQuest();
    }

    public void CompleteQuest()
    {
        complete = true;
        QuestManager.instance.CompleteQuest(type);
    }

    public void AfterClameReward()
    {
        complete = false;

        // if (QuestManager.instance.FindQuest(type).GetMaxRequestLevel() >= currentLevel)
            currentLevel++;
        currentProgress = 0;
    }

    public void ResetQuest()
    {
        currentLevel = 0;
        currentProgress = 0;
    }
}
