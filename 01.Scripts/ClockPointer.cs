using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using EPOOutline;

[System.Serializable]
public class ClockPointerSaveData
{
    public string guid;
    public int level;
    public float spawnRotate;
    public float lastRotation;
}

public enum PointerSkinType
{
    Default,
    Antique
}

[System.Serializable]
public struct PointerSkin
{
    public Mesh mesh;
    public Material[] materials;
    public Material[] materials_Trans;
    public Collider collider;
    public GameObject effect;
}

public class ClockPointer : MonoBehaviour
{
    public string guid;

    Clock currentClock;

    ClockNum lastAttackedNum = null;

    public int clockDirection = 1;

    public TrailRenderer trailRenderer;
    public Outlinable outlinable;
    public Collider _collider;
    public MeshRenderer _renderer;
    public MeshFilter _meshFilter;

    public int Level = 1;
    public int damage = 1;
    public float extraSpeed = 0f;
    public float bonusForIncomeUpgrade = 0.15f;

    public bool coolTime = false;

    public bool Stop = false;

    public bool selected = false;

    Vector3 originScale;

    public Mesh meshtype1;
    public Mesh meshtype2;
    public PointerSkin defaultSkin;
    public PointerSkin antiqueSkin;
    public Material[] GetCurrectMaterial(bool transparent = false) =>
    transparent ? (MainManager.instance.pointerSkinType == PointerSkinType.Default ? defaultSkin.materials_Trans : antiqueSkin.materials_Trans)
     : (MainManager.instance.pointerSkinType == PointerSkinType.Default ? defaultSkin.materials : antiqueSkin.materials);

    private Upgrade currentUpgrade;


    private void Start()
    {
        currentClock = GetComponentInParent<Clock>();

        originScale = transform.localScale;

        SetTrailLength();

        if (MainManager.instance._ITR_MODE)
            ChangeMesh();

        ChangeSkin();

        currentUpgrade = MainManager.instance.FindUpgrade(UpgradeType.Speed);
    }

    private void Update()
    {
        if (MainManager.instance.enableRotate && !Stop && !currentClock.stop && !selected && !MainManager.instance.pause && !MainManager.instance.tutorial1)
            transform.Rotate(new Vector3(0, 0, ((((MainManager.instance.GetRotateSpeed2(currentUpgrade) * 0.5f + extraSpeed)
             * (MainManager.instance.lastFever ? 5f : 1))
              * (AdManager.instance.speedFever ? 3f : 1f))
               * (MainManager.instance.onDragPointer ? 0.25f : 1)
                * (IapManager.instance.starterPack ? 3 : 1)
                 * clockDirection * (Time.deltaTime * 35)
                 * (MainManager.instance.pointerSkinType == PointerSkinType.Antique ? 1.3f : 1))));

        if (AdManager.instance.autoFever)
        {
            StartTrail();
        }

        // print(Time.deltaTime);

        // CheckDistBetweenNum();
    }

    public void StartTrail()
    {
        trailRenderer.emitting = true;
    }

    public void StopTrail()
    {
        if (Stop)
            return;
        trailRenderer.emitting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Stop || selected)
            return;

        if (other.CompareTag("ClockNum"))
        {
            var clockNum = other.GetComponentInParent<ClockNum>();

            if ((StageManager.instance.currentClock.aliveNum.Length == 1 ? true : lastAttackedNum != clockNum) && coolTime == false)
            {
                // Debug.LogError(21411);
                float AddedDamage = (damage * MainManager.instance.UPGRADE_INCOME_VAlUE[MainManager.instance.income.currentLevel]) * (IapManager.instance.moneyPack ? 2 : 1f)
                 * (AdManager.instance.moneyFever ? AdManager.instance.moenyFeverMultifly : 1f) * (clockNum.goldNum ? 2 : 1);
                clockNum.TakeDamage(damage);
                lastAttackedNum = clockNum;
                SaveManager.instance.AddMoney(AddedDamage);
                StageManager.instance.currentClock.ClockWiggle();
                clockDirection = (clockDirection == 1) ? -1 : 1;

                coolTime = true;
                if (coolTime)
                    this.TaskDelay(0.05f, () => coolTime = false);

                if (MainManager.instance.showMoneyGainText)
                {
                    if (Cheat.instance.hitTextType == HitTextType.Text)
                    {
                        if (AdManager.instance.moneyFever || clockNum.goldNum)
                            MainManager.instance.GenerateMoneyText(AddedDamage, _collider.ClosestPoint(other.transform.position) + new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f), -0.2f), "MoneyParticleText_MoneyFever");

                        else
                            MainManager.instance.GenerateMoneyText(AddedDamage, _collider.ClosestPoint(other.transform.position) + new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f), -0.2f));
                    }
                    else if (Cheat.instance.hitTextType == HitTextType.Mesh)
                    {
                        MainManager.instance.GenerateMoneyMesh(AddedDamage, clockNum.transform.position + (Vector3.back * 0.15f));
                    }
                }
            }

        }

        // if (other.CompareTag("Stopper") && coolTime == false)
        // {
        //     if (other.GetComponentInChildren<Stopper>().blocked)
        //     {
        //         clockDirection = (clockDirection == 1) ? -1 : 1;
        //         lastAttackedNum = null;
        //     }
        // }
    }

    public void CheckDistBetweenNum()
    {
        // currentClock.GetClockNums().Where((n) => )
    }

    public void ChangeRotateDirection()
    {

    }

    public void ChangeRotateSpeed()
    {
        transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public void SelectThisPointer()
    {
        if (selected)
            return;

        selected = true;

        transform.DOScale(originScale * 1.1f, 0.2f).SetEase(Ease.OutBack);

        outlinable.enabled = true;
    }

    public void UnSelectThisPointer()
    {
        transform.DOScale(originScale, 0.2f).SetEase(Ease.OutBack);

        outlinable.enabled = false;

        selected = false;

        lastAttackedNum = null;
    }

    Tween outlineTween = null;

    public void HighlightThisPointerON()
    {
        transform.DOScale(originScale * 1.1f, 0.2f).SetEase(Ease.OutBack);

        outlinable.enabled = true;

        outlineTween = outlinable.OutlineParameters.DODilateShift(0f, 0.5f).SetLoops(-1, LoopType.Yoyo).OnStart(() => outlinable.OutlineParameters.DilateShift = 1);
    }

    public void HighlightThisPointerOFF()
    {
        transform.DOScale(originScale, 0.2f).SetEase(Ease.OutBack);

        if (outlineTween != null)
        {
            outlineTween.Kill();
            outlineTween = null;
        }

        outlinable.enabled = false;
    }

    public void SetTrailLength()
    {
        trailRenderer.time = MainManager.instance.lastFever ? 0.2f : 0.8f;
    }

    public void ChangeMesh()
    {
        switch (ITRManager.instance.pointerType)
        {
            case pointerType.type1:
                GetComponentInChildren<MeshFilter>().mesh = meshtype1;
                GetComponentInChildren<MeshCollider>().sharedMesh = meshtype1;

                break;

            case pointerType.type2:
                GetComponentInChildren<MeshFilter>().mesh = meshtype2;
                GetComponentInChildren<MeshCollider>().sharedMesh = meshtype2;

                break;
        }
    }

    public void ChangeSkin()
    {
        defaultSkin.collider.enabled = false;
        antiqueSkin.collider.enabled = false;

        if (defaultSkin.effect != null)
            defaultSkin.effect.SetActive(false);
        if (antiqueSkin.effect != null)
            antiqueSkin.effect.SetActive(false);

        switch (MainManager.instance.pointerSkinType)
        {
            case PointerSkinType.Default:
                _meshFilter.mesh = defaultSkin.mesh;
                GetComponentInChildren<MeshCollider>().sharedMesh = defaultSkin.mesh;
                _renderer.materials = defaultSkin.materials;
                defaultSkin.collider.enabled = true;
                if (defaultSkin.effect != null)
                    defaultSkin.effect.SetActive(true);
                break;

            case PointerSkinType.Antique:
                _meshFilter.mesh = antiqueSkin.mesh;
                GetComponentInChildren<MeshCollider>().sharedMesh = antiqueSkin.mesh;
                _renderer.materials = antiqueSkin.materials;
                antiqueSkin.collider.enabled = true;
                if (antiqueSkin.effect != null)
                    antiqueSkin.effect.SetActive(true);
                break;
        }
    }
}
