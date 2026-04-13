using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using UnityEngine;

public class BulletRangeWeapon : MonoBehaviour
{
    public GameObject effectObj;
    private ParticleSystem hit;
    private ParticleSystem flash;

    private float damage;
    private Transform target;
    private float speed;

    private Action<GameObject> returnToPool;
    public GameObject playerOwner;

    private void Start()
    {
        hit = effectObj.transform.GetChild(2).GetComponent<ParticleSystem>();
        flash = effectObj.transform.GetChild(3).GetComponent<ParticleSystem>();
    }

    public void SetStart(float dmg, Transform target, float speed, Action<GameObject> returnToPool)
    {
        damage = dmg;
        this.target = target;
        this.speed = speed;
        this.returnToPool = returnToPool;
        playerOwner = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (target == null)
        {
            returnToPool?.Invoke(gameObject);
            return;
        }

        // Bay theo target
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // Xoay mũi tên về phía target
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = lookRot * Quaternion.Euler(-90f, 0f, 0f);
        }

        if (dir.magnitude <= distanceThisFrame)
        {
            StartCoroutine(HitTarget());
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }



    IEnumerator HitTarget()
    {
        hit.Play();
        flash.Play();
        yield return new WaitForSeconds(0.12f);
        // Gây damage cho enemy ở đây
        target.gameObject.GetComponent<Enemy>()?.TakeDamage(damage, playerOwner); // Giả sử người chơi bắn
        // Trả lại bullet vào pool
        returnToPool?.Invoke(gameObject);
    }
}
