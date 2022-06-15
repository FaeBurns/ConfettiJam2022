using BeanLib.References;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component responsible for handling the player's attacks.
/// </summary>
public class PlayerAttack : ReferenceResolvedBehaviour
{
    private float cooldownFinishedTime = 0f;

    [AutoReference] private TimeResourceManager timeManager;
    [BindComponent(Child = true)] private TriggerCountCheck meleeOverlapTrigger = null;

    [Header("References")]
    [SerializeField] private Transform mouseDirectionTransform;

    [Header("Combined")]
    [SerializeField] private LayerMask attackLayer;

    [Header("Melee")]
    [SerializeField] private float meleeCooldown = 0.1f;
    [SerializeField] private float meleeDamage = 5f;

    [Header("Ranged")]
    [SerializeField] private float rangedCooldown = 0.15f;
    [SerializeField] private float rangedPelletDamage = 2f;
    [SerializeField] private int rangedPelletCount = 6;
    [SerializeField] private float rangedMaxDistance = 5f;
    [SerializeField] private float rangedSpread = 5f;
    [SerializeField] private float rangedSpreadDeviation = 1f;
    [SerializeField] private float rangedTimeCost = 1f;

    [Header("Animation")]
    [SerializeField] private Animator meleeAnimator;
    [SerializeField] private string[] attackAnimationLayerNames;
    [SerializeField] private GameObject pelletAnimationPrefab;
    [SerializeField] private float pelletAnimationLifetime = 0.5f;
    [SerializeField] private float pelletAnimationSpeed = 2f;
    [SerializeField] private Transform shotgunOrigin;

    private void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        float angleRad = Mathf.Atan2(mouseWorldPos.y - transform.position.y, mouseWorldPos.x - transform.position.x);
        float targetAngle = angleRad * Mathf.Rad2Deg;

        mouseDirectionTransform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        if (Input.GetMouseButtonDown(0))
        {
            DoMeleeAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DoRangedAttack(targetAngle);
        }
    }

    private void DoMeleeAttack()
    {
        // if still on cooldown
        if (cooldownFinishedTime > Time.time)
        {
            return;
        }

        string layerName = attackAnimationLayerNames[Mathf.RoundToInt(Random.value * (attackAnimationLayerNames.Length - 1))];

        meleeAnimator.PlayInFixedTime(layerName, -1, 0);

        // convert to array to avoid enumeration issues if one of the targets dies.
        foreach (GameObject collidingObject in meleeOverlapTrigger.Objects.ToArray())
        {
            // try and get damageable from target object
            Damageable damageable = collidingObject.GetComponent<Damageable>();

            // if not found in initial search
            if (damageable == null)
            {
                // check parent
                damageable = collidingObject.GetComponentInParent<Damageable>();
            }

            // if found now
            if (damageable != null)
            {
                // deal damage
                damageable.DealDamage(meleeDamage, gameObject, DamageType.Melee);
            }
        }

        cooldownFinishedTime = Time.time + meleeCooldown;
    }

    private void DoRangedAttack(float targetAngle)
    {
        // if still on cooldown
        if (cooldownFinishedTime > Time.time)
        {
            return;
        }

        float halfPelletCount = rangedPelletCount / 2f;
        float individualSpread = rangedSpread / rangedPelletCount;

        for (int i = 0; i < rangedPelletCount; i++)
        {
            float angle = (targetAngle - ((i - halfPelletCount) * individualSpread)) - (individualSpread / 2f);

            float deviation = rangedSpreadDeviation * ((Random.value * 2) - 1);

            angle += deviation;

            // use Vector3.right here as we're using 2d where up is forwards
            Vector3 forwardsVector = Quaternion.Euler(0, 0, angle) * Vector3.right;

            RaycastHit2D hit = Physics2D.Raycast(shotgunOrigin.position, forwardsVector, rangedMaxDistance, attackLayer);

            // was anything actually hit
            if (hit.collider != null)
            {
                Damageable damageable = hit.collider.gameObject.GetComponentInParent<Damageable>();
                if (damageable != null)
                {
                    damageable.DealDamage(rangedPelletDamage, gameObject, DamageType.Ranged);
                }
            }

            GameObject pelletObject = Instantiate(pelletAnimationPrefab, shotgunOrigin.position, Quaternion.identity);
            ShotgunPelletMovement pelletMover = pelletObject.GetComponent<ShotgunPelletMovement>();

            pelletMover.Velocity = forwardsVector * pelletAnimationSpeed;

            Destroy(pelletObject, pelletAnimationLifetime);
        }

        cooldownFinishedTime = Time.time + rangedCooldown;

        timeManager.Drain(rangedTimeCost);
    }
}