using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanLib.References;
using UnityEngine;

public class ShotgunPelletMovement : ReferenceResolvedBehaviour
{
    public Vector2 Velocity { get; set; }

    [BindComponent] private Rigidbody2D rb;

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Velocity);
    }
}
