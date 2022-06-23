using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for showing the directions to the collectibles.
/// </summary>
public class ClockCompass : MonoBehaviour
{
    private GameObject player;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField] private List<Collectible> targets = new List<Collectible>();

    [SerializeField] private GameObject prefab;

    private void Start()
    {
        targets.Execute((target) => target.OnCollect += (_) => Target_OnCollect(target));
        Spawn();

        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(player.transform.position);

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            GameObject uiObject = spawnedObjects[i];
            Collectible target = targets[i];

            Vector2 direction = target.transform.position - Camera.main.transform.position;

            float angle = Vector2.SignedAngle(Vector2.right, direction);

            uiObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Target_OnCollect(Collectible target)
    {
        Remove(target);
        Update();
    }

    private void Spawn()
    {

        for (int i = 0; i < targets.Count; i++)
        {
            spawnedObjects.Add(Instantiate(prefab, transform).transform.gameObject);
        }
    }

    private void Remove(Collectible targetForRemoval)
    {
        // remove target
        targets.Remove(targetForRemoval);

        // remove one of the trackers
        Destroy(spawnedObjects[0]);
        spawnedObjects.RemoveAt(0);
    }
}