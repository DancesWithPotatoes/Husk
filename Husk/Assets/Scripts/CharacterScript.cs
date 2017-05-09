//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        16/04/17
// Date last edited:    09/05/17
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
    // The weight of the character, used to determine how quickly any knockback effect fades.
    public float Weight;

    // The property used to get the current heading of the character.
    public Vector2 Heading
    {
        get { return heading; }
    }

    // The property used to get whether the character is currently unable to move itself.
    public bool IsFrozen
    {
        // freezeCoroutine will have a value if FreezeCoroutine() is running, else it will always be equal to null - the character will also be considered frozen if under the influence of knockback.
        get { return (freezeCoroutine != null || knockBackForce != Vector2.zero); }
    }

    // The property used to get whether the character is currently flashing a color.
    public bool IsColorFlashing
    {
        // colorFlashCoroutine will have a value if one of the FlashColorCoroutine() overloads are running, else it will always be equal to null.
        get { return (colorFlashCoroutine != null); }
    }

    // Applies various damage effects to the character if it isn't currently invincible.
    public void Damage(Vector2 attackKnockbackForce, float attackInvincibilityDuration, float attackStaggerDuration, float screenShakeMagnitude, float hitStutterDuration)
    {
        // Ensures that the stagger and invincibility duration values are greater than or equal to zero.
        if (attackStaggerDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("THe specified stagger duration must be greater than or equal to zero.");
        if (attackInvincibilityDuration < 0.0f)
            throw new System.ArgumentOutOfRangeException("THe specified invincibility duration must be greater than or equal to zero.");

        // If the character isn't invincible, applies the damage effects.
        if (invincibilityTimer == 0.0f)
        {
            // If the damage dealt to the character doesn't apply knockback but does apply stagger, instantly applies the stagger effect before the character is made invincible and thus immune to being frozen.
            if (attackKnockbackForce == Vector2.zero && attackStaggerDuration > 0.0f)
            {
                if (IsFrozen)
                    Unfreeze();
                Freeze(attackStaggerDuration);
            }

            // Updates the character members which will continue to effect the character after the method is completed.
            this.knockBackForce = attackKnockbackForce;
            this.invincibilityTimer = attackInvincibilityDuration;
            this.previousAttackStaggerDuration = attackStaggerDuration;

            // Causes the screen to shake for the duration of the of the character invincibility duration.
            if (screenShakeMagnitude > 0.0f && attackInvincibilityDuration > 0.0f)
            {
                // The script used to handle the main camera.
                MainCameraScript cameraScript = Camera.main.GetComponent<MainCameraScript>();
                // Shakes the camera for the duration of the damaged player invinciblity.
                if (cameraScript.IsShaking)
                    cameraScript.StopShaking();
                cameraScript.Shake(attackInvincibilityDuration, screenShakeMagnitude);
            }

            // Temporarily pauses the gameplay to create a 'hit stutter' effect, adding impact to the damage of the attack.
            if (hitStutterDuration > 0.0f)
                GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>().PauseScene(hitStutterDuration);  

            // Causes the character to flash red.
            if (IsColorFlashing)
                StopColorFlashing();
            FlashColor(Color.red, 0.2f);

            DamageAddendum();
        }
    }

    // Stops the character from being able to move itself for the specified duration - not applicable if it's currently invincible.
    public void Freeze(float duration)
    {
        if (invincibilityTimer == 0.0f)
            freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    // Unfreezes the character if it is currently frozen - doesn't cut short any knockback effect.
    public void Unfreeze()
    {
        if (freezeCoroutine != null)
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
    // The force (applied through gameplay code, not Unity's physics engine) currently being applied to the character to simulate 'knockback' from an attack.
    private Vector2 knockBackForce;
    // The duration to freeze the character after the knockback effect of the previous attack has worn off.
    private float previousAttackStaggerDuration;
    // The amount of time in seconds for which the character will be impervious to newly-applied damage and status effects such as knockback and stagger.
    private float invincibilityTimer;

    // A coroutine which freezes the character in place for the specified duration, then unfreezes it.
    private IEnumerator FreezeCoroutine(float duration)
    {
        if (IsPaused)
            throw new System.InvalidOperationException("The character can't be frozen when paused.");
        if (invincibilityTimer > 0.0f)
            throw new System.InvalidOperationException("The character can't be frozen when invincible.");
        if (IsFrozen && knockBackForce == Vector2.zero)
            throw new System.InvalidOperationException("The character has already been manually frozen in a manner that isn't a result of knockback.");
        if (duration <= 0.0f)
            throw new System.ArgumentOutOfRangeException("The specified duration to freeze the character must be greater than zero.");

        // A timer used to store how long the coroutine has been waiting since the character was initially frozen.
        float waitTimer = 0.0f;
        // A pause-friendly loop which causes the coroutine to wait until the specified duration has passed - throughout this period freezeCoroutine will have a value, which means the IsFrozen property will return true.
        while (waitTimer < duration)
        {
            // Progresses through the loop if the character is currently unpaused and not frozen specifically as a result of knockback.
            if (!IsPaused && knockBackForce == Vector2.zero)
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
        previousAttackStaggerDuration = 0.0f;
        invincibilityTimer = 0.0f;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        if (!IsPaused)
        {
            // If the character is currently invincible, decrements the invincibility timer.
            if (invincibilityTimer > 0.0f)
            {
                invincibilityTimer -= Time.deltaTime;
                if (invincibilityTimer < 0.0f)
                    invincibilityTimer = 0.0f;
            }

            // If the character isn't frozen or being effected by knockback, updates it's movement and attacking status.
            if (!IsFrozen)
            {
                UpdateMovement();
                UpdateAttacking();
            }

            UpdateKnockback();
        }
    }

    // Called each time the GUI elements of the scene are updated.
    private void OnGUI()
    {
        // If the character is invincible, draws a white 'I' character on it.
        if (invincibilityTimer > 0.0f)
            DrawTextOnCharacter(new Vector2(-9.0f, -10.0f), "I");

        // If the character is frozen, draws a white 'F' character on it.
        if (IsFrozen)
            DrawTextOnCharacter(new Vector2(1.0f, -10.0f), "F");
    }

    // Updates the influence of the knockback effect on the character.
    private void UpdateKnockback()
    {
        // Else if the character is currently being effected by knockback, continues moving it to simulate being forced backwards and decrements the knockback force to account for friction.
        if (knockBackForce != Vector2.zero)
        {
            this.transform.Translate(knockBackForce * Time.deltaTime);

            // Reduces the magnitude of the knockback force by the weight value of the character every second.
            knockBackForce = Vector2.ClampMagnitude(knockBackForce, knockBackForce.magnitude - (Weight * Time.deltaTime));

            // If the knockback effect is over, staggers the character according to the duration specified by the previous attack.
            if (knockBackForce.magnitude < 0.1f)
            {
                knockBackForce = Vector2.zero;

                if (IsFrozen)
                    Unfreeze();
                // A temporary variable used to store the current value of the invincibility timer.
                float tempInvincibilityTimer = 0.0f;
                // Stores the value of the invincibility timer in the temp variable and sets it to zero, as the character cannot be frozen when the invincibility timer is greater than zero.
                if (invincibilityTimer > 0.0f)
                {
                    tempInvincibilityTimer = invincibilityTimer;
                    invincibilityTimer = 0.0f;
                }
                // Freezes the character to simulate the stagger effect.
                Freeze(previousAttackStaggerDuration);
                // Resets the invincibility timer to it's original state.
                invincibilityTimer = tempInvincibilityTimer;

                previousAttackStaggerDuration = 0.0f;
            }
        }
    }

    // Renders the specified string as GUI text on the character.
    private void DrawTextOnCharacter(Vector2 offsetFromCenterInPixels, string text)
    {
        // The position of the character in screen space (y-value is inverted because the screen origin is at the bottom-left).
        Vector2 screenSpacePosition = Camera.main.WorldToScreenPoint(new Vector2(this.transform.position.x, -this.transform.position.y));

        // Adjusts the screen position so that the text will be rendered at the specified offset from the center of the character in screen-space pixels.
        screenSpacePosition.x += offsetFromCenterInPixels.x;
        screenSpacePosition.y += offsetFromCenterInPixels.y;

        // Displays the text on the character using a GUI label.
        GUI.Label(new Rect(screenSpacePosition, new Vector2(9999.9f, 9999.9f)), text);
    }
}