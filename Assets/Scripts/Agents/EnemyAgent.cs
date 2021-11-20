using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] NavMeshAgent navMeshAgent;
    
    [SerializeField] float raycastRadius = 20;
    
    [SerializeField] float AngleVisibility = 45.0f;
    private Vector3 walkPosition;
    private ResourceAgent target;
    private float currentHealth;

    protected enum ENEMY_STATE{
        PATROL, BATTLE
    }

    protected ENEMY_STATE currentState = ENEMY_STATE.PATROL;
    // Start is called before the first frame update
    
    void LookAroundAngle(){

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

    void Start()
    {
        currentHealth = maxHealth;
        SetRandomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState){
            case ENEMY_STATE.PATROL:
                LookAroundAngle();
                navMeshAgent.SetDestination(walkPosition);
                if(Vector3.Distance(transform.position, walkPosition) <= 1){
                    SetRandomPosition();
                }
                break;
            case ENEMY_STATE.BATTLE:

                if(target == null){
                    currentState = ENEMY_STATE.PATROL;
                }else{
                    // TODO attack
                }
                break;
        }
    }

    private void SetRandomPosition(){

        walkPosition = new Vector3(Random.Range(-5, 5),0, Random.Range(-5, 5));
    }

    public void TakeDamge(float amount){
        currentHealth -= amount;
        if(currentHealth <= 0){
            Destroy(gameObject);
        }
    }
}
