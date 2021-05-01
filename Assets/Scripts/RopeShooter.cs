using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeShooter : MonoBehaviour
{
    public enum State {
        IDLE,
        CHARGING,
        SHOOTING,
        PLUGGED,
        RETRACTING,
    }

    public Char_Action action;
    public Rigidbody2D plug;
    public List<RopeDelta> ropes;
    public RopeDelta ropeDeltaPrefab;

    public Transform CharPlugLinePos;
    public Transform PlugLinePos;

    public State state;
    public float ropeDelta = 0.2f;
    public float totalLength = 10f;
    public float maxShootingSpeed = 20f;

    Rigidbody2D rb;
    Animator anim;
    LineRenderer lr;
    HingeJoint2D hingeJoint;
    DistanceJoint2D distJoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lr = GetComponent<LineRenderer>();
        hingeJoint = GetComponent<HingeJoint2D>();
        distJoint = GetComponent<DistanceJoint2D>();
    }

    void FixedUpdate()
    {
        State nextState = state;
        switch (state)
        {
        case State.IDLE:
            nextState = IdleUpdate();
            break;
        case State.CHARGING:
            nextState = ChargingUpdate();
            break;
        case State.SHOOTING:
            nextState = ShootingUpdate();
            break;
        case State.PLUGGED:
            nextState = PluggedUpdate();
            break;
        case State.RETRACTING:
            nextState = RetractingUpdate();
            break;
        }

        if (state != nextState)
        {
            switch (nextState)
            {
            case State.IDLE:
                IdleTransition();
                break;
            case State.CHARGING:
                ChargingTransition();
                break;
            case State.SHOOTING:
                ShootingTransition();
                break;
            case State.PLUGGED:
                PluggedTransition();
                break;
            case State.RETRACTING:
                RetractingTransition();
                break;
            }
            state = nextState;
        }
    }

    State IdleUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            return State.CHARGING;
        return State.IDLE;
    }

    State ChargingUpdate()
    {
        var aimDirection = GetAimDirection();
        plug.transform.position = action.transform.position + aimDirection * ropeDelta;
        plug.transform.right = aimDirection;

        if (Input.GetMouseButtonUp(0))
            return State.SHOOTING;
        return State.CHARGING;
    }

    State ShootingUpdate()
    {
        var lastRopePiece = ropes[ropes.Count - 1];
        if (Vector3.Distance(lastRopePiece.transform.position, action.transform.position) > ropeDelta)
        {
            if (getCurrentLength() < totalLength)
            {
                AddRopePiece(lastRopePiece.rb);
            }
            else
            {
                hingeJoint.enabled = true;
                hingeJoint.connectedBody = lastRopePiece.rb;

                distJoint.enabled = true;
                distJoint.connectedBody = plug;
                distJoint.distance = totalLength;
            }
        }

        RenderRopes();

        var plugVelocity = plug.velocity;
        if (plugVelocity.sqrMagnitude > 0.001f)
            plug.transform.right = plugVelocity.normalized;

        if (Physics2D.OverlapCircle(plug.position, 0.1f, action.groundlayer)) {
            return State.PLUGGED;
        }
        return State.SHOOTING;
    }
    
    State PluggedUpdate()
    {
        var lastRopePiece = ropes[ropes.Count - 1];
        if (Vector3.Distance(lastRopePiece.transform.position, action.transform.position) > ropeDelta)
        {
            if (getCurrentLength() < totalLength)
            {
                AddRopePiece(lastRopePiece.rb);
            }
            else
            {
                hingeJoint.enabled = true;
                hingeJoint.connectedBody = lastRopePiece.rb;

                distJoint.enabled = true;
                distJoint.connectedBody = plug;
                distJoint.distance = totalLength;
            }
        }

        RenderRopes();

        return State.PLUGGED;
    }

    State RetractingUpdate()
    {
        return State.RETRACTING;
    }

    void IdleTransition()
    {
        lr.enabled = false;
        plug.gameObject.SetActive(false);
        foreach (var rope in ropes)
            Destroy(rope.gameObject);
        ropes.Clear();
    }

    void ChargingTransition()
    {
        var aimDirection = GetAimDirection();

        plug.gameObject.SetActive(true);
        plug.isKinematic = true;
        plug.transform.position = action.transform.position + aimDirection * ropeDelta;
        plug.transform.right = aimDirection;
    }

    void ShootingTransition()
    {
        var aimDirection = GetAimDirection();

        plug.isKinematic = false;
        plug.transform.position = action.transform.position + aimDirection * ropeDelta;
        plug.AddForce(maxShootingSpeed * aimDirection, ForceMode2D.Impulse);

        AddRopePiece(plug);
    }

    void PluggedTransition()
    {
        plug.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void RetractingTransition()
    {

    }

    Vector3 GetAimDirection()
    {
        var mouseScreenPosition = Input.mousePosition;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        var aimDireciton = mouseWorldPosition - action.transform.position;
        aimDireciton.z = 0;
        aimDireciton.Normalize();
        return aimDireciton;
    }

    void RenderRopes()
    {
        lr.enabled = true;
        int idx = 0;
        lr.positionCount = ropes.Count + 2;
        lr.SetPosition(idx++, new Vector3(PlugLinePos.position.x, PlugLinePos.position.y, 0f));
        foreach (var rope in ropes)
            lr.SetPosition(idx++, new Vector3(rope.transform.position.x, rope.transform.position.y, 0f));
        lr.SetPosition(idx, new Vector3(CharPlugLinePos.position.x, CharPlugLinePos.position.y, 0f));
    }

    RopeDelta AddRopePiece(Rigidbody2D attachedTo)
    {
        var ropePiece = Instantiate<RopeDelta>(ropeDeltaPrefab);
        var initialPosition = action.transform.position;
        var targetPosition = attachedTo.position;
        ropePiece.transform.position = initialPosition;
        ropePiece.transform.right = (targetPosition - (Vector2)initialPosition).normalized;
        ropePiece.Initialize(ropeDelta, attachedTo);
        ropes.Add(ropePiece);
        return ropePiece;
    }

    float getCurrentLength()
    {
        return ropes.Count * ropeDelta;
    }
}
