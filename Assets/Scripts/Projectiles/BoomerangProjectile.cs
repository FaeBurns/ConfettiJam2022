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
    private float startTime;
    private Vector2 offset;
    private float rotationOffset;
    private float distanceScale;

    [BindComponent] private ContactDamageDealer damager;

    [SerializeField] private float duration = 2f;
    [SerializeField] private AnimationCurve distanceCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve scaleCurve;
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

    /// <summary>
    /// Gets or sets the object that spawns this projectile.
    /// </summary>
    public GameObject Spawner { get; set; }

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        startTime = Time.time;

        offset = transform.position;
        rotationOffset = transform.rotation.eulerAngles.z;

        distanceScale = Mathf.Max(Vector2.Distance(transform.position, PlayerObject.transform.position), minimumDistanceScale);

        damager.BlameObject = Spawner;

        Destroy(gameObject, duration);
    }

    private void Update()
    {
        float time = (Time.time - startTime) / duration;

        Vector2 distance = new Vector2(distanceCurve.Evaluate(time), 0) * distanceScale;
        float rotation = rotationCurve.Evaluate(time) * rotationScale;
        Vector2 position = distance.Rotate(Vector2.zero, rotation + rotationOffset);

        transform.position = position + offset;

        smallHandTransform.rotation = Quaternion.Euler(smallHandTransform.rotation.eulerAngles + new Vector3(0, 0, smallHandRotationSpeed * Time.deltaTime));
        bigHandTransform.rotation = Quaternion.Euler(bigHandTransform.rotation.eulerAngles + new Vector3(0, 0, bigHandRotationSpeed * Time.deltaTime));
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleCurve.Evaluate(time));
    }

    private void OnDestroy()
    {
        Finished?.Invoke(gameObject);
    }
}