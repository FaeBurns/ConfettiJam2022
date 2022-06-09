using System.Linq;
using BeanLib.References;
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
    [SerializeField] private float rangedTimeCost = 1f;

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

        foreach (GameObject collidingObject in meleeOverlapTrigger.Objects)
        {
            Damageable damageable = collidingObject.GetComponent<Damageable>();

            if (damageable != null)
            {
                damageable.DealDamage(meleeDamage);
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

            // use Vector3.right here as we're using 2d where up is forwards
            Vector3 forwardsVector = Quaternion.Euler(0, 0, angle) * Vector3.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, forwardsVector, rangedMaxDistance, attackLayer);

            // was anything actually hit
            if (hit.collider != null)
            {
                Damageable damageable = hit.collider.gameObject.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.DealDamage(rangedPelletDamage);
                }

                Debug.DrawLine(transform.position, hit.point, Color.green, 0.25f);
            }

            Debug.DrawLine(transform.position, transform.position + (forwardsVector * rangedMaxDistance), Color.red, 0.25f);
        }

        cooldownFinishedTime = Time.time + rangedCooldown;

        timeManager.Drain(rangedTimeCost);
    }
}