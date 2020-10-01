using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStateBubble : MonoBehaviour
{
    public Image stateBubble;

    public Sprite idleState;
    public Sprite gatherState;
    public Sprite attackState;

    public void OnStateChange (UnitState state)
    {
        stateBubble.enabled = true;

        switch(state)
        {
            case UnitState.Idle:
            {
                stateBubble.sprite = idleState;
                break;
            }
            case UnitState.Gather:
            {
                stateBubble.sprite = gatherState;
                break;
            }
            case UnitState.Attack:
            {
                stateBubble.sprite = attackState;
                break;
            }
            default:
            {
                stateBubble.enabled = false;
                break;
            }
        }
    }
}