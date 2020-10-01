using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player[] players;

    public static GameManager instance;

    void Awake ()
    {
        instance = this;
    }

    // returns a random enemy player
    public Player GetRandomEnemyPlayer (Player me)
    {
        Player ranPlayer = players[Random.Range(0, players.Length)];

        while(ranPlayer == me)
        {
            ranPlayer = players[Random.Range(0, players.Length)];
        }

        return ranPlayer;
    }

    // called when a unit dies, check to see if there's one remaining player
    public void UnitDeathCheck ()
    {
        int remainingPlayers = 0;
        Player winner = null;

        for(int x = 0; x < players.Length; x++)
        {
            if(players[x].units.Count > 0)
            {
                remainingPlayers++;
                winner = players[x];
            }
        }

        // if there is more than 1 remaining player, return
        if(remainingPlayers != 1)
            return;

        EndScreenUI.instance.SetEndScreen(winner.isMe);
    }
}