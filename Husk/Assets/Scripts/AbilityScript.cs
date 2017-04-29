//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        29/04/17
// Date last edited:    29/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// An abstract script used to implement the different types of actions/attacks which characters can perform.
public abstract class AbilityScript : MonoBehaviour
{
    // Whether or not the ability object is currently locked into an unchanging state.
    [HideInInspector]
    public bool IsPaused { get; set; }
    // The duration in seconds for which the ability object is active before it self-destructs.
    public float Duration;

    // Initialises the ability object using the specified character and attaches the ability to it as a child.
    public void InitialiseUsingCharacter(Transform character)
    {
        // Ensures that the spcified object is a valid game character.
        if (character.GetComponent<CharacterScript>() == null)
            throw new System.ArgumentException("The ability object must be initialised using an object which has a derivative of CharacterScript attached as a component.");

        parentCharacter = character;
        this.transform.SetParent(parentCharacter);
    }


    // The parent character which the ability object effects.
    protected Transform parentCharacter;

    // An empty virtual method which can be used by derived classes to add extra functionality to the Update() method.
    protected virtual void UpdateAddendum() { }


    // The amount of time in seconds for which the ability object has existed.
    private float existTime = 0.0f;

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        existTime += Time.deltaTime;
        if (existTime >= Duration)
            Destroy(this.gameObject);

        if (!IsPaused)
            UpdateAddendum();
    }
}