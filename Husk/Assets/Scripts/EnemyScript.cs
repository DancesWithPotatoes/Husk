//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    29/03/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
// A script used to handle the actions of an enemy character.
public class EnemyScript : MonoBehaviour
{
    // Damages the enemy character.
    public void Damage()
    {
        StartCoroutine(FlashColor(Color.red, 0.2f));
    }

    // A coroutine which causes the enemy character to flash the specified color before returning to it's original color.
    private IEnumerator FlashColor(Color flashColor, float duration)
    {
        if (duration > 0.0f)
        {
            Color originalColor = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = flashColor;

            // Pauses the coroutine until the specified duration has passed.
            yield return new WaitForSeconds(duration);

            GetComponent<SpriteRenderer>().color = originalColor;
        }
        else        
            yield return null;        
    }

    // Called when the script is loaded.
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        // Sets the enemy rigidbody to be kinematic so that it can be moved programmatically whilst still colliding with the physics objects in the scene.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Called when the script is initialised.
    private void Start()
    {

    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {

    }
}