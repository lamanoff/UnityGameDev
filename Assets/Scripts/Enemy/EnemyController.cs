using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;

class EnemyController : MonoBehaviour
{
    private GameObject target;
    private PlayerController playerController;
    private NavMeshAgent agent;
    private bool allowAttack = false;
    private float attackTimer;
    private RangeAttack rangeAttack;
    public event Action<float> GetDamage;
    public float Damage { get; set; }
    public float DamageDeviationRatio = 5;
    public float HitsPerSecond = 1;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        playerController = target.GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        rangeAttack = GetComponent<RangeAttack>();
        if (rangeAttack != null)
            rangeAttack.Damage = Damage;
    }

    public void TakeDamage(float damage)
        => GetDamage?.Invoke(damage);

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            allowAttack = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            allowAttack = false;
    }

    private void Attack()
    {
        if (attackTimer > 0)
            return;
        var currentDeviation = Damage / DamageDeviationRatio;
        var damage = Damage + UnityEngine.Random.Range(-currentDeviation, currentDeviation);
        if (rangeAttack == null)
            playerController.TakeDamage(transform.position, damage);
        else
            rangeAttack.Attack();
        attackTimer = 1 / HitsPerSecond;
    }

    private void SetDestination()
    {
        if (rangeAttack == null)
            agent.SetDestination(target.transform.position);
        else
        {
            var distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            if (distanceToTarget > rangeAttack.Range)
            {
                if (agent.enabled == false)
                    agent.enabled = true;
                agent.SetDestination(target.transform.position);
                allowAttack = false;
            }
            else
            {
                if (agent.enabled == true)
                    agent.enabled = false;
                gameObject.transform.LookAt(target.transform);
                allowAttack = true;
            }
        }
    }

    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        if (target == null)
            return;
        if (allowAttack)
            Attack();
        SetDestination();
    }
}
