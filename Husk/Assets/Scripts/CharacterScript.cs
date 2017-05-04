//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        16/04/17
// Date last edited:    04/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// An abstract script which when derived will be used to handle the actions of a character within the world that can attack, move, and be damaged by other character-derived objects.
public abstract class CharacterScript : MonoBehaviour
{
    // Whether or not the character is currently locked into an unchanging state.
    [HideInInspector]
    public bool IsPaused;
    // The maximum movement speed of the character.
    public float MoveSpeed;

    // The property used to get the current heading of the character.
    public Vector2 Heading
    {
        get { return heading; }
    }

    // The property used to get whether the character is currently unable to move itself.
    public bool IsFrozen
    {
        // freezeCoroutine will have a value if FreezeCoroutine() is running, else it will always be equal to null - the character will also be considered frozen if under the influence of 'knockback'.
        get { return (freezeCoroutine != null || knockBackForce != Vector2.zero); }
    }

    // The property used to get whether the character is currently flashing a color.
    public bool IsColorFlashing
    {
        // colorFlashCoroutine will have a value if one of the FlashColorCoroutine() overloads are running, else it will always be equal to null.
        get { return (colorFlashCoroutine != null); }
    }

    // Damages the character.
    public void Damage()
    {
        // Causes the character to flash red.
        if (IsColorFlashing)
            StopColorFlashing();
        FlashColor(Color.red, 0.2f);
        
        DamageAddendum();
    }

    // Applies the specified knockback force to the player, causing it to be unable to act until it stops moving.
    public void ApplyKnockbackForce(Vector2 force)
    {
        knockBackForce = force;
    }

    // Stops the character from being able to move itself for the specified duration.
    public void Freeze(float duration)
    {
        freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    // Unfreezes the character if it is currently frozen.
    public void Unfreeze()
    {
        // Ensures that the character is currently frozen and thus that FreezeCoroutine() is currently running.
        if (!IsFrozen)
            throw new System.InvalidOperationException("The character isn't currently frozen.");

        StopCoroutine(freezeCoroutine);
        freezeCoroutine = null;
    }

    // Causes the character to flash the specified color for the set duration.
    public void FlashColor(Color color, float duration)
    {
        colorFlashCoroutine = StartCoroutine(FlashColorCoroutine(color, duration));
    }
    // Causes the character to to gradually change to and then temporarily flash the specified color.
    public void FlashColor(Color color, float colorChangeDuration, float flashDuration)
    {
        colorFlashCoroutine = StartCoroutine(FlashColorCoroutine(color, colorChangeDuration, flashDuration));
    }
    // Causes the character to to gradually change to, flash, and then change back from the specified color.
    public void FlashColor(Color color, float changeToFlashColorDuration, float flashDuration, float changeToOriginalColorDuration)
    {
        colorFlashCoroutine = StartCoroutine(FlashColorCoroutine(color, changeToFlashColorDuration, flashDuration, changeToOriginalColorDuration));
    }

    // Stops the character from flashing a color if it is currently doing so.
    public void StopColorFlashing()
    {
        // Ensures that the character is currently flashing a color and thus that FlashColorCoroutine() is currently running.
        if (!IsColorFlashing)
            throw new System.InvalidOperationException("The character isn't currently flashing a color.");

        StopCoroutine(colorFlashCoroutine);
        colorFlashCoroutine = null;

        // Sets the color of the character sprite to it's default value.
        GetComponentInChildren<SpriteRenderer>().color = defaultSpriteRendererColor;
    }

    // Changes the local rotation of the specified child object of the character so that it faces in the same direction as the character's heading.
    public void RotateChildObjectToFaceCharacterHeading(Transform childObject)
    {
        // Ensures that the specified object is a child of the character.
        if (!childObject.IsChildOf(this.transform))
            throw new System.ArgumentException("The object being rotated to face the heading of the character must be a child of the character.");

        // The angle in degrees to rotate the child object in local space so that it is facing in the same direction as the character heading vector.
        float attackAngle = Mathf.Atan2(-heading.x, heading.y) * Mathf.Rad2Deg;
        // Rotates the child object.
        childObject.localRotation = Quaternion.AngleAxis(attackAngle, Vector3.forward);
    }


    // The default color tint of the character's SpriteRenderer component.
    protected Color defaultSpriteRendererColor;
    // A normalised vector representing the direction in which the character is currently facing.
    protected Vector2 heading;

    // An empty virtual method which can be used by derived classes to add extra functionality to the Damage() method.
    protected virtual void DamageAddendum() { }

    // An empty virtual method which can be used by derived classes to add extra functionality to the Awake() method.
    protected virtual void AwakeAddendum() { }

    // An abstract method which will be used to implement the movement of the character in derived scripts.
    protected abstract void UpdateMovement();

    // An abstract method which will be used to implement the attacking of the character in derived scripts.
    protected abstract void UpdateAttacking();


    // The coroutine which is currently causing the character to be frozen.
    private Coroutine freezeCoroutine;
    // The coroutine which is currently causing the character to color flash.
    private Coroutine colorFlashCoroutine;
    // The force (applied through gameplay code, not Unity's physics engine) being applied to the character under the influence of 'knockback'.
    private Vector2 knockBackForce;

    // A coroutine which freezes the character in place for the specified duration, then unfreezes it.
    private IEnumerator FreezeCoroutine(float duration)
    {
        if (IsPaused)
            throw new System.InvalidOperationException("The character can't be frozen when paused.");
        if (IsFrozen)
            throw new System.InvalidOperationException("The character is already frozen.");
        if (duration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration to freeze the character must be greater than zero.");

        // A timer used to store how long the coroutine has been waiting since the character was initially frozen.
        float waitTimer = 0.0f;
        // A pause-friendly loop which causes the coroutine to wait until the specified duration has passed - throughout this period freezeCoroutine will have a value, which means the IsFrozen property will return true.
        while (waitTimer < duration)
        {
            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                waitTimer += Time.deltaTime;

            yield return null;
        }

        // Calls the Unfreeze() method so that freezeCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless an overload of this coroutine is running.
        Unfreeze();
    }

    // A coroutine which causes the character to temporarily flash the specified color.
    private IEnumerator FlashColorCoroutine(Color flashColor, float duration)
    {
        // Ensures that the character isn't already flashing a color and that the given flash duration is valid.
        if (IsColorFlashing)
            throw new System.InvalidOperationException("The character is already flashing a color.");
        if (duration < 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to flash a color must be greater than or equal to zero.");

        // The sprite renderer of the child object which displays the character sprite.
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = flashColor;

        // A timer used to store how long the coroutine has been waiting since the character changed color.
        float waitTimer = 0.0f;
        // A pause-friendly loop which causes the coroutine to wait until the specified duration has passed - throughout this period flashColorCoroutine will have a value, which means the IsColorFlashing property will return true.
        while (waitTimer < duration)
        {
            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                waitTimer += Time.deltaTime;

            yield return null;
        }

        // Calls the StopColorFlashing() method so that colorFlashCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless an overload of this coroutine is running.
        StopColorFlashing();
    }
    // A coroutine which causes the character to gradually change to and then temporarily flash the specified color.
    private IEnumerator FlashColorCoroutine(Color flashColor, float colorChangeDuration, float flashDuration)
    {
        // Ensures that the character isn't already flashing a color and that the given durations are valid.
        if (IsColorFlashing)
            throw new System.InvalidOperationException("The character is already flashing a color.");
        if (colorChangeDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to gradually change to a color must be greater than or equal to zero.");
        if (flashDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to flash a color must be greater than or equal to zero.");

        // The sprite renderer of the child object which displays the character sprite.
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // A timer used to store the duration in seconds for which the character color has been changing from the original color to the flash color.
        float colorChangeTimer = 0.0f;
        // Loops until the color has been completely changed from the original to the flash color.
        while (colorChangeTimer < colorChangeDuration)
        {
            spriteRenderer.color = Color.Lerp(defaultSpriteRendererColor, flashColor, colorChangeTimer / colorChangeDuration);

            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                colorChangeTimer += Time.deltaTime;

            yield return null;
        }

        // A timer used to store how long the coroutine has been waiting since the character was changed to the flash color.
        float waitTimer = 0.0f;
        // A pause-friendly loop which causes the coroutine to wait until the specified duration has passed - throughout this period flashColorCoroutine will have a value, which means the IsColorFlashing property will return true.
        while (waitTimer < flashDuration)
        {
            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                waitTimer += Time.deltaTime;

            yield return null;
        }

        // Calls the StopColorFlashing() method so that colorFlashCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless an overload of this coroutine is running.
        StopColorFlashing();
    }
    // A coroutine which causes the character to gradually change to, flash, and then gradually change back from the specified color.
    private IEnumerator FlashColorCoroutine(Color flashColor, float changeToFlashColorDuration, float flashDuration, float changeToOriginalColorDuration)
    {
        // Ensures that the character isn't already flashing a color and that the given durations are valid.
        if (IsColorFlashing)
            throw new System.InvalidOperationException("The character is already flashing a color.");
        if (changeToFlashColorDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to gradually change to a color must be greater than or equal to zero.");
        if (flashDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to flash a color must be greater than than or equal to zero.");
        if (changeToOriginalColorDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to gradually change back to it's original color must be greater than than or equal to zero.");

        // The sprite renderer of the child object which displays the character sprite.
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // A timer used to store the duration in seconds for which the character color has been changing from the original color to the flash color.
        float colorChangeTimer = 0.0f;
        // Loops until the color has been completely changed from the original to the flash color.
        while (colorChangeTimer < changeToFlashColorDuration)
        {
            spriteRenderer.color = Color.Lerp(defaultSpriteRendererColor, flashColor, colorChangeTimer / changeToFlashColorDuration);

            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                colorChangeTimer += Time.deltaTime;

            yield return null;
        }
        // Ensures that the character color is set to the flash color if the changeToFlashColorDuration is zero.
        spriteRenderer.color = flashColor;

        // A timer used to store how long the coroutine has been waiting since the character was changed to the flash color.
        float waitTimer = 0.0f;
        // A pause-friendly loop which causes the coroutine to wait until the specified duration has passed - throughout this period flashColorCoroutine will have a value, which means the IsColorFlashing property will return true.
        while (waitTimer < flashDuration)
        {
            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                waitTimer += Time.deltaTime;

            yield return null;
        }

        colorChangeTimer = 0.0f;
        // Loops until the color has been completely changed from the flash color to the original color.
        while (colorChangeTimer < changeToOriginalColorDuration)
        {
            spriteRenderer.color = Color.Lerp(flashColor, defaultSpriteRendererColor, colorChangeTimer / changeToOriginalColorDuration);

            // Progresses through the loop if the character is currently unpaused.
            if (!IsPaused)
                colorChangeTimer += Time.deltaTime;

            yield return null;
        }
        // Ensures that the character is set to it's original color if the changeToOriginalColorDuration is zero.
        spriteRenderer.color = defaultSpriteRendererColor;

        // Calls the StopColorFlashing() method so that colorFlashCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless an overload of this coroutine is running.
        StopColorFlashing();
    }


    // Called when the script is loaded.
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        // Sets the enemy rigidbody to be kinematic so that it can be moved programmatically whilst still colliding with the physics objects in the scene.
        GetComponent<Rigidbody2D>().isKinematic = true;
        // Stores the initial color of the character sprite.
        defaultSpriteRendererColor = GetComponentInChildren<SpriteRenderer>().color;

        AwakeAddendum();
    }

    // Called when the script is initialised.
    private void Start()
    {
        IsPaused = false;
        heading = Vector2.down;
        freezeCoroutine = null;
        colorFlashCoroutine = null;
        knockBackForce = Vector2.zero;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        if (!IsPaused && !IsFrozen)
        {
            UpdateMovement();
            UpdateAttacking();
        }

        if (knockBackForce != Vector2.zero)
        {
            this.transform.Translate(knockBackForce * Time.deltaTime);

            knockBackForce = Vector2.ClampMagnitude(knockBackForce, knockBackForce.magnitude - (3.0f * Time.deltaTime));

            if (knockBackForce.magnitude < 0.1f)
                knockBackForce = Vector2.zero;
        }
    }
}