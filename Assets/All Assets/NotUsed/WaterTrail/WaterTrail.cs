using UnityEngine;

public class WaterTrail : MonoBehaviour
{
    public ParticleSystem waterTrail; // Reference to the Particle System
    private bool isInWater = false;
    private float exitDelay = 0.2f; // Delay time before considering exit
    private float exitTimer = 0f;
    private Transform player; // Reference to the player's transform

    private void Start()
    {
        // Assuming the player object is assigned manually in the Inspector
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the water's collider is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the water.");
            PlayWaterTrail(); // Start the water trail when the player enters the water
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that exited the water's collider is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the water.");
            exitTimer = Time.time + exitDelay; // Start the exit delay timer
        }
    }

    private void Update()
    {
        // Make the particle system follow the player while in water
        if (waterTrail.isPlaying && player != null)
        {
            waterTrail.transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
        }

        // Check if the player has exited the water after the delay
        if (isInWater && Time.time >= exitTimer)
        {
            StopWaterTrail(); // Stop the water trail after the delay
            isInWater = false; // Reset the flag
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuously check if the player is still inside the water and trigger the effect
        if (other.CompareTag("Player"))
        {
            if (!waterTrail.isPlaying)  // Check if the particle system isn't already playing
            {
                Debug.Log("Player is in the water, playing water trail.");
                PlayWaterTrail(); // Play the water trail if it's not playing
            }
            isInWater = true;
        }
    }

    private void PlayWaterTrail()
    {
        if (!waterTrail.isPlaying)  // Check if the particle system isn't already playing
        {
            waterTrail.Play(); // Start the particle system if it's not playing
            Debug.Log("Water trail started.");
        }
    }

    private void StopWaterTrail()
    {
        if (waterTrail.isPlaying)  // Check if the particle system is playing
        {
            waterTrail.Stop(); // Stop the particle system if it's playing
            Debug.Log("Water trail stopped.");
        }
    }
}
