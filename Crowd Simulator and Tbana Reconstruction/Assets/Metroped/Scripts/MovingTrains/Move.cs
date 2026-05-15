using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private enum TrainState { WaitingAtStart, MovingToPlatform, WaitingAtPlatform, MovingToEnd }
    private TrainState currentState;

    private float timer = 0f; 
    private float startWaitTime; 
    private float platformWaitTime = 10f; 
    
    private Vector3 startPos;
    private float stopX = 65f;   
    private float endX = 270f;   
    
    private float currentSpeed = 0f;
    private float maxSpeed = 10f;
    private float acceleration = 3f;

    public AudioSource[] audioSource;   // Array of AudioSourcees sincce eeach train has multiple sounces which sound comes from
    public AudioClip[] trainClips;      // An array of soundclips, trainClip[0] is arrival sound, trainClip[1] is safety announcement and trainClip[2] is departure

    private bool timerGuard = false;

    // Set train to starting position
    void Start()
    {
        startPos = new Vector3(-115f, transform.position.y, transform.position.z);
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
                float distToCenter = stopX - transform.position.x;
                
                // Bromsa in n�r vi �r n�ra (15 enheter kvar)
                float targetSpeed = (distToCenter < 15f) ? 0f : maxSpeed;
                
                // Mjuk f�r�ndring av farten
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

                // NY LOGIK: Om farten �r n�stan noll, stanna precis d�r du �r
                if (currentSpeed < 0.05f && distToCenter < 1.0f) 
                {
                    currentSpeed = 0f; // Nolla farten helt
                    timer = 0f;
                    currentState = TrainState.WaitingAtPlatform;
                    // Vi tvingar INTE positionen h�r, t�get stannar "d�r det landar"
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
                transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

                if (transform.position.x >= endX)
                {
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
        startWaitTime = Random.Range(5f, 20f); // Ny slumpm�ssig tid
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