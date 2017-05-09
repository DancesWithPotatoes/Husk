//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        02/04/17
// Date last edited:    09/05/17
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
        // shakeCoroutine will have a value if ShakeCoroutine() is running, else it will always be equal to null.
        get { return (shakeCoroutine != null); }
    }

    // Causes the camera to shake.
    public void Shake(float duration, float magnitude)
    {
        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    // Stops the camera from shaking if it is currently doing so.
    public void StopShaking()
    {
        // Ensures that the camera is currently shaking and thus that ShakeCoroutine() is currently running.
        if (!IsShaking)
            throw new System.InvalidOperationException("The camera isn't currently shaking.");

        StopCoroutine(shakeCoroutine);
        shakeCoroutine = null;

        // Resets the camera to it's original position.
        this.transform.position = defaultPosition;
    }
    

    // The coroutine which is currently causing the camera to shake.
    private Coroutine shakeCoroutine;
    // The default world position of the camera when it isn't shaking.
    private Vector3 defaultPosition;

    // Called when the script is initialised.
    private void Start()
    {
        defaultPosition = this.transform.position;
    }


    // A coroutine which causes the camera to shake.
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        // Ensures that the camera isn't already shaking and that the given shake duration and magnitide values are valid.
        if (IsShaking)
            throw new System.InvalidOperationException("The camera is already shaking.");
        else if (duration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The duration of the camera-shaking action must be greater than zero.");
        else if (magnitude <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The magnitude of the camera-shaking action must be greater than zero.");

        // A timer used to store the elapsed time in seconds for which the camera has been shaking during the execution of this coroutine.
        float shakeTimer = 0.0f;
        // A loop which causes the camera to continually shake until the timer has run out.
        while (shakeTimer < duration)
        {
            // If the camera is currently in it's original position, moves it in a random direction the distance specified by the magnitude parameter.
            if (this.transform.position == defaultPosition)
            {
                // The random direction in which the camera will move during this loop.
                Vector2 shakeDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                shakeDirection.Normalize();

                this.transform.Translate(shakeDirection * magnitude);
            }
            // Else if the camera is not in it's original position, returns it to that position
            else
                this.transform.position = defaultPosition;

            shakeTimer += Time.deltaTime;
            yield return null;
        }
        
        // Calls the StopShaking() method so that shakeCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless this coroutine is running.
        StopShaking();
    }
}