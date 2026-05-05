using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;          // 3D AudioSource
    public AudioClip[] footstepClips;        // Footstep sounds
    public float stepDistance = 2f;          // Distance traveled per step

    private Vector3 lastPosition;
    private float distanceTraveled = 0f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0; // ignore vertical movement
        distanceTraveled += delta.magnitude;

        if (distanceTraveled >= stepDistance)
        {
            PlayFootstep();
            distanceTraveled = 0f;
        }

        lastPosition = transform.position;
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f); // slight pitch variation
        audioSource.PlayOneShot(clip);
    }
}