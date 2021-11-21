using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResourceAgent : MonoBehaviour
{
    // configurações
    [SerializeField] float raycastRadius = 20;
    
    [SerializeField] float AngleVisibility = 45.0f;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float healingFactor = 1;
    [SerializeField] float attackPower = 5;
    [SerializeField] float miningDelay = 5;
    [SerializeField] float attackRange = 6;
    [SerializeField] float baseMinDistance = 8;
    [SerializeField] float attackDelay = 2;
    [SerializeField] Animator animator;
    [SerializeField] BaseController baseController;
    [SerializeField] NavMeshAgent navMeshAgent;

    // eventos
    public System.Action<Transform> OnEnemyFound;
    public System.Action OnEnemyLost;
    public System.Action<float> OnHealthChanged;
    public System.Action OnDied;
    public System.Action OnBaseReached;
    public System.Action OnResourceCollected;
    public System.Action OnResourceDelivered;

    // variables
    private float currentHealth;
    private Resource collectedResource;
    private ResourceSpawn resourceSpawn;
    private float resourceTimer = 0;
    private float attackTimer = 0;
    private bool isEnemyClose = false;
    
    public void SetBase(BaseController baseController){
        this.baseController = baseController;
    }

    void LookAroundAngle()
    {
        // Procura pelo objetos a sua volta
        bool enemyFound = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, raycastRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 collisionDir = colliders[i].transform.position - transform.position;
            float angle = Vector3.Angle(collisionDir.normalized, transform.forward);

            if (angle > AngleVisibility)
                continue;

            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Enemy") && OnEnemyFound != null)
            {
                enemyFound = true;
                isEnemyClose = true;
                OnEnemyFound(colliders[i].transform);
            }
        }

        if (isEnemyClose && enemyFound == false && OnEnemyLost != null){
            OnEnemyLost();
            isEnemyClose = false;
        }
    }

    public void TakeDamage(float amount){
        currentHealth -= amount;
        if(currentHealth <= 0){
            currentHealth = 0;

            if(OnDied != null){
                OnDied();
            }
        }

        if(OnHealthChanged != null){
            OnHealthChanged(currentHealth);
        }
    }

    public void SetResource(Resource.ResourceType targetResource){
        if(resourceSpawn == null){
            resourceSpawn = baseController.GetResourceSpawnOfType(targetResource);
        }
    }

    public void CollectResource(){
        if(resourceSpawn == null) return;

        if(Vector3.Distance(transform.position, resourceSpawn.transform.position) >= attackRange){
            navMeshAgent.SetDestination(resourceSpawn.transform.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", false);
        }else{
            navMeshAgent.SetDestination(transform.position);
            resourceTimer += Time.deltaTime;

            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", true);

            if(resourceTimer >= miningDelay){
                collectedResource = resourceSpawn.SpawnResource();
                if(OnResourceCollected != null){
                    OnResourceCollected();
                }
            }
        }
    }

    public void DeliverResource(){
        navMeshAgent.SetDestination(baseController.transform.position);
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);

        if(Vector3.Distance(transform.position, baseController.transform.position) < baseMinDistance){
            baseController.ReceiveResource(collectedResource);
            ClearTargetResource();

            if(OnResourceDelivered != null)
                OnResourceDelivered();
        }
    }

    public void ClearTargetResource(){
        resourceSpawn = null;
        resourceTimer = 0;
    }

    public void GoToBase(){
        navMeshAgent.SetDestination(baseController.transform.position);
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);

        if(Vector3.Distance(transform.position, baseController.transform.position) < baseMinDistance){
            navMeshAgent.SetDestination(transform.position);

            if(OnBaseReached != null){
                OnBaseReached();
            }
        }
    }

    public float GetHealthPercent(){
        return currentHealth/maxHealth;
    }

    public Resource.ResourceType RequestResource(){
        return baseController.GetResourceInNeed();
    }

    public void Heal(){
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
        
        currentHealth = Mathf.Clamp(currentHealth + healingFactor * Time.deltaTime, 0, maxHealth);
        if(OnHealthChanged != null){
            OnHealthChanged(currentHealth);
        }
    }

    public bool IsMaxHealth(){
        return currentHealth.Equals(maxHealth);
    }

    public void AttackTarget(Transform target){
        if(Vector3.Distance(transform.position, target.position) >= attackRange){
            navMeshAgent.SetDestination(target.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", false);
        }else{
            navMeshAgent.SetDestination(transform.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsAttacking", true);
            attackTimer += Time.deltaTime;

            if(attackTimer >= attackDelay){
                target.GetComponent<EnemyAgent>().TakeDamge(attackPower);
                attackTimer = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        LookAroundAngle();
        if(Input.GetKeyDown(KeyCode.K)){
            TakeDamage(5);
        }
    }
}
