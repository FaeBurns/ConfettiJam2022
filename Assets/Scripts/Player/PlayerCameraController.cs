using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Component responsible for keeping the player inside the camera's view.
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private float speed = 0.1f;

    private void Awake()
    {
        // don't wanna have to plug in in case of other scenes
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        // get difference between target and self
        Vector3 diff = player.transform.position - transform.position;

        // slow down movement
        diff *= speed;

        // stop movement on the Z axis
        diff.z = 0;

        // apply movement
        transform.position += diff * Time.deltaTime;
    }
}