using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    // configurações
    [SerializeField] float raycastRadius = 20;
    
    [SerializeField] float AngleVisibility = 45.0f;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float attackPower = 5;
    [SerializeField] float attackRange = 6;
    [SerializeField] float attackDelay = 2;
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] AudioSource audioSource;


    // eventos
    public System.Action OnEnemyLost;
    public System.Action OnDied;


    // variaveis
    private float currentHealth;
    private ResourceAgent target;
    private float attackTimer = 0;
    private Vector3 spawnPosition;


    protected enum ENEMY_STATE
    {
        PATROL, BATTLE
    }

    protected ENEMY_STATE currentState = ENEMY_STATE.PATROL;
    
    void LookAroundAngle()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, raycastRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 collisionDir = colliders[i].transform.position - transform.position;
            float angle = Vector3.Angle(collisionDir.normalized, transform.forward);

            if (angle > AngleVisibility)
                continue;

            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("ResourceAgent"))
            {
                target = colliders[i].gameObject.GetComponent<ResourceAgent>();
                currentState = ENEMY_STATE.BATTLE;
            }
        }
    }
    public void TakeDamge(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (OnDied != null)
            {
                animator.SetBool("Died", true);
                OnDied();
            }
        }
    }

    public void AttackTarget(Transform target)
    {
        if (Vector3.Distance(transform.position, target.position) >= attackRange)
        {
            navMeshAgent.SetDestination(target.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", false);
        }
        else
        {
            navMeshAgent.SetDestination(transform.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", true);
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                target.GetComponent<ResourceAgent>().TakeDamage(attackPower);
                attackTimer = 0;
            }
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        SetRandomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case ENEMY_STATE.PATROL:
                    LookAroundAngle();
                    navMeshAgent.SetDestination(spawnPosition);

                    if(Vector3.Distance(transform.position, spawnPosition) <= 1)
                    {
                        SetRandomPosition();
                    }
                    break;
                case ENEMY_STATE.BATTLE:

                    if(target == null)
                    {
                        currentState = ENEMY_STATE.PATROL;
                    }
                    else
                    {
                        currentState = ENEMY_STATE.BATTLE;
                        AttackTarget(target.transform);
                    }
                    break;
        }
    }

    private void SetRandomPosition()
    {
        spawnPosition = new Vector3(Random.Range(-5, 5),0, Random.Range(-5, 5));
        //Transform batata = new 
    }

}
