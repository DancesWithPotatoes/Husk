//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        02/04/17
// Date last edited:    22/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
// A script used to control the main camera that is used to view the gameplay scene.
public class MainCameraScript : MonoBehaviour
{
    // The property used to get if the camera is currently shaking.
    public bool IsShaking
    {
        get { return (shakeTimer > 0.0f); }
    }

    // Causes the camera to shake.
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    // Stops the camera from shaking if it is currently doing so.
    public void StopShaking()
    {
        // Ensures that the camera is currently shaking.
        if (IsShaking == false)
            throw new System.InvalidOperationException("The camera is not currently shaking.");

        // Resets the shake timer which will end the shaking loop within ShakeCoroutine().
        shakeTimer = 0.0f;
    }


    // The amount of time in seconds until the camera stops shaking.
    private float shakeTimer;

    // Called when the script is initialised.
    private void Start()
    {
        shakeTimer = 0.0f;
    }

    // A coroutine which causes the camera to shake for the specified duration.
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        // Ensures that the camera isn't already shaking and that the specified parameters are valid.
        if (IsShaking)
            throw new System.InvalidOperationException("The camera is already shaking.");
        else if (duration <= 0.0f)
            throw new System.ArgumentException("The duration of the camera-shaking action must be greater than zero.");
        else if (magnitude <= 0.0f)
            throw new System.ArgumentException("The magnitude of the camera-shaking action must be greater than zero.");

        // The initial position of the camera before it starts shaking.
        Vector3 originalPosition = this.transform.position;
        shakeTimer = duration;

        // A loop which causes the camera to continually shake until the timer has run out.
        while (shakeTimer > 0.0f)
        {
            // If the camera is currently in it's original position, moves it in a random direction the distance specified by the magnitude parameter.
            if (this.transform.position == originalPosition)
            {
                // The random direction in which the camera will move during this loop.
                Vector2 shakeDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                shakeDirection.Normalize();

                this.transform.Translate(shakeDirection * magnitude);
            }
            // Else if the camera is not in it's original position, returns it to this position
            else
                this.transform.position = originalPosition;

            shakeTimer -= Time.deltaTime;
            yield return null;
        }

        // Returns the camera to it's original position once the loop is complete and resets the timer.
        this.transform.position = originalPosition;
        shakeTimer = 0.0f;
    }
}