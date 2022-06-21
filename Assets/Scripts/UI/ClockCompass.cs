using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockCompass : MonoBehaviour
{
    private List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField] private List<Collectible> targets = new List<Collectible>();

    [SerializeField] private GameObject prefab;

    private void Start()
    {
        targets.Execute((target) => target.OnCollect += Target_OnCollect);
        Spawn();
    }

    private void Update()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            GameObject uiObject = spawnedObjects[i];
            Collectible target = targets[i];

            Vector2 direction = target.transform.position - transform.position;

            uiObject.transform.forward = direction;
        }
    }

    private void Target_OnCollect(Sprite obj)
    {
        Cull();
        Spawn();
        Update();
    }

    private void Spawn()
    {
        int count = GetValidCount();

        int diff = count - targets.Count;

        for (int i = 0; i < diff; i++)
        {
            spawnedObjects.Add(Instantiate(prefab, transform).transform.parent.gameObject);
        }
    }

    private void Cull()
    {
        int count = GetValidCount();

        int passed = 0;
        foreach (GameObject obj in spawnedObjects)
        {
            if (passed >= count)
            {
                Destroy(obj);
            }
            passed++;
        }
    }

    private int GetValidCount()
    {
        int validCount = 0;
        foreach (Collectible obj in targets)
        {
            if (obj != null)
            {
                validCount++;
            }
        }

        return validCount;
    }
}