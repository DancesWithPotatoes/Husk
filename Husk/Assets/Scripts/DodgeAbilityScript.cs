//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        29/04/17
// Date last edited:    29/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// An implementation of AbilityScript which allows the parent character of the GameObject to which this is attached to dodge.
public class DodgeAbilityScript : AbilityScript
{
    // Called each frame and used to update gameplay logic.
    protected override void UpdateAddendum() 
    {
        // TEST!
        if (!parentCharacter.GetComponent<CharacterScript>().IsColorFlashing)
        {
            parentCharacter.GetComponent<CharacterScript>().FlashColor(new Color(Random.RandomRange(0.0f, 1.0f), Random.RandomRange(0.0f, 1.0f), Random.RandomRange(0.0f, 1.0f)), 0.33f, 0.33f, 0.33f); 
        }
    }
}