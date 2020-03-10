//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using NVIDIA.Flex;

public class PushAgentBasic : Agent
{
    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    public GameObject area;

    /// <summary>
    /// The area bounds.
    /// </summary>
    [HideInInspector]
    public Bounds areaBounds;

    PushBlockAcademy m_Academy;

    /// <summary>
    /// The goal to push the block to.
    /// </summary>
    public GameObject goal;

    ///////////
    public GameObject octopus1;
    public Octo_detect octo_detect;

    public bool useVectorObs;

    Rigidbody m_AgentRb;  //cached on initialization
    Material m_GroundMaterial; //cached on Awake()
    RayPerception m_RayPer;

    float[] m_RayAngles = { 0f, 15f, 30f, 45f, 60f, 75f, 90f, 105f, 120f, 135f, 150f, 165f, 180f};
    string[] m_DetectableObjects = { "goal", "wall", "octopus1" };

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>
    Renderer m_GroundRenderer;

    //bool goalOctopus = false;


    void Awake()
    {
        m_Academy = FindObjectOfType<PushBlockAcademy>(); //cache the academy
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();

        octo_detect = m_Academy.Octopus1.GetComponent<Octo_detect>();
        octo_detect.agent = this;

        m_RayPer = GetComponent<RayPerception>();

        // Cache the agent rigidbody
        m_AgentRb = GetComponent<Rigidbody>();
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when a goal is scored
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;

        SetResetParameters();
    }

    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            var rayDistance = 12f;
            //Debug.Log(m_RayPer.Perceive(rayDistance, m_RayAngles, m_DetectableObjects, 0f, 0f));
            AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, m_DetectableObjects, 0f, 0f));
            //AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, m_DetectableObjects, 0.6f, 0f));
        }
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos(float margin)
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * margin,
                areaBounds.extents.x * margin);

            var randomPosZ = Random.Range(-areaBounds.extents.z * margin,
                areaBounds.extents.z * margin);

            if (margin>= 0.6f){
                randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 3f, randomPosZ);
            }else{
                randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            }
            
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }


    /// <summary>
    /// Called when the agent moves the Octopus into the goal.
    /// </summary>
    public void ScoredAOctopus()
    {
        // We use a reward of 5.
        AddReward(5f);

        // By marking an agent as done AgentReset() will be called automatically.
        Done();

        // Swap ground material for a bit to indicate we scored.
        StartCoroutine(GoalScoredSwapGroundMaterial(m_Academy.goalScoredMaterial, 0.2f));
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    { 
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 1 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }

    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>
    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);

        // Goalies and Strikers have slightly different action spaces.
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * m_Academy.agentRunSpeed,
            ForceMode.VelocityChange);
    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Move the agent using the action.
        MoveAgent(vectorAction);

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / agentParameters.maxStep);
    }

    public override float[] Heuristic()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return new float[] { 3 };
        }
        if (Input.GetKey(KeyCode.W))
        {
            return new float[] { 1 };
        }
        if (Input.GetKey(KeyCode.A))
        {
            return new float[] { 4 };
        }
        if (Input.GetKey(KeyCode.S))
        {
            return new float[] { 2 };
        }
        return new float[] { 0 };
    }

//////////////////////////////////////////////////////
    /// <summary>
    /// Returns a new random float between -4 and 4.
    /// </summary>
    float NewRandomPosition()
    {
        float x = Random.value * 15.0f - 7.5f;
        return x;
    }

    /// <summary>
    /// Resets the academy by teleporting the target to a new random location and resetting the number of steps.
    /// </summary>
    public void OctopusReset()
    {
        m_Academy.Octopus1.GetComponent<FlexActor>().Teleport
        (GetRandomSpawnPos(0.7f), Quaternion.Euler(Vector3.zero));
    }
///////////////////////////////////////////////////////////////
    /// <summary>
    /// In the editor, if "Reset On Done" is checked then AgentReset() will be
    /// called automatically anytime we mark done = true in an agent script.
    /// </summary>
    public override void AgentReset()
    {
        var rotation = Random.Range(0, 4);
        var rotationAngle = rotation * 90f;
        area.transform.Rotate(new Vector3(0f, rotationAngle, 0f));

        //reset
        OctopusReset();

        transform.position = GetRandomSpawnPos(0.5f);
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;

        SetResetParameters();
    }

    public void SetGroundMaterialFriction()
    {
        var resetParams = m_Academy.resetParameters;

        var groundCollider = ground.GetComponent<Collider>();

        groundCollider.material.dynamicFriction = resetParams["dynamic_friction"];
        groundCollider.material.staticFriction = resetParams["static_friction"];
    }


    public void SetResetParameters()
    {
        SetGroundMaterialFriction();
        //SetBlockProperties();
    }
}
