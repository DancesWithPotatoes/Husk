//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        16/04/17
// Date last edited:    24/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// An abstract script which when derived will be used to handle the actions of a character within the world that can attack, move, and be damaged by other character-derived objects.
public abstract class CharacterScript : MonoBehaviour
{
    // The prefab used which allows the character to spawn an attack object to damage enemy characters.
    public Transform AttackPrefab;
    // The maximum movement speed of the character.
    public float MoveSpeed;

    // The property used to get the current heading of the character.
    public Vector2 Heading
    {
        get { return heading; }
    }

    // The property used to get whether the character currently has a child attack object spawned.
    public bool IsAttacking
    {
        get { return (this.transform.GetComponentInChildren<AttackScript>() != null); }
    }

    // The property used to get whether the child is currently frozen.
    public bool IsFrozen
    {
        // freezeCoroutine will have a value if FreezeCoroutine() is running, else it will always be equal to null.
        get { return (freezeCoroutine != null); }
    }

    // The property used to get whether the character is currently flashing a color.
    public bool IsColorFlashing
    {
        // colorFlashCoroutine will have a value if one of the FlashColorCoroutine() overloads are running, else it will always be equal to null.
        get { return (colorFlashCoroutine != null); }
    }

    // Damages the character.
    public virtual void Damage()
    {
        // Causes the character to flash red.
        if (IsColorFlashing)
            StopColorFlashing();
        FlashColor(Color.red, 0.2f);
    }

    // Freezes the character in place for the specified duration.
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


    // A normalised vector representing the direction in which the character is currently facing.
    protected Vector2 heading;
            
    // An abstract method which will be used to implement the movement of the character in derived scripts.
    protected abstract void UpdateMovement();

    // An abstract method which will be used to implement the attacking of the character in derived scripts.
    protected abstract void UpdateAttacking();


    // The default color tint of the character's SpriteRenderer component.
    private Color defaultSpriteRendererColor;
    // The coroutine which is currently causing the character to be frozen.
    private Coroutine freezeCoroutine;
    // The coroutine which is currently causing the character to color flash.
    private Coroutine colorFlashCoroutine;

    // A coroutine which freezes the character in place for the specified duration, then unfreezes it.
    private IEnumerator FreezeCoroutine(float duration)
    {
        // Ensures that the character isn't already frozen and that the given freeze duration is valid.
        if (IsFrozen)
            throw new System.InvalidOperationException("The character is already frozen.");
        if (duration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration to freeze the character must be greater than zero.");

        // Pauses the coroutine until the specified duration has passed - throughout this period freezeCoroutine will have a value, which means the IsFrozen property will return true.
        yield return new WaitForSeconds(duration);

        // Calls the Unfreeze() method so that freezeCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless an overload of this coroutine is running.
        Unfreeze();
    }

    // A coroutine which causes the character to temporarily flash the specified color.
    private IEnumerator FlashColorCoroutine(Color flashColor, float duration)
    {
        // Ensures that the character isn't already flashing a color and that the given flash duration is valid.
        if (IsColorFlashing)
            throw new System.InvalidOperationException("The character is already flashing a color.");
        if (duration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to flash a color must be greater than zero.");

        // The sprite renderer of the child object which displays the character sprite.
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = flashColor;

        // Pauses the coroutine until the specified duration has passed.
        yield return new WaitForSeconds(duration);

        // Calls the StopColorFlashing() method so that colorFlashCoroutine will be set to null when this coroutine finishes, meaning that it will always be null unless an overload of this coroutine is running.
        StopColorFlashing();
    }
    // A coroutine which causes the character to gradually change to and then temporarily flash the specified color.
    private IEnumerator FlashColorCoroutine(Color flashColor, float colorChangeDuration, float flashDuration)
    {
        // Ensures that the character isn't already flashing a color and that the given durations are valid.
        if (IsColorFlashing)
            throw new System.InvalidOperationException("The character is already flashing a color.");
        if (colorChangeDuration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to gradually change to a color must be greater than zero.");
        if (flashDuration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration for the character to flash a color must be greater than zero.");

        // The sprite renderer of the child object which displays the character sprite.
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // A timer used to store the duration in seconds for which the character color has been changing from the original color to the flash color.
        float colorChangeTimer = 0.0f;
        // Loops until the color has been completely changed from the original to the flash color.
        while (colorChangeTimer < colorChangeDuration)
        {
            spriteRenderer.color = Color.Lerp(defaultSpriteRendererColor, flashColor, colorChangeTimer / colorChangeDuration);

            colorChangeTimer += Time.deltaTime;
            yield return null;
        }

        // Pauses the coroutine until the specified flash duration has passed.
        yield return new WaitForSeconds(flashDuration);

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
    }

    // Called when the script is initialised.
    private void Start()
    {
        heading = Vector2.down;
        freezeCoroutine = null;
        colorFlashCoroutine = null;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        UpdateMovement();
        UpdateAttacking();
    }
}