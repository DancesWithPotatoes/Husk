//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        02/04/17
// Date last edited:    16/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
// A script used to control the main camera used to view the gameplay scene.
public class MainCameraScript : MonoBehaviour
{
    // The property used to get if the camera is currently shaking.
    public bool IsShaking
    {
        get { return (shakeTimer > 0.0f); }
    }

    // Causes the camera to shake if it is not already doing so.
    public void Shake(float duration, float magnitude)
    {
        // Starts the coroutine which causes the camera to continually shake for the specified duration.
        if (!IsShaking && duration > 0.0f && magnitude > 0.0f)
            StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    // The amount of time in seconds until the camera stops shaking.
    private float shakeTimer = 0.0f;
    
    // Called when the script is initialised.
    private void Start()
    {
        shakeTimer = 0.0f;
    }
    
    // A coroutine which causes the camera to shake for the specified duration.
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        if (!IsShaking && duration > 0.0f && magnitude > 0.0f)
        {
            // The original position of the camera.
            Vector3 originalPosition = this.transform.position;
            shakeTimer = duration;

            // The loop which causes the camera to continually shake until the shake timer has run out.
            while (shakeTimer > 0.0f)
            {
                // If the camera is currently in it's original position, moves it in a random direction by specified magnitude.
                if (this.transform.position == originalPosition)
                {
                    // The random direction which the camera will move during this loop of the shaking.
                    Vector2 shakeDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                    shakeDirection.Normalize();

                    this.transform.Translate(shakeDirection * magnitude);
                }
                // Else if the camera is not in it's original position, returns it.
                else
                    this.transform.position = originalPosition;

                shakeTimer -= Time.deltaTime;

                yield return null;
            }

            // Returns the camera to it's original position once the loop is complete.
            this.transform.position = originalPosition;
            shakeTimer = 0.0f;
        }
        else
            yield return null;
    }
}