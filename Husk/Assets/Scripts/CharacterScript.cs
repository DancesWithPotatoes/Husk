//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        16/04/17
// Date last edited:    16/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of a character within the world which can attack, move, and be damaged by other characters.
public class CharacterScript : MonoBehaviour
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

    // The property used to get whether the currently has a child attack object spawned.
    public bool IsAttacking
    {
        get { return (this.transform.GetComponentInChildren<AttackScript>() != null); }
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
    private Vector2 heading;
    // Whether or not the character is currently frozen in place.
    private bool isFrozen;

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

    // Called when the script is initialised.
    private void Start()
    {
        isFrozen = false;
        heading = Vector2.down;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        UpdateMovement();
        UpdateAttacking();

        OutputDebugData();
    }

    // Updates the movement of the character.
    private void UpdateMovement()
    {
        if (!isFrozen)
        {
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            // Clamps the movement magnitude to 1.0f so that the player will move at the same maximum speed in all directions.
            movement = Vector2.ClampMagnitude(movement, 1.0f);

            this.transform.Translate(movement * MoveSpeed * Time.deltaTime);

            // If the movement direction of the player has changed, updates the normalised heading vector.
            if (movement.magnitude > 0.0f && heading != movement.normalized)
                heading = movement.normalized;
        }
    }

    // Updates the attacking status of the character.
    private void UpdateAttacking()
    {
        if (Input.GetButtonDown("LightAttack"))
        {
            if (!IsAttacking)
                Attack();
        }
    }

    // If the player isn't already attacking, spawns a temporary attack object which will damage enemies with which it collides.
    private void Attack()
    {
        if (!IsAttacking)
        {
            // Spawns a new attack object and sets it to be a child of the player character.
            Transform attackObject = (Transform)Instantiate(AttackPrefab, this.transform.position, Quaternion.identity);
            attackObject.GetComponent<AttackScript>().InitialiseUsingPlayer(this.transform);
            attackObject.name = "Player Attack";
        }
    }

    // Outputs visual representations and logs of the inner workings of the player object for debugging.
    private void OutputDebugData()
    {
        // Draws a line representing the heading of the player on the scene window.
        Debug.DrawLine(this.transform.position, (Vector2)this.transform.position + heading, Color.green, Time.deltaTime, false);
    }
}