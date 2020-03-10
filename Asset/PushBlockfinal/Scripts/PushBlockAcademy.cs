//Every scene needs an academy script.
//Create an empty gameObject and attach this script.
//The brain needs to be a child of the Academy gameObject.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using NVIDIA.Flex;

public class PushBlockAcademy : Academy
{
    /// <summary>
    /// The "walking speed" of the agents in the scene.
    /// </summary>
    public float agentRunSpeed;

    /// <summary>
    /// The agent rotation speed.
    /// Every agent will use this setting.
    /// </summary>
    public float agentRotationSpeed;

    /// <summary>
    /// The spawn area margin multiplier.
    /// ex: .9 means 90% of spawn area will be used.
    /// .1 margin will be left (so players don't spawn off of the edge).
    /// The higher this value, the longer training time required.
    /// </summary>
    public float spawnAreaMarginMultiplier;

    /// <summary>
    /// When a goal is scored the ground will switch to this
    /// material for a few seconds.
    /// </summary>
    public Material goalScoredMaterial;

    /// <summary>
    /// When an agent fails, the ground will turn this material for a few seconds.
    /// </summary>
    public Material failMaterial;

    /// <summary>
    /// The gravity multiplier.
    /// Use ~3 to make things less floaty
    /// </summary>
    public float gravityMultiplier;

////// Felx objects modified below
    
    /// <summary>
    /// Maximum number of particles in scene.
    /// </summary>
    public int maxParticles = 4096;

    /// <summary>
    /// Radius of flex particles.
    /// </summary>
    public float particleRadius = 0.5f;

    /// <summary>
    /// Dimension of particle state (pos_x, pos_y, pos_z, mass, vel_x, vel_y, vel_z, id).
    /// </summary>
   ///??? const int particleDimension = 8;

    /// <summary>
    /// The flex container or solver.
    /// </summary>
    public FlexContainer flexContainer;

    /// <summary>
    /// The flex agent.
    /// </summary>
    public GameObject Octopus1;

    /// <summary>
    /// Initantiates and initializes a new flex container.
    /// </summary>
    /// <param name="maxParticles">Maximum number of particles of flex container.</param>
    /// <param name="particleRadius">Radius of flex particles.</param>
    void InitializeFlexContainer(int maxParticles, float particleRadius)
    {
        flexContainer = ScriptableObject.CreateInstance<FlexContainer>();
        // number of particles of all FleX objects, in this case 8 agent and 8 target particles
        flexContainer.maxParticles = maxParticles;
        flexContainer.radius = particleRadius;
        flexContainer.solidRest = particleRadius;
        flexContainer.collisionDistance = particleRadius / 2.0f;
    }

    ///isHeadlessMode

    ///AddFlexSolidActor

    ///InitializeTarget

    ///InitializeAgent

    ///InitializeAcademy



/////////

    void State()
    {
        Physics.gravity *= gravityMultiplier;
    }
}
