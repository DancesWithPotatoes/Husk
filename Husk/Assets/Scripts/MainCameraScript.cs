//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        02/04/17
// Date last edited:    02/04/17
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
        get { return isShaking; }
    }

    // Causes the camera to shake.
    public IEnumerator Shake(float duration, float magnitude)
    {
        if (!isShaking && duration > 0.0f && magnitude > 0.0f)
        {           
            // The original position of the camera.
            Vector3 originalPosition = this.transform.position;
            // The timer used to store how long the coroutine has been running.
            float timer = 0.0f;

            while (timer < duration)
            {
                if (this.transform.position == originalPosition)
                {
                    // The random direction which the camera will move during this loop of the shaking.
                    Vector2 shakeDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                    shakeDirection.Normalize();

                    this.transform.Translate(shakeDirection * magnitude);
                }
                else
                {
                    this.transform.position = originalPosition;
                }

                yield return null;

                timer += Time.deltaTime;
            }

            this.transform.position = originalPosition;
        }
        else
            yield return null;   
    }

    // If the camera is currently shaking.
    private bool isShaking;

    // Called when the script is loaded.
    private void Awake()
    {

    }

    // Called when the script is initialised.
    private void Start()
    {
        isShaking = false;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {

    }
}