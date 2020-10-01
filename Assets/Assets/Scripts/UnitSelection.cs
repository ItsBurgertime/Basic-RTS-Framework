using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public RectTransform selectionBox;
    public LayerMask unitLayerMask;

    private List<Unit> selectedUnits = new List<Unit>();
    private Vector2 startPos;

    // components
    private Camera cam;
    private Player player;

    void Awake ()
    {
        // get the components
        cam = Camera.main;
        player = GetComponent<Player>();
    }

    void Update ()
    {
        // mouse down
        if(Input.GetMouseButtonDown(0))
        {
            ToggleSelectionVisual(false);
            selectedUnits = new List<Unit>();

            TrySelect(Input.mousePosition);
            startPos = Input.mousePosition;
        }

        // mouse up
        if(Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }

        // mouse held down
        if(Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    // called when we click on a unit
    void TrySelect (Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100, unitLayerMask))
        {
            Unit unit = hit.collider.GetComponent<Unit>();

            if(player.IsMyUnit(unit))
            {
                selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }
    }

    // called when we are creating a selection box
    void UpdateSelectionBox (Vector2 curMousePos)
    {
        if(!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
    }

    // called when we release the selection box
    void ReleaseSelectionBox ()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach(Unit unit in player.units)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

            if(screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }
    }

    // removes all destroyed or missing units from the selected list
    public void RemoveNullUnitsFromSelection ()
    {
        for(int x = 0; x < selectedUnits.Count; x++)
        {
            if(selectedUnits[x] == null)
                selectedUnits.RemoveAt(x);
        }
    }

    // toggles the selected units selection visual
    void ToggleSelectionVisual (bool selected)
    {
        foreach(Unit unit in selectedUnits)
        {
            unit.ToggleSelectionVisual(selected);
        }
    }

    // returns whether or not we're selecting a unit or units
    public bool HasUnitsSelected ()
    {
        return selectedUnits.Count > 0 ? true : false;
    }

    // returns the selected units
    public Unit[] GetSelectedUnits ()
    {
        return selectedUnits.ToArray();
    }
}