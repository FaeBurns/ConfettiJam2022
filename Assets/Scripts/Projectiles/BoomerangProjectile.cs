using System;
using System.Collections;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for handling the movement of a boomerang projectile.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BoomerangProjectile : ReferenceResolvedBehaviour
{
    private float endTime;
    private float startTime;
    private Vector2 offset;
    private float rotationOffset;
    private float distanceScale;

    [BindComponent] private Rigidbody2D rb;

    [SerializeField] private float duration = 2f;
    [SerializeField] private AnimationCurve distanceCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private float minimumDistanceScale = 5f;
    [SerializeField] private float rotationScale = 60f;

    [Header("Visuals")]
    [SerializeField] private Transform smallHandTransform;
    [SerializeField] private Transform bigHandTransform;
    [SerializeField] private float smallHandRotationSpeed;
    [SerializeField] private float bigHandRotationSpeed;

    /// <summary>
    /// Event fired when the projectile finishes.
    /// </summary>
    public event Action<GameObject> Finished;

    /// <summary>
    /// Gets or sets the target player object.
    /// </summary>
    public GameObject PlayerObject { get; set; }

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        startTime = Time.time;
        endTime = startTime + duration;

        offset = transform.position;
        rotationOffset = transform.rotation.eulerAngles.z;

        distanceScale = Mathf.Max(Vector2.Distance(transform.position, PlayerObject.transform.position), minimumDistanceScale);

        Debug.Log($"rotational offset: {rotationOffset}");
    }

    private void Update()
    {
        // if time is up
        if (endTime <= Time.time)
        {
            // remove self
            Destroy(gameObject);
        }

        Vector2 distance = new Vector2(distanceCurve.Evaluate(Time.time - startTime), 0) * distanceScale;
        float rotation = rotationCurve.Evaluate(Time.time - startTime) * rotationScale;
        Vector2 position = distance.Rotate(Vector2.zero, rotation + rotationOffset);

        transform.position = position + offset;

        smallHandTransform.rotation = Quaternion.Euler(smallHandTransform.rotation.eulerAngles + new Vector3(0, 0, smallHandRotationSpeed * Time.deltaTime));
        bigHandTransform.rotation = Quaternion.Euler(bigHandTransform.rotation.eulerAngles + new Vector3(0, 0, bigHandRotationSpeed * Time.deltaTime));
    }

    private void OnDestroy()
    {
        Finished?.Invoke(gameObject);
    }
}