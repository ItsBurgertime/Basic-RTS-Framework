using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum UnitState
{
    Idle,
    Move,
    MoveToResource,
    Gather,
    MoveToEnemy,
    Attack
}

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public UnitState state;

    public int curHp;
    public int maxHp;

    public int minAttackDamage;
    public int maxAttackDamage;

    public float attackRate;
    private float lastAttackTime;

    public float attackDistance;

    public float pathUpdateRate = 1.0f;
    private float lastPathUpdateTime;

    public int gatherAmount;
    public float gatherRate;
    private float lastGatherTime;

    public ResourceSource curResourceSource;
    private Unit curEnemyTarget;

    [Header("Components")]
    public GameObject selectionVisual;
    private NavMeshAgent navAgent;
    public UnitHealthBar healthBar;

    public Player player;

    // events
    [System.Serializable]
    public class StateChangeEvent : UnityEvent<UnitState> { }
    public StateChangeEvent onStateChange;

    void Start ()
    {
        // get the components
        navAgent = GetComponent<NavMeshAgent>();

        SetState(UnitState.Idle);
    }

    void SetState (UnitState toState)
    {
        state = toState;

        // calling the event
        if(onStateChange != null)
            onStateChange.Invoke(state);

        if(toState == UnitState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    void Update ()
    {
        switch(state)
        {
            case UnitState.Move:
            {
                MoveUpdate();
                break;
            }
            case UnitState.MoveToResource:
            {
                MoveToResourceUpdate();
                break;
            }
            case UnitState.Gather:
            {
                GatherUpdate();
                break;
            }
            case UnitState.MoveToEnemy:
            {
                MoveToEnemyUpdate();
                break;
            }
            case UnitState.Attack:
            {
                AttackUpdate();
                break;
            }
        }
    }

    // called every frame the 'Move' state is active
    void MoveUpdate ()
    {
        if(Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Idle);
    }

    // called every frame the 'MoveToResource' state is active
    void MoveToResourceUpdate ()
    {
        if(curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }
       // Debug.Log(Vector3.Distance(transform.position, navAgent.destination));
        if (Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
        {
            SetState(UnitState.Gather);

            return;

        }
    }

    // called every frame the 'Gather' state is active
    void GatherUpdate ()
    {
        if(curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        LookAt(curResourceSource.transform.position);

        if(Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time;
            curResourceSource.GatherResource(gatherAmount, player);
        }
    }

    // called every frame the 'MoveToEnemy' state is active
    void MoveToEnemyUpdate ()
    {
        // if our target is dead, go idle
        if(curEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if(Time.time - lastPathUpdateTime > pathUpdateRate)
        {
            lastPathUpdateTime = Time.time;
            navAgent.isStopped = false;
            navAgent.SetDestination(curEnemyTarget.transform.position);
        }

        if(Vector3.Distance(transform.position, curEnemyTarget.transform.position) <= attackDistance)
            SetState(UnitState.Attack);
    }

    // called every frame the 'Attack' state is active
    void AttackUpdate ()
    {
        // if our target is dead, go idle
        if(curEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        // if we're still moving, stop
        if(!navAgent.isStopped)
            navAgent.isStopped = true;

        // attack every 'attackRate' seconds
        if(Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            curEnemyTarget.TakeDamage(Random.Range(minAttackDamage, maxAttackDamage + 1));
        }

        // look at the enemy
        LookAt(curEnemyTarget.transform.position);

        // if we're too far away, move towards the enemy
        if(Vector3.Distance(transform.position, curEnemyTarget.transform.position) > attackDistance)
            SetState(UnitState.MoveToEnemy);
    }

    // called when an enemy unit attacks us
    public void TakeDamage (int damage)
    {
        curHp -= damage;

        if(curHp <= 0)
            Die();

        healthBar.UpdateHealthBar(curHp, maxHp);
    }

    // called when our health reaches 0
    void Die ()
    {
        player.units.Remove(this);

        GameManager.instance.UnitDeathCheck();

        Destroy(gameObject);
    }

    // moves the unit to a specific position
    public void MoveToPosition (Vector3 pos)
    {
        SetState(UnitState.Move);

        navAgent.isStopped = false;
        navAgent.SetDestination(pos);
    }

    // move to a resource and begin to gather it
    public void GatherResource (ResourceSource resource, Vector3 pos)
    {
        curResourceSource = resource;
        SetState(UnitState.MoveToResource);

        navAgent.isStopped = false;
        navAgent.SetDestination(pos);
        SetState(UnitState.Gather);
    }

    // move to an enemy unit and attack them
    public void AttackUnit (Unit target)
    {
        curEnemyTarget = target;
        SetState(UnitState.MoveToEnemy);
    }

    // toggles the selection ring around our feet
    public void ToggleSelectionVisual (bool selected)
    {
        if(selectionVisual != null)
            selectionVisual.SetActive(selected);
    }

    // rotate to face the given position
    void LookAt (Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}