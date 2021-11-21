using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStateMachine : MonoBehaviour
{
    // referencias
    [SerializeField] ResourceAgent resourceAgent;
    [SerializeField] Transform enemyTransform;
    [SerializeField] Color miningColor;
    [SerializeField] Color deliverColor;
    [SerializeField] Color fleeingColor;
    [SerializeField] Color healingColor;

    [SerializeField] Color battleColor;
    private Color stateColor;

    enum State{
        MINE,
        DELIVER,
        FLEEING_TO_BASE,
        HEAL,
        BATTLE
    }

    private State currentState;

    private void StartMineState(){
        currentState = State.MINE;
        resourceAgent.ClearTargetResource();
        stateColor = miningColor;

        Resource.ResourceType typeRequired = resourceAgent.RequestResource();
        resourceAgent.SetResource(typeRequired);
    }

    private void StartDeliverState(){
        currentState = State.DELIVER;
        stateColor = deliverColor;
    }

    private void StartBattleState(){
        if(enemyTransform != null){
            currentState = State.BATTLE;
            stateColor = battleColor;
        }

    }

    private void StartFleeingState(){
        currentState = State.FLEEING_TO_BASE;
        stateColor = fleeingColor;
    }

    private void StartHealState(){
        currentState = State.HEAL;
        stateColor = healingColor;
    }


    // Start is called before the first frame update
    void Start()
    {
        SetupListeners();
        StartMineState();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState){
            case State.MINE:
                resourceAgent.CollectResource();
                break;
            case State.DELIVER:
                resourceAgent.DeliverResource();
                break;
            case State.BATTLE:
                if(enemyTransform == null)
                    CalculateFuzzyLogic();
                else
                    resourceAgent.AttackTarget(enemyTransform);
                break;
            case State.FLEEING_TO_BASE:
                resourceAgent.GoToBase();
                break;
            case State.HEAL:
                resourceAgent.Heal();
                if(resourceAgent.IsMaxHealth()){
                    CalculateFuzzyLogic();
                }
                break;
        }
    }

    // Listeners
    private void SetupListeners(){
        resourceAgent.OnResourceCollected = OnResourceCollected;
        resourceAgent.OnResourceDelivered = OnResourceDelivered;
        resourceAgent.OnEnemyFound = OnEnemyFound;
        resourceAgent.OnEnemyLost = OnEnemyLost;
        resourceAgent.OnHealthChanged = OnHealthChanged;
        resourceAgent.OnBaseReached = OnBaseReached;
        resourceAgent.OnDied = OnDeath;
    }

    private void DisableListeners(){
        resourceAgent.OnResourceCollected = null;
        resourceAgent.OnResourceDelivered = null;
        resourceAgent.OnEnemyFound = null;
        resourceAgent.OnEnemyLost = null;
        resourceAgent.OnHealthChanged = null;
        resourceAgent.OnBaseReached = null;
        resourceAgent.OnDied = null;
    }

    private void OnBaseReached(){
        if(currentState == State.FLEEING_TO_BASE){
            StartHealState();
        }
    }

    private void OnHealthChanged(float currentHealth){
        if(currentState == State.DELIVER) return;

        if(currentState == State.HEAL){
            if(resourceAgent.IsMaxHealth()){
                StartMineState();
            }
        }else{
            
            CalculateFuzzyLogic();
        }
    }

    private void OnEnemyFound(Transform target){
        enemyTransform = target;
        CalculateFuzzyLogic();
    }

    private void OnEnemyLost(){
        enemyTransform = null;
        CalculateFuzzyLogic();
    }

    private void OnResourceCollected(){
        StartDeliverState();
    }

    private void OnResourceDelivered(){
        StartMineState();
    }

    private void OnDeath(){
        DisableListeners();
        Destroy(gameObject);
    }

    private void DecideFuzzy(float minValue, float maxValue, float currentValue ,State lowerState, State higherState){
        float range = maxValue - minValue;
        float lowerPossibility = 1 - ((currentValue - minValue)/(maxValue - minValue));

        if(Random.value <= lowerPossibility){
            SetState(lowerState);
        }else{
            SetState(higherState);
        }        
    }

    private void CalculateFuzzyLogic(){
        if(currentState == State.DELIVER || currentState == State.FLEEING_TO_BASE) return; // se está em deliver não muda o stado
        float healthPercent = resourceAgent.GetHealthPercent();

        if(currentState == State.MINE){
            // entre 50 e 70% não muda o estado atual
            if(healthPercent > 0.8f){
                StartBattleState();   
            }else if(healthPercent > 0.7f && healthPercent <= 0.8f){
                DecideFuzzy(0.7f, 0.8f, healthPercent, State.MINE, State.BATTLE);
            }else if(healthPercent > 0.3f && healthPercent <= 0.5f){
                DecideFuzzy(0.3f, 0.5f, healthPercent, State.HEAL, State.MINE);
            } else if(healthPercent <= 0.3f){
                StartFleeingState();
            }

        }else if(enemyTransform == null){ // se não há inimigo, vai minerar
            StartMineState();
        }else{
            if(healthPercent > 0.5f){
                StartBattleState();
            }else if(healthPercent > 0.2f && healthPercent <= 0.5f){
                DecideFuzzy(0.2f, 0.5f, healthPercent, State.HEAL, State.BATTLE);
            }else{
                StartFleeingState();
            }
        }
    }

    private void SetState(State newState){
        switch(newState){
            case State.BATTLE:
                StartBattleState();
                break;
            case State.FLEEING_TO_BASE:
            case State.HEAL:
                if(currentState != State.HEAL && currentState != State.FLEEING_TO_BASE)
                    StartFleeingState();
                break;
            case State.MINE:
                if(currentState != State.MINE)
                    StartMineState();
                break;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = stateColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * 5, 3);
    }
}
