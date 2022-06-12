using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for displaying a trail line in local space.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LocalTrailRenderer : ReferenceResolvedBehaviour
{
    // Found at https://forum.unity.com/threads/trail-renderer-local-space.97756/
    // Modified to work with modern versions of unity and to fit the required use

    // Author: Eric Hodgson 2017

    // NOTE: This script should be placed on the parent of the object that's moving.
    //  It should be drawn in the parent's local space, and move/rotate with the parent.
    //  If the trail script is on the moving object, the entire trail will move and
    //   rotate with the object itself, instead of leaving a trail behind in it's
    //   parent's coordinate system.

    public bool Emitting;

    // internal log of last trail point... could also use myLine.GetPosition(myLine.numPositions)
    private Vector3 lastPosition;

    [BindComponent] private LineRenderer myLine;

    [Tooltip("Object that is leaving the trail")]
    [SerializeField] private Transform objToFollow;

    [Tooltip("Object to use as world reference")]
    [SerializeField] private Transform worldReferenceObj;

    [Tooltip("How far should the object move before leaving a new trail point")]
    [SerializeField] private float distIncrement = 0.1f;

    [Tooltip("Toggle this to make trail be a finite number of segments")]
    [SerializeField] private bool limitTrailLength = false;

    [Tooltip("Set the number of segments here")]
    [SerializeField] private int maxPositions = 10;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        // fix error in editor
        if (myLine == null)
        {
            return;
        }

        // ....and make sure it's set to use local space
        myLine.useWorldSpace = false;

        // Reset the trail
        Reset();
    }

    private void Reset()
    {
        // Wipe out any old positions in the LineRenderer
        myLine.positionCount = 0;

        // Then set the first position to our object's current local position
        AddPoint(GetWorldPosition());
    }

    // Add a new point to the line renderer on demand
    private void AddPoint(Vector3 newPoint)
    {
        // Increase the number of positions to render by 1
        myLine.positionCount += 1;

        // Set the new, last item in the Vector3 list to our new point
        myLine.SetPosition(myLine.positionCount - 1, newPoint);

        // Check to see if the list is too long
        if (limitTrailLength && myLine.positionCount > maxPositions)
        {
            // ...and discard old positions if necessary
            TruncatePositions(maxPositions);
        }

        // Log this position as the last one logged
        lastPosition = newPoint;
    }

    // Shorten position list to the desired amount, discarding old values
    private void TruncatePositions(int newLength)
    {
        // Create a temporary list of the desired length
        Vector3[] tempList = new Vector3[newLength];

        // Calculate how many extra items will need to be cut out from the original list
        int nExtraItems = myLine.positionCount - newLength;

        // Loop through original list and add newest X items to temp list
        for (int i = 0; i < newLength; i++)
        {
            // shift index by nExtraItems... e.g., if 2 extras, start at index 2 instead of index 0
            tempList[i] = myLine.GetPosition(i + nExtraItems);
        }

        // Set the LineRenderer's position list length to the appropriate amount
        myLine.positionCount = newLength;

        // ...and use our tempList to fill it's positions appropriately
        myLine.SetPositions(tempList);
    }

    private void Update()
    {
        // Get the current position of the object in local space
        Vector3 curPosition = GetWorldPosition();

        // Check to see if object has moved far enough
        if (Vector3.Distance(curPosition, lastPosition) > distIncrement)
        {
            // ..and add the point to the trail if so
            AddPoint(curPosition);
        }
    }

    private Vector3 GetWorldPosition()
    {
        return RotateAroundPivot(objToFollow.localPosition, worldReferenceObj.localPosition, worldReferenceObj.localRotation);
    }

    private Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        // get in local space
        Vector3 localPoint = point - pivot;

        // rotate
        localPoint = angle * localPoint;

        // return to world space
        return localPoint + pivot;
    }
}