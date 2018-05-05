using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangeAttack : MonoBehaviour
{
    public GameObject Bullet;
    public float Range;
    public float Lifetime;
    public float Damage { get; set; }

    public void Attack()
    {
        if (Bullet == null)
            throw new NullReferenceException("The projectile was not installed!");
        var instance = Instantiate(Bullet, transform, false);
        instance.GetComponent<MagicHandler>().Damage = Damage;
        var velocityRatio = gameObject.GetComponent<NavMeshAgent>().speed;
        instance.GetComponent<Rigidbody>().AddForce(instance.transform.forward * 100 * velocityRatio, ForceMode.Acceleration);
        instance.transform.SetParent(transform.parent, true);
        instance.transform.localScale = Bullet.transform.localScale;
        instance.transform.position -= new Vector3(0, 0.3f, 0);
        Destroy(instance, Lifetime);
    }
}
