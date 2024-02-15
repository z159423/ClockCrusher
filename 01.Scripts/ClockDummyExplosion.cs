using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ClockDummyExplosion : MonoBehaviour
{
    public Rigidbody[] rigidbodies;

    public float explosionForce = 10f;  // 폭발 힘
    public float explosionRadius = 5f;  // 폭발 반경

    public float minRotationForce = 5f;  // 최소 힘
    public float maxRotationForce = 10f; // 최대 힘

    public void ExplostionParts()
    {
        foreach (var rigid in rigidbodies)
        {
            Vector3 explosionDirection = rigid.transform.position - transform.position;

            // 폭발 힘을 적용 (방향이 반대로 가도록 음수를 곱함)
            rigid.AddForce(explosionDirection.normalized * explosionForce, ForceMode.Impulse);

            // 랜덤한 회전 축 생성
            Vector3 randomRotationAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            // 랜덤한 회전 힘 생성
            float randomForce = Random.Range(minRotationForce, maxRotationForce);

            // Rigidbody에 회전 힘을 적용
            rigid.AddTorque(randomRotationAxis * randomForce, ForceMode.Impulse);
        }
    }

    [Button]
    public void FindRigidbody()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }
}
