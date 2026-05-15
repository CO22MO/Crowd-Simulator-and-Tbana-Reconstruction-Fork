using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSec : MonoBehaviour
{
    private enum TrainState { WaitingAtStart, MovingToPlatform, WaitingAtPlatform, MovingToEnd }
    private TrainState currentState;

    private float timer = 0f; 
    private float startWaitTime; 
    private float platformWaitTime = 10f; 

    private Vector3 startPos;
    private float stopX = 65f; 
    private float endX = -140f; 

    private float currentSpeed = 0f;
    private float maxSpeed = 10f;
    private float acceleration = 3f;

    public AudioSource[] audioSource;   // Array of AudioSourcees sincce eeach train has multiple sounces which sound comes from
    public AudioClip[] trainClips;      // An array of soundclips, trainClip[0] is arrival sound, trainClip[1] is safety announcement and trainClip[2] is departure

    private bool timerGuard = false;

    // Set train to starting position
    void Start()
    {
        startPos = new Vector3(245f, transform.position.y, transform.position.z);
        transform.position = startPos;
        ResetToStart();
    }

    // Updates train position and state
    void Update()
    {
        switch (currentState)
        {
            // Train is waiting out a timer at starting position
            case TrainState.WaitingAtStart:
                timer += Time.deltaTime;
                // Train starts moving towards the metro, arrival sound is played
                if (timer >= startWaitTime)
                {
                    timer = 0f;
                    changeVolume(0.5f);
                    PlayTrainSound(trainClips[0]);
                    currentState = TrainState.MovingToPlatform;
                }
                break;

            case TrainState.MovingToPlatform:
                float distToCenter = transform.position.x - stopX;
                
                float targetSpeed = (distToCenter < 15f) ? 0f : maxSpeed;
                
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

                if (currentSpeed < 0.05f && distToCenter < 1.0f) 
                {
                    currentSpeed = 0f;
                    timer = 0f;
                    currentState = TrainState.WaitingAtPlatform;
                }
                break;

            // Train is waiting out timer at the platform
            case TrainState.WaitingAtPlatform:
                timer += Time.deltaTime;
                // Wait 2 seconds then play the safety announcement
                if (timer >= 2 && timerGuard == false)
                {
                    changeVolume(1f);
                    PlayTrainSound(trainClips[1]);
                    timerGuard = true;
                }
                // Play train sound for train departing after timer is done
                if (timer >= platformWaitTime)
                {
                    timer = 0f;
                    changeVolume(0.5f);
                    PlayTrainSound(trainClips[2]);
                    timerGuard = false;
                    currentState = TrainState.MovingToEnd;
                }
                break;

            case TrainState.MovingToEnd:
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

                if (transform.position.x <= endX)
                {
                    StopTrainSound();
                    ResetToStart();
                }
                break;
        }
    }

    // Move train back to original position
    void ResetToStart()
    {
        transform.position = startPos;
        currentSpeed = 0f;
        timer = 0f;
        startWaitTime = Random.Range(5f, 20f);
        currentState = TrainState.WaitingAtStart;
    }

    // Play an AudioClip on all audioSources
    void PlayTrainSound(AudioClip clip)
    {
        foreach(AudioSource source in audioSource) 
        {
            source.PlayOneShot(clip);
        }
    }

    // Kills the sound from the train
    void StopTrainSound()
    {
        foreach(AudioSource source in audioSource) 
        {
            source.Stop();
        }
    }

    // Updates the volume of all audioSources
    void changeVolume(float vol)
    {
        foreach(AudioSource source in audioSource) 
        {
            source.volume = vol;
        }
    }
}