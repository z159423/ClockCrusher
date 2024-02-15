using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Unity.VisualScripting;

public class Tutorial : MonoBehaviour
{
    [TitleGroup("First Tutorial")] public Transform firstTutorialParent;
    [TitleGroup("First Tutorial")] public Transform mergeStickTutorialParent;

    [TitleGroup("Pause Tutorial")] public Transform pauseBtn;
    [TitleGroup("Pause Tutorial")] public Transform tutorialFinger;
    [TitleGroup("Pause Tutorial")] public Transform pauseTutorialTarget;
    [TitleGroup("Pause Tutorial")] public Vector3 pauseTutorialOffset;
    [TitleGroup("Pause Tutorial")] public GameObject pauseTutorialText;

    [TitleGroup("Quest Tutorial")] public Transform questTutorialParent;

    [TitleGroup("RVTicket Tutorial")] public Transform shopSideUI;


    public bool pauseTutorial = false;
    public bool dragPhase = false;
    public bool questTutorial = false;
    public bool rvTicketTutorial = false;
    public bool myStickTutorial = false;

    public GameObject currentTutorial;

    [Space]

    public static bool tutorialEnable = true;

    public static Tutorial insatnce;
    public void Awake()
    {
        insatnce = this;
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            StartPauseTutorial();
    }
#endif

    public void AddStickTutorialStart()
    {
        if (!tutorialEnable)
            return;

        currentTutorial = HightlightFocus.GenerateMask(firstTutorialParent, firstTutorialParent.position, firstTutorialParent, 1.2f);
        // ES3.Save<bool>("AddStickTutorial", true);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - AddStick Start", true, true);
    }

    public void AddStickTutorialEnd()
    {
        if (currentTutorial != null)
        {
            Destroy(currentTutorial);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - AddStick End", true, true);
        }
    }

    public void mergeStickTutorialStart()
    {
        if (!tutorialEnable)
            return;

        if (currentTutorial != null)
            Destroy(currentTutorial);

        questTutorialParent.parent.SetSiblingIndex(questTutorialParent.parent.GetSiblingIndex() - 1);
        mergeStickTutorialParent.transform.SetSiblingIndex(mergeStickTutorialParent.parent.childCount);
        currentTutorial = HightlightFocus.GenerateMask(mergeStickTutorialParent, mergeStickTutorialParent.position, mergeStickTutorialParent, 1.2f);
        // ES3.Save<bool>("mergeStickTutorial", true);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - MergeStick Start", true, true);
    }

    public void mergeStickTutorialEnd()
    {
        if (currentTutorial != null)
        {
            Destroy(currentTutorial);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - MergeStick End", true, true);

            ES3.Save<bool>("EnableQuest", false);
            // QuestManager.instance.questSideUI.ShowAnimation();
        }
    }

    public void StartPauseTutorial()
    {
        if (!tutorialEnable)
            return;

        if (currentTutorial != null)
            Destroy(currentTutorial);

        MainManager.instance.pauseBtn.gameObject.SetActive(true);
        MainManager.instance.pauseBtn.FirstPause();

        currentTutorial = HightlightFocus.GenerateMask(pauseBtn, pauseBtn.position + new Vector3(-30, 0), pauseBtn, 0.6f);
        ES3.Save<bool>("PauseTutorial", true);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - Pause Start", true, true);

        pauseTutorial = true;

    }

    private ClockPointer firstPointer;

    public void StartDragPhase()
    {
        // mergetheStickText.SetActive(true);

        Destroy(currentTutorial);

        pauseTutorialTarget = StageManager.instance.currentClock.GetComponentInChildren<TutorialFingerTarget>().transform;

        firstPointer = MainManager.instance.pointerList.OrderByDescending((n) => n.Level).First();

        Vector2 startPos = RectTransformUtility.WorldToScreenPoint(MainManager.instance.mainCamera, (firstPointer.transform.up * 0.5f) + pauseTutorialOffset);
        Vector2 endPos = RectTransformUtility.WorldToScreenPoint(MainManager.instance.mainCamera, (pauseTutorialTarget.position) + pauseTutorialOffset);

        // pauseBtn.btnImage.sprite = pauseBtn.playImage;

        tutorialFinger.GetComponent<RectTransform>().anchoredPosition = startPos - new Vector2(Screen.width / 2f, Screen.height / 2f);
        tutorialFinger.gameObject.SetActive(true);

        tutorialFinger.GetComponent<RectTransform>().DOAnchorPos(endPos - new Vector2(Screen.width / 2f, Screen.height / 2f), 1).SetLoops(-1, LoopType.Restart);

        dragPhase = true;

        pauseTutorialText.SetActive(true);
        MainManager.instance.Focus_OnlySelectedStick(firstPointer);
    }

    public void EndDragPhase()
    {
        tutorialFinger.gameObject.SetActive(false);

        MainManager.instance.pauseBtn.GetComponentInChildren<PauseBtn>().ShowHide();

        dragPhase = false;

        pauseTutorialText.SetActive(false);
        // firstPointer.HighlightThisPointerOFF();

        MainManager.instance.Unfocus_SelectedStickAndMergeableStick();
    }

    public void EndPauseTutorial()
    {
        pauseTutorialTarget = null;

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - Pause End", true, true);

        pauseTutorial = false;

        MainManager.instance.pauseBtn.rvBtn.SetActive(true);

        ES3.Save<bool>("EnableIS", false);
        this.TaskDelay(10, () => ES3.Save<bool>("EnableIS", true));
    }


    public void StartQuestTutorial()
    {
        if (!tutorialEnable)
            return;

        if (currentTutorial != null)
            Destroy(currentTutorial);

        currentTutorial = HightlightFocus.GenerateMask(questTutorialParent, questTutorialParent.position, questTutorialParent, 1.2f);
        questTutorialParent.parent.SetSiblingIndex(questTutorialParent.parent.GetSiblingIndex() + 1);
        ES3.Save<bool>("QuestTutorial", true);
        questTutorial = true;

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - Quest Start", true, true);
    }

    public void EndQuestTutorial()
    {
        if (currentTutorial != null)
        {
            questTutorial = false;
            Destroy(currentTutorial);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - Quest End", true, true);
        }
    }

    [Button("StartRVTicketTutorial")]
    public void StartRVTicketTutorial()
    {
        if (!tutorialEnable)
            return;

        if (currentTutorial != null)
            Destroy(currentTutorial);

        shopSideUI.gameObject.SetActive(true);
        currentTutorial = HightlightFocus.GenerateMask(shopSideUI, shopSideUI.position, shopSideUI, 0.5f);
        ES3.Save<bool>("enableShop", true);
        rvTicketTutorial = true;

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - RVTicket Start", true, true);
    }

    public void EndRVTicketTutorial()
    {
        if (currentTutorial != null)
        {
            rvTicketTutorial = false;
            Destroy(currentTutorial);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - RVTicket End", true, true);
        }


        if (!ES3.KeyExists("RequestReview"))
        {
            this.TaskDelay(5f, () => CustomReviewManager.instance.StoreReview());
            ES3.Save<bool>("RequestReview", true);
        }
    }


    [Button("StartMyStickTutorial")]
    public void StartMyStickTutorial()
    {
        if (!tutorialEnable)
            return;

        ES3.Save<bool>("MyStickTutorial", true);

        myStickTutorial = true;

        currentTutorial = HightlightFocus.GenerateMask_FullMask(MainManager.instance.playUI, MainManager.instance.stickCollectionBtn.icon.transform.position, 0.6f);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - MyStick Start", true, true);
    }

    public void SelectStickPhase()
    {

    }

    public void EndMyStickTutorial()
    {
        if (currentTutorial != null)
        {
            myStickTutorial = false;
            Destroy(currentTutorial);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Tutorial - MyStick End", true, true);
        }
    }
}
