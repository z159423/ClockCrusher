using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttractorCustom : MonoBehaviour
{
    [SerializeField] Coffee.UIExtensions.UIParticleAttractor ui_ParticleAttractor;
    [SerializeField] public ParticleSystem particle;
    [SerializeField] public RectTransform attractorTarget;
    [SerializeField] public RectTransform startPoint;

    public void InitParticle(Material mat, int cycle, ParticleSystem.Burst burst)
    {
        var renderer = particle.GetComponent<ParticleSystemRenderer>().material = mat;
        var emission = particle.emission;

        for (int i = 0; i <= cycle; i++)
        {
            emission.SetBurst(i, burst);
        }
    }

    // public void Init(Transform target, int item, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null, Transform _startPoint = null)
    // {
    //     attractorTarget.SetParent(target);
    //     attractorTarget.anchoredPosition = Vector2.zero;

    //     if (_startPoint != null)
    //     {
    //         // startPoint.transform.position = 
    //     }

    //     var renderer = particle.GetComponent<ParticleSystemRenderer>().material = item.candy.particleMat;

    //     var emission = particle.emission;

    //     short cycle = (short)Mathf.Clamp((short)(item.count / 10), 1, 7);

    //     for (int i = 0; i <= cycle; i++)
    //     {
    //         int count = (cycle > i) ? 10 : item.count % 10;
    //         emission.SetBurst(i, new ParticleSystem.Burst(i * 0.8f / ((float)item.count / 5f), (short)count, (short)count, 1, 0.8f / ((float)item.count / 5f)));
    //     }

    //     if (onAttract != null)
    //         ui_ParticleAttractor.m_OnAttracted.AddListener(onAttract);

    //     particle.Play();

    //     if (OnCompleteParticle != null)
    //         RunManager.instance.TaskWaitUntil(() =>
    //         {
    //             OnCompleteParticle.Invoke(); Destroy(gameObject);
    //         }, () => (!particle.IsAlive()));
    // }

    // public void Init(CandyItem item, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    // {
    //     // attractorTarget.SetParent(target);
    //     // attractorTarget.anchoredPosition = Vector2.zero;

    //     // if (_startPoint != null)
    //     // {
    //     //     // startPoint.transform.position = 
    //     // }

    //     var renderer = particle.GetComponent<ParticleSystemRenderer>().material = item.candy.particleMat;

    //     var emission = particle.emission;

    //     short cycle = (short)Mathf.Clamp((short)(item.count / 10), 1, 7);

    //     for (int i = 0; i <= cycle; i++)
    //     {
    //         int count = (cycle > i) ? 10 : item.count % 10;
    //         emission.SetBurst(i, new ParticleSystem.Burst(i * 0.8f / ((float)item.count / 5f), (short)count, (short)count, 1, 0.8f / ((float)item.count / 5f)));
    //     }

    //     if (onAttract != null)
    //         ui_ParticleAttractor.m_OnAttracted.AddListener(onAttract);

    //     particle.Play();

    //     if (OnCompleteParticle != null)
    //         RunManager.instance.TaskWaitUntil(() =>
    //         {
    //             OnCompleteParticle.Invoke(); Destroy(gameObject);
    //         }, () => (!particle.IsAlive()));
    // }

    public void StartEmit(Transform target, Vector2 startPos, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    {
        startPoint.anchoredPosition = startPos;
        attractorTarget.transform.position = target.position;

        particle.Play();

        if (onAttract != null)
            ui_ParticleAttractor.m_OnAttracted.AddListener(onAttract);

        if (OnCompleteParticle != null)
            MainManager.instance.TaskWaitUntil(() =>
            {
                OnCompleteParticle.Invoke(); Destroy(gameObject);
            }, () => (!particle.IsAlive()));
    }

    public void StartEmit(Transform target, Transform startPos, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    {
        if (startPoint != null && startPos != null)
            startPoint.transform.position = startPos.position;
        if (attractorTarget != null && target != null)
            attractorTarget.transform.position = target.position;

        particle.Play();

        if (onAttract != null)
            ui_ParticleAttractor.m_OnAttracted.AddListener(onAttract);

        if (OnCompleteParticle != null)
            MainManager.instance.TaskWaitUntil(() =>
            {
                OnCompleteParticle.Invoke(); Destroy(gameObject);
            }, () => (!particle.IsAlive()));
    }

    // public void Init2(Transform end, Transform start, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    // {
    //     startPoint.transform.SetParent(start);
    //     startPoint.anchoredPosition3D = Vector3.zero;
    //     startPoint.transform.localScale = Vector3.one * 100;

    //     attractorTarget.SetParent(end);
    //     attractorTarget.anchoredPosition3D = Vector2.zero;

    //     if (onAttract != null)
    //         ui_ParticleAttractor.m_OnAttracted.AddListener(onAttract);

    //     particle.Play();

    //     if (OnCompleteParticle != null)
    //         RunManager.instance.TaskWaitUntil(() =>
    //         {
    //             OnCompleteParticle.Invoke(); Destroy(gameObject);
    //         }, () => (!particle.IsAlive()));
    // }
}
