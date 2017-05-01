//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        29/04/17
// Date last edited:    01/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// An ability object which allows a parent character to perform a dodge.
public class DodgeAbilityScript : AbilityScript
{
    // Called each frame and used to update gameplay logic.
    protected override void UpdateAddendum() 
    {
        // TEST!
        //if (!parentCharacter.GetComponent<CharacterScript>().IsColorFlashing)
        //{
        //    parentCharacter.GetComponent<CharacterScript>().FlashColor(new Color(Random.RandomRange(0.0f, 1.0f), Random.RandomRange(0.0f, 1.0f), Random.RandomRange(0.0f, 1.0f)), 0.33f, 0.33f, 0.33f); 
        //}
    }
}