//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        25/04/17
// Date last edited:    25/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle global actions that effect every gameplay object in the scene.
public class SceneControllerScript : MonoBehaviour
{
    // The property used to get if the scene is currently paused.
    public bool IsPaused
    {
        get { return (pauseTimer != 0.0f); }
    }

    // Pauses the scene until unpaused.
    public void PauseScene()
    {
        // Ensures that the scene isn't already paused.
        if (IsPaused)
            throw new System.InvalidOperationException("The scene is already paused.");

        ChangeAllGameObjectsPausedState(true);

        // Sets the pause timer to -1.0f so that the scene will remain paused indefinitely until unpaused.
        pauseTimer = -1.0f;
    }
    // Pauses the scene for the specified duration.
    public void PauseScene(float duration)
    {
        // Ensures that the scene isn't already paused and that the specified duration is valid.
        if (IsPaused)
            throw new System.InvalidOperationException("The scene is already paused.");
        if (duration <= 0.0f)
            throw new System.ArgumentException("The pause duration must be greater than zero.");

        ChangeAllGameObjectsPausedState(true);

        pauseTimer = duration;
    }

    // Unpauses the scene.
    public void UnpauseScene()
    {
        // Ensures that the scene is paused.
        if (!IsPaused)
            throw new System.InvalidOperationException("The scene must be paused in order to be unpaused.");

        ChangeAllGameObjectsPausedState(false);

        pauseTimer = 0.0f;
    }


    // A timer used to store the duration in seconds for which the game objects in the scene will continue to be paused - if the value is -1.0f, the scene will remain paused indefinitely.
    private float pauseTimer;

    // Called when the script is initialised.
    private void Start()
    {
        pauseTimer = 0.0f;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        // If the pause timer is greater than zero, decrements it towards the point where it reaches zero and unpauses the scene.
        if (pauseTimer > 0.0f)
        {
            pauseTimer -= Time.deltaTime;

            if (pauseTimer <= 0.0f)
            {                
                UnpauseScene();
                pauseTimer = 0.0f;
            }
        }
    }

    // Alters the 'paused' status of all gameplay objects within the scene.
    private void ChangeAllGameObjectsPausedState(bool pausedState)
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerCharacterScript>().IsPaused = pausedState;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            enemy.GetComponent<EnemyCharacterScript>().IsPaused = pausedState;

        foreach (GameObject attack in GameObject.FindGameObjectsWithTag("Attack"))
            attack.GetComponent<AttackScript>().IsPaused = pausedState;
    }
}