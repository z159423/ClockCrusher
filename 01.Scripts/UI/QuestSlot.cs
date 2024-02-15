using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestSlot : MonoBehaviour
{
    [SerializeField] QuestType type;
    public QuestType _GetType => type;
    [SerializeField] Image icon;
    [SerializeField] Text questTitle;
    [SerializeField] Button clameBtn;
    [SerializeField] Text reward;
    [SerializeField] Transform moneyIcon;

    [SerializeField] GameObject progress;
    [SerializeField] Image fill;


    public void Init(QuestType type)
    {
        this.type = type;

        UpdateUI();
    }

    public void UpdateUI()
    {
        var find = QuestManager.instance.FindQuest(type);
        var savedata = QuestManager.instance.FindQuestSaveData(type);

        switch (type)
        {
            case QuestType.AddNSticks:
                icon.sprite = Resources.Load<Sprite>("quest icons/icon_AddStick");
                questTitle.text = "Add " + find.goalRequest[savedata.currentLevel] + " Stick";
                break;

            case QuestType.MergeNTimes:
                icon.sprite = Resources.Load<Sprite>("quest icons/icon_Merge");
                questTitle.text = "Merge " + find.goalRequest[savedata.currentLevel] + " Stick";
                break;

            case QuestType.UpgradeSpeedNTimes:
                icon.sprite = Resources.Load<Sprite>("quest icons/icon_Speed");
                questTitle.text = "Upgrade Speed " + find.goalRequest[savedata.currentLevel] + " Times";
                break;

            case QuestType.UpgradeIncome1Times:
                icon.sprite = Resources.Load<Sprite>("quest icons/icon_Money");
                questTitle.text = "Upgrade Income " + find.goalRequest[savedata.currentLevel] + " Times";
                break;

            case QuestType.TapNTimes:
                icon.sprite = Resources.Load<Sprite>("quest icons/icon_AutoFever");
                questTitle.text = "Tap " + find.goalRequest[savedata.currentLevel] + " Times";
                break;

            case QuestType.GetLvNStick:
                icon.sprite = Resources.Load<Sprite>("quest icons/icon_LvUp");
                questTitle.text = "Get " + find.goalRequest[savedata.currentLevel] + " Stick";
                break;

            case QuestType.MoveLvNStickToAnotherArea:
                icon.sprite = Resources.Load<Sprite>("quest icons/img_Icon_Move");
                questTitle.text = "Move " + find.goalRequest[savedata.currentLevel] + " Sticks";
                break;

            case QuestType.Destory1Number:
                icon.sprite = Resources.Load<Sprite>("quest icons/img_Icon_breake");
                questTitle.text = "Break " + find.goalRequest[savedata.currentLevel] + " Numers";
                break;
        }

        questTitle.text = QuestManager.instance.GetQuestDesctiption(type);

        icon.GetComponent<RectTransform>().sizeDelta = new Vector2(icon.sprite.rect.width, icon.sprite.rect.height);

        reward.text = QuestManager.instance.GetQuestReward(type).ToString();

        progress.SetActive(!savedata.complete);
        clameBtn.gameObject.SetActive(savedata.complete);

        fill.fillAmount = (float)savedata.currentProgress / (float)find.goalRequest[savedata.currentLevel];
    }

    private void Awake()
    {
        clameBtn.onClick.AddListener(OnClickClame);
    }

    public void OnClickClame()
    {
        if (QuestManager.instance.FindQuestSaveData(type).complete)
        {
            QuestManager.instance.OnClickClameBtn(type);

            MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.moneyIcon.transform, moneyIcon);

            if (QuestManager.instance.FindQuestSaveData(type).currentLevel > QuestManager.instance.FindQuest(type).goalRequest.Length)
                Destroy(gameObject);
        }
    }
}
