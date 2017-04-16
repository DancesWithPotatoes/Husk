//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        01/04/17
// Date last edited:    16/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle a temporary direction indicator object used to visualise the current heading of the parent player character.
public class PlayerDirectionIndicatorScript : MonoBehaviour
{
    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        // Rotates the direction indicator to face in the same direction as the parent player heading.
        this.transform.GetComponentInParent<PlayerScript>().RotateChildObjectToFacePlayerHeading(this.transform);
    }
}