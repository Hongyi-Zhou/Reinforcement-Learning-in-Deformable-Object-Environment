﻿//Detect when the orange block has touched the goal.
//Detect when the orange block has touched an obstacle.
//Put this script onto the orange block. There's nothing you need to set in the editor.
//Make sure the goal is tagged with "goal" in the editor.

using UnityEngine;

public class Octo_detect: MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    public PushAgentBasic agent;  //

    void OnCollisionEnter(Collision col)
    {

        //Debug.Log(col.transform.gameObject.name);
        // Touched goal.
        if (col.gameObject.CompareTag("goal"))
        {
            
            agent.ScoredAOctopus();
        }

    }
}