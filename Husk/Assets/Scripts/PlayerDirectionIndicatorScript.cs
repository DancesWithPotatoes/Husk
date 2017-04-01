//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        01/04/17
// Date last edited:    01/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle a temporary direction indicator used to visualise the current heading of the player character.
public class PlayerDirectionIndicatorScript : MonoBehaviour
{
    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        // The current heading of the parent player object.
        Vector2 direction = this.transform.GetComponentInParent<PlayerScript>().Heading;

        direction.Normalize();
        // The angle in degrees to rotate the direction indicator object in local space so that it is facing in the same direction as the player heading.
        float attackAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;

        this.transform.localRotation = Quaternion.AngleAxis(attackAngle, Vector3.forward);
    }
}