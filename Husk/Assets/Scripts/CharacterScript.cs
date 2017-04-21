//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        16/04/17
// Date last edited:    21/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// An abstract script which when derived will be used to handle the actions of a character within the world which can attack, move, and be damaged by other character-derived objects.
public abstract class CharacterScript : MonoBehaviour
{
    // The prefab used which allows the character to spawn an attack object and damage enemy characters.
    public Transform AttackPrefab;
    // The maximum movement speed of the character.
    public float MoveSpeed;

    // The property used to get the heading of the character.
    public Vector2 Heading
    {
        get { return heading; }
    }

    // The property used to get whether the character currently has a child attack object spawned.
    public bool IsAttacking
    {
        get { return (this.transform.GetComponentInChildren<AttackScript>() != null); }
    }

    // The property used to get whether the character is currently flashing a color.
    public bool IsFlashing
    {
        get { return isFlashing; }
    }

    // Damages the character.
    public void Damage()
    {
        if (IsFlashing)
            StopFlashing();

        StartCoroutine(FlashColor(Color.red, 0.2f));
    }

    // Freezes the character in place for the specified duration.
    public void Freeze(float duration)
    {
        if (duration > 0.0f && !isFrozen)
            StartCoroutine(FreezeCoroutine(duration));
    }

    // Changes the local rotation of the specified child object of the character so that it faces in the same direction as the character's heading.
    public void RotateChildObjectToFaceCharacterHeading(Transform childObject)
    {
        // If the specified object is a child of the character, rotates it.
        if (childObject.IsChildOf(this.transform))
        {
            // The angle in degrees to rotate the child object in local space so that it is facing in the same direction as the character heading vector.
            float attackAngle = Mathf.Atan2(-heading.x, heading.y) * Mathf.Rad2Deg;

            childObject.localRotation = Quaternion.AngleAxis(attackAngle, Vector3.forward);
        }
        // Else, throws an exception.
        else
            throw new System.Exception("The object being rotated to face the heading of the character must be a child of the character.");
    }


    // A normalised vector representing the direction in which the character is facing.
    protected Vector2 heading;
    // Whether or not the character is currently frozen in place.
    protected bool isFrozen;

    // A coroutine which causes the character to temporarily flash the specified color.
    protected IEnumerator FlashColor(Color flashColor, float duration)
    {
        if (!isFlashing)
        {
            if (duration > 0.0f)
            {
                isFlashing = true;

                // Gets the sprite renderer of the child object which displays the character sprite.
                SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                // The original color tint of the sprite renderer.
                Color originalColor = spriteRenderer.color;

                spriteRenderer.color = flashColor;

                // The amount of time in seconds for which the character has been flashing.
                float flashTimer = 0.0f;
                // Pauses the coroutine until the specified duration has passed.
                while (flashTimer < duration)
                {
                    // If the 'flashing' flag is still true, remains the flashing color.
                    if (IsFlashing)
                    {
                        flashTimer += Time.deltaTime;
                        yield return null;
                    }
                    // Else if the 'flashing' flag has been set to false during the execution of the coroutine via the StopFlashing() method, cuts it short to stop the character from flashing.
                    else
                        break;
                }
                
                spriteRenderer.color = originalColor;

                isFlashing = false;
            }
            else
                yield return null;
        }
        else
            throw new System.Exception("Character " + this.name + " is already flashing a color.");
    }
    // A coroutine which causes the character to gradually transition to and then flash the specified color.
    protected IEnumerator FlashColor(Color flashColor, float transitionDuration, float flashDuration)
    {
        if (!isFlashing)
        {
            if (flashDuration > 0.0f)
            {
                isFlashing = true;

                // Gets the sprite renderer of the child object which displays the character sprite.
                SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                // The original color tint of the sprite renderer.
                Color originalColor = spriteRenderer.color;

                if (transitionDuration > 0.0f)
                {
                    // The timer used to record how many seconds the sprite tint has been transitioning towards the final flash color.
                    float transitionTimer = 0.0f;
                    // A loop which lerps the sprite color from the original to the flash color throughout the specified duration.
                    while (transitionTimer < transitionDuration)
                    {
                        // If the 'flashing' flag is still true, continues lerping towards the flashing color.
                        if (IsFlashing)
                        {
                            spriteRenderer.color = Color.Lerp(originalColor, flashColor, transitionTimer / transitionDuration);

                            transitionTimer += Time.deltaTime;

                            yield return null;

                        }
                        // Else if the 'flashing' flag has been set to false during the execution of the coroutine via the StopFlashing() method, cuts it short to stop the character from flashing.
                        else
                            break;
                    }
                }

                spriteRenderer.color = flashColor;

                // The amount of time in seconds for which the character has been flashing.
                float flashTimer = 0.0f;
                // Pauses the coroutine until the specified duration has passed.
                while (flashTimer < flashDuration)
                {
                    // If the 'flashing' flag is still true, remains the flashing color.
                    if (isFlashing)
                    {
                        flashTimer += Time.deltaTime;
                        yield return null;
                    }
                    // Else if the 'flashing' flag has been set to false during the execution of the coroutine, cuts it short to stop the character from flashing.
                    else
                        break;
                }

                spriteRenderer.color = originalColor;

                isFlashing = false;
            }
            else
                yield return null;
        }
        else
            throw new System.Exception("Character " + this.name + " is already flashing a color.");
    }

    // Stops the character from continuing to flash if it is currently doing so.
    protected void StopFlashing()
    {
        // Changes the 'flashing' flag, which in turn will end any FlashColor coroutine which is currently running.
        isFlashing = false;
    }

    // An abstract method which will be used to implement the movement of the character in derived scripts.
    protected abstract void UpdateMovement();

    // An abstract method which will be used to implement the attacking of the character in derived scripts.
    protected abstract void UpdateAttacking();


    // Whether the character is currently in the process of flashing a color.
    private bool isFlashing;

    // A coroutine which freezes the character in place for the specified duration, then unfreezes it.
    private IEnumerator FreezeCoroutine(float duration)
    {
        if (duration > 0.0f && !isFrozen)
        {
            isFrozen = true;

            // Pauses the coroutine until the specified duration has passed.
            yield return new WaitForSeconds(duration);

            isFrozen = false;
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
        isFrozen = false;
        heading = Vector2.down;
        isFlashing = false;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        UpdateMovement();
        UpdateAttacking();
    }
}