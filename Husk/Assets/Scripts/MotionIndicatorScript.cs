//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        07/05/17
// Date last edited:    07/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A basic script which causes an object to constantly move back and forth between two points, used to clearly indicate when the game is paused or 'stuttering'.
public class MotionIndicatorScript : MonoBehaviour
{
    // The first of two points which the motion indicator object will lerp between.
    public Vector2 PointA;
    // The second of two points which the motion indicator object will lerp between.
    public Vector2 PointB;
    // Whether the motion indicator object is currently paused.
    public bool IsPaused;
    // The amount of time in seconds which the motion indicator object takes to travel between points.
    public float TravelDuration;


    // Whether the motion indicator object is currently travelling to point A - else it will be travelling to B.
    private bool isTravellingToPointA = false;
    // The amount of time in seconds for which the motion indicator object has been travelling to it's current point.
    private float travelTimer = 0.0f;
    
    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        if (!IsPaused)
        {
            travelTimer += Time.deltaTime;
            if (travelTimer >= TravelDuration)
            {
                isTravellingToPointA = !isTravellingToPointA;
                travelTimer = 0.0f;
            }

            this.transform.position = Vector2.Lerp(isTravellingToPointA ? PointB : PointA, isTravellingToPointA ? PointA : PointB, travelTimer / TravelDuration);
        }
    }
}