using UnityEngine;

public class GhostTrap : MonoBehaviour
{
    internal enum E_TrapState
    {
        None,
        Thrown,
        Activated,
        Failed,
        Success
    }
   
    [Header("Trapping Settings")]
    [SerializeField] private float trapRadius = 5f; // Radius used to see if any ghosts are within the traps area
    [SerializeField] private float trapDuration = 5f; // Duration of how long the trapping will take
    private E_TrapState trapState = E_TrapState.None; 

    private void Start()
    {

    }

    private void Update()
    {
        switch (trapState)
        {
            case E_TrapState.None:
                // Do nothing and or initialize anything needed
                break;
            case E_TrapState.Thrown:
                // Check for when the trap hits the ground 
                // When the ground is hit, change the state to Activated
                break;
            case E_TrapState.Activated:
                // Check for the ghost within the trap radius
                // If a ghost is within the trap radius, attempt to trap
                // Move to failed if no ghost is found within the trap radius
                // Move to failed if the trapping fails
                // Move to success if the trapping is successful
                break;
            case E_TrapState.Failed:
                // Allow player to throw another trap
                // Find a way to hold a max number of retries
                // Delete the trap after a certain time
                break;
            case E_TrapState.Success:
                // Handle the success of trapping a ghost
                // Figure out how, where and when to add the ghost to the players's inventory
                // Possibly play a success animation or sound (try to suck the ghost into the trap, shake trap, then remove trap from the scene after handling players inventory)
                break;
            default:
                break;
        }
    }

    public void ThrowTrap(Vector3 position, GameObject target, Camera arCamera)
    {

    }
}
