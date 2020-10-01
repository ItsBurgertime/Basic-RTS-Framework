using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommander : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;
    public LayerMask layerMask;

    // components
    private UnitSelection unitSelection;
    private Camera cam;

    void Awake ()
    {
        // get the components
        unitSelection = GetComponent<UnitSelection>();
        cam = Camera.main;
    }

    void Update ()
    {
        // did we press down our right mouse button and do we have units selected?
        if(Input.GetMouseButtonDown(1) && unitSelection.HasUnitsSelected())
        {
            // shoot a raycast from our mouse, to see what we hit
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // cache the selected units in an array
            Unit[] selectedUnits = unitSelection.GetSelectedUnits();

            // shoot the raycast
            if(Physics.Raycast(ray, out hit, 100, layerMask))
            {
                unitSelection.RemoveNullUnitsFromSelection();

                // are we clicking on the ground?
                if(hit.collider.CompareTag("Ground"))
                {
                    UnitsMoveToPosition(hit.point, selectedUnits);
                    CreateSelectionMarker(hit.point, false);
                }

                // did we click on a resource?
                else if(hit.collider.CompareTag("Resource"))
                {
                    UnitsGatherResource(hit.collider.GetComponent<ResourceSource>(), selectedUnits);
                    CreateSelectionMarker(hit.collider.transform.position, true);
                }

                // did we click on an enemy?
                else if(hit.collider.CompareTag("Unit"))
                {
                    Unit enemy = hit.collider.gameObject.GetComponent<Unit>();

                    if(!Player.me.IsMyUnit(enemy))
                    {
                        UnitsAttackEnemy(enemy, selectedUnits);
                        CreateSelectionMarker(enemy.transform.position, false);
                    }
                }
            }
        }
    }

    // called when we command units to move somewhere
    void UnitsMoveToPosition (Vector3 movePos, Unit[] units)
    {
        Vector3[] destinations = UnitMover.GetUnitGroupDestinations(movePos, units.Length, 2);

        for(int x = 0; x < units.Length; x++)
        {
            units[x].MoveToPosition(destinations[x]);
        }
    }

    // called when we command units to gather a resource
    void UnitsGatherResource (ResourceSource resource, Unit[] units)
    {
        // are just selecting 1 unit?
        if(units.Length == 1)
        {
            units[0].GatherResource(resource, UnitMover.GetUnitDestinationAroundResource(resource.transform.position));
        }
        // otherwise, calculate the unit group formation
        else
        {
            Vector3[] destinations = UnitMover.GetUnitGroupDestinationsAroundResource(resource.transform.position, units.Length);

            for(int x = 0; x < units.Length; x++)
            {
                units[x].GatherResource(resource, destinations[x]);
            }
        }
    }

    // called when we command units to attack an enemy
    void UnitsAttackEnemy (Unit target, Unit[] units)
    {
        for(int x = 0; x < units.Length; x++)
            units[x].AttackUnit(target);
    }

    // creates a new selection marker visual at the given position
    void CreateSelectionMarker (Vector3 pos, bool large)
    {
        GameObject marker = Instantiate(selectionMarkerPrefab, new Vector3(pos.x, 0.01f, pos.z), Quaternion.identity);

        if(large)
            marker.transform.localScale = Vector3.one * 3;
    }
}