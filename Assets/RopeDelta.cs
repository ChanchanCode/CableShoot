using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeDelta : MonoBehaviour
{
    static readonly float WIDTH = 0.1f;
    static readonly float WEIGHT_PER_LENGTH = 0.03f;

    public Rigidbody2D rb;

    public void Initialize(float deltaLength, Rigidbody2D connectedPlugSide)
    {
        // var collider = GetComponent<BoxCollider2D>();
        var joint = GetComponent<HingeJoint2D>();
        var distJoint = GetComponent<DistanceJoint2D>();

        // collider.size = new Vector2(deltaLength, RopeDelta.WIDTH);
        rb.mass = WEIGHT_PER_LENGTH * deltaLength;
        joint.connectedBody = connectedPlugSide;

        distJoint.connectedBody = connectedPlugSide;
        distJoint.distance = deltaLength;
    }
}
