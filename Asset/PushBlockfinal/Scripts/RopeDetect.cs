using UnityEngine;

public class RopeDetect : MonoBehaviour {

    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    //public PushAgentBasic agent;  //

    void OnCollisionEnter(Collision col)
    {

        Debug.Log(col.transform.gameObject.name);
        // Touched goal

    }
}
