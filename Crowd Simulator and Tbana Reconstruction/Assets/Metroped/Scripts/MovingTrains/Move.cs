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

    public AudioSource[] audioSource;
    public AudioClip[] trainClips;

    private bool timerGuard = false;
    private int alternatingGuard = 0;

    void Start()
    {
        startPos = new Vector3(-115f, transform.position.y, transform.position.z);
        transform.position = startPos;
        ResetToStart();
    }

    void Update()
    {
        switch (currentState)
        {
            case TrainState.WaitingAtStart:
                timer += Time.deltaTime;
                if (timer >= startWaitTime)
                {
                    timer = 0f;
                    PlayTrainSound(trainClips[0]);
                    currentState = TrainState.MovingToPlatform;
                }
                break;

            case TrainState.MovingToPlatform:
                float distToCenter = stopX - transform.position.x;
                
                // Bromsa in när vi är nära (15 enheter kvar)
                float targetSpeed = (distToCenter < 15f) ? 0f : maxSpeed;
                
                // Mjuk förändring av farten
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

                // NY LOGIK: Om farten är nästan noll, stanna precis där du är
                if (currentSpeed < 0.05f && distToCenter < 1.0f) 
                {
                    currentSpeed = 0f; // Nolla farten helt
                    timer = 0f;
                    currentState = TrainState.WaitingAtPlatform;
                    // Vi tvingar INTE positionen här, tåget stannar "där det landar"
                }
                break;

            case TrainState.WaitingAtPlatform:
                timer += Time.deltaTime;
                if (timer >= 3 && timerGuard == false && alternatingGuard % 2 == 1)
                {
                    PlayTrainSound(trainClips[1]);
                    timerGuard = true;
                }
                if (timer >= platformWaitTime)
                {
                    timer = 0f;
                    alternatingGuard++;
                    // Play train sound for train departing
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

    void ResetToStart()
    {
        transform.position = startPos;
        currentSpeed = 0f;
        timer = 0f;
        startWaitTime = Random.Range(5f, 20f); // Ny slumpmässig tid
        currentState = TrainState.WaitingAtStart;
    }

    void PlayTrainSound(AudioClip clip)
    {
        foreach(AudioSource source in audioSource) 
        {
            source.PlayOneShot(clip);
        }
    }

    void StopTrainSound()
    {
        foreach(AudioSource source in audioSource) 
        {
            source.Stop();
        }
    }
}