using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum HitTextType
{
    Text,
    Mesh
}

public class Cheat : MonoBehaviour
{
    [SerializeField] private CanvasGroup UI_CanvasGroup;
    [SerializeField] private CanvasGroup money_CanvasGroup;
    [SerializeField] private GameObject console;

    [SerializeField] private GameObject ITR_UI_1;
    [SerializeField] private GameObject ITR_UI_2;

    public int selectedSpawnPointerLevel = 1;

    public bool whileEdit = false;

    [Space]

    [SerializeField] private InputField[] clockNumHpInputFields;

    [Space]

    public Dropdown select_pointerType;
    public Dropdown select_clockMatType;
    public Dropdown select_mergeType;
    public Dropdown select_backgroundType;

    public HitTextType hitTextType;


    public static Cheat instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // this.TaskDelay(0.1f, () => MondayOFF.AdsManager.HideBanner());

        if (MainManager.instance._ITR_MODE)
        {
            select_pointerType.value = (int)ITRManager.instance.pointerType;
            select_clockMatType.value = (int)ITRManager.instance.clockMatType;
            select_mergeType.value = (int)ITRManager.instance.mergeType;
            select_backgroundType.value = (int)ITRManager.instance.backgroundType;

            select_pointerType.onValueChanged.AddListener(SelectPointerType);
            select_clockMatType.onValueChanged.AddListener(SelectClockMatType);
            select_mergeType.onValueChanged.AddListener(SelectMergeTypeType);
            select_backgroundType.onValueChanged.AddListener(SelectBackgroundType);
        }
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UI_CanvasGroup.alpha = UI_CanvasGroup.alpha == 0 ? 1 : 0;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            money_CanvasGroup.alpha = money_CanvasGroup.alpha == 0 ? 1 : 0;
        }
        else if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            MainManager.instance.pause = !console.activeSelf;
            console.SetActive(!console.activeSelf);
        }
        else if (Input.GetKeyDown(KeyCode.G) && !Cheat.instance.whileEdit)
        {
            SkipStage();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            hitTextType = hitTextType == HitTextType.Text ? HitTextType.Mesh : HitTextType.Text;
        }
    }
#endif

    public void OnChangeSelectedPointerLevel(int level) => selectedSpawnPointerLevel = level;


    public void ToggleITR_UI_1()
    {
        ITR_UI_1.SetActive(!ITR_UI_1.activeSelf);

        UI_CanvasGroup.alpha = UI_CanvasGroup.alpha = 0;
        money_CanvasGroup.alpha = money_CanvasGroup.alpha = 0;

    }

    public void ToggleITR_UI_2()
    {
        ITR_UI_2.SetActive(!ITR_UI_2.activeSelf);

        UI_CanvasGroup.alpha = UI_CanvasGroup.alpha = 0;
        money_CanvasGroup.alpha = money_CanvasGroup.alpha = 0;
    }

    public void Toggle_MoneyGainText()
    {
        MainManager.instance.showMoneyGainText = !MainManager.instance.showMoneyGainText;
    }

    public void SkipStage()
    {
        StageManager.instance.StageClear();
        StageManager.instance.NextStage();
    }

    public void ForceChangeValue(int num, string value)
    {
        StageManager.instance.currentClock.ForceChangeValue(num, int.Parse(value));
    }

    public void ChangeClockNumHpField(int num, int value)
    {
        clockNumHpInputFields[num].text = value.ToString();
        whileEdit = false;
    }

    public void OnEditStart()
    {
        whileEdit = true;
    }

    public void OnEidtEnd()
    {
        whileEdit = false;
    }

    public void SelectPointerType(int index)
    {
        ITRManager.instance.pointerType = (pointerType)index;

        SceneManager.LoadSceneAsync(1);
    }

    public void SelectClockMatType(int index)
    {
        ITRManager.instance.clockMatType = (clockMatType)index;

        SceneManager.LoadSceneAsync(1);
    }

    public void SelectMergeTypeType(int index)
    {
        ITRManager.instance.mergeType = (MergeType)index;

        SceneManager.LoadSceneAsync(1);
    }

    public void SelectBackgroundType(int index)
    {
        ITRManager.instance.backgroundType = (BackgroundType)index;

        SceneManager.LoadSceneAsync(1);
    }

}
