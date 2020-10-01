using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public float checkRate = 1.0f;
    public float nearbyEnemyAttackRange;

    public LayerMask unitLayerMask;

    private PlayerAI playerAI;
    private Unit unit;

    public void InitializeAI (PlayerAI playerAI, Unit unit)
    {
        this.playerAI = playerAI;
        this.unit = unit;
    }

    void Start ()
    {
        InvokeRepeating("Check", 0.0f, checkRate);
    }

    void Check ()
    {
        // check if we have nearby enemies - if so, attack them
        if(unit.state != UnitState.Attack && unit.state != UnitState.MoveToEnemy)
        {
            Unit potentialEnemy = CheckForNearbyEnemies();

            if(potentialEnemy != null)
                unit.AttackUnit(potentialEnemy);
        }



        // if we're doing nothing, find a new resource
        if (unit.state == UnitState.Idle)
            FindNewResource();
        // if we're moving to a resource which is destroyed, find a new one
        else if(unit.state == UnitState.MoveToResource && unit.curResourceSource == null)
            FindNewResource();
    }

    // find a nearby resource to gather from
    void FindNewResource ()
    {
        ResourceSource resourceToGet = playerAI.GetClosestResource(transform.position);

        if(resourceToGet != null)
        {
            unit.GatherResource(resourceToGet, UnitMover.GetUnitDestinationAroundResource(resourceToGet.transform.position));
        }
        else
            PursueEnemy();
    }

    // checks for nearby enemies with a sphere cast
    Unit CheckForNearbyEnemies ()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, nearbyEnemyAttackRange, Vector3.up, unitLayerMask);

        GameObject closest = null;
        float closestDist = 0.0f;

        for(int x = 0; x < hits.Length; x++)
        {
            // skip if this is us
            if(hits[x].collider.gameObject == gameObject)
                continue;

            // is this a teammate?
            if(unit.player.IsMyUnit(hits[x].collider.GetComponent<Unit>()))
                continue;

            if(!closest || Vector3.Distance(transform.position, hits[x].transform.position) < closestDist)
            {
                closest = hits[x].collider.gameObject;
                closestDist = Vector3.Distance(transform.position, hits[x].transform.position);
            }
        }

        if(closest != null)
            return closest.GetComponent<Unit>();
        else
            return null;
    }

    // called when there's no more resources - chase after a random enemy
    void PursueEnemy ()
    {
        Player enemyPlayer = GameManager.instance.GetRandomEnemyPlayer(unit.player);

        if(enemyPlayer.units.Count > 0)
            unit.AttackUnit(enemyPlayer.units[Random.Range(0, enemyPlayer.units.Count)]);
    }
}