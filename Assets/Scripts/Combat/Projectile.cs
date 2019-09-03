using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{   
    [SerializeField] float speed = 1f;
    [SerializeField] bool isHoming = false;
    [SerializeField] GameObject hitImpact = null;
    [SerializeField] float maxLifeTime = 2f;
    [SerializeField] GameObject[] destroyOnHit;
    [SerializeField] float lifeAfterImpact = 0.2f;
    [SerializeField] UnityEvent onHit;

    Transform target = null;
    GameObject instigator = null;
    float damage;
    float time = 0;

    private void Start()
    {
        transform.LookAt(GetAimPosition());
    }

    void Update()
    {
        if(isHoming && !target.GetComponent<Health>().IsDead())
        {
            transform.LookAt(GetAimPosition());
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private Vector3 GetAimPosition()
    {
        CapsuleCollider collider = target.GetComponent<CapsuleCollider>();
        if (!collider) return target.position;
        return target.position + Vector3.up * (collider.height / 2);
    }

    public void SetTarget(Transform target, GameObject instigator, float damage)
    {
        this.target = target;
        this.damage = damage;
        this.instigator = instigator;

        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject != target.gameObject) return;
        other.GetComponent<Health>().TakeDamage(instigator, damage);
        onHit.Invoke();
        
        speed = 0;

        if(hitImpact)
        {
            Instantiate(hitImpact, other.ClosestPoint(transform.position), transform.rotation);
        }

        foreach(GameObject toDestroy in destroyOnHit)
        {
            Destroy(toDestroy);
        }
        Destroy(gameObject, lifeAfterImpact);
    }
}
 