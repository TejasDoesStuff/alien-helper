using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HerdBoid : MonoBehaviour
{
    // movement
    public float maxSpeed = 2.5f;
    public float maxAccel = 8f;            // steering
    public float turnRate = 8f;            // rotation speed

    // neighborhood
    public float neighborRadius = 15f;
    public float separationDistance = 2f;
    [Range(0f, 180f)] public float fieldOfView = 150f;
     
    // weights
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 2.0f;
    public float obstacleAvoidanceWeight = 3.0f;

    // obstacle avoidance
    public float obstacleCheckDistance = 3f;
    public LayerMask obstacleMask = ~0;

    // animation
    public float idleSpeedThreshold = 0.05f; 
    public string walkParam = "isWalking";
    public string idleParam = "isIdle";

    // optional flock filtering
    public int flockID = 0;

    // internals
    public Vector3 velocity;
    private Animator animator;

    // Static registry avoids needing colliders for neighbor search
    private static readonly List<HerdBoid> All = new List<HerdBoid>();

    void OnEnable() { if (!All.Contains(this)) All.Add(this); }
    void OnDisable() { All.Remove(this); }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (velocity.sqrMagnitude < 0.001f) velocity = transform.forward * maxSpeed * 0.5f;
    }

    void Update()
    {
        // 1. find neighbors
        var neighbors = GetNeighbors();

        // 2. figure out where to steer
        Vector3 steer = Vector3.zero;

        if (neighbors.Count > 0)
        {
            steer += Cohesion(neighbors)   * cohesionWeight;
            steer += Alignment(neighbors)  * alignmentWeight;
            steer += Separation(neighbors) * separationWeight;
        }

        steer += AvoidObstacles() * obstacleAvoidanceWeight;

        // 3. apply the steering
        steer = Vector3.ClampMagnitude(steer, maxAccel) * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity + steer, maxSpeed);

        // 4. move + rotations
        velocity.y = 0f;
        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            var face = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, face, Time.deltaTime * turnRate);
        }

        // 5. animations
        bool moving = velocity.magnitude > idleSpeedThreshold;
        animator.SetBool(walkParam, moving);
        animator.SetBool(idleParam, !moving);
    }

    // boid Stuff
    List<HerdBoid> GetNeighbors()
    {
        var list = new List<HerdBoid>();
        Vector3 pos = transform.position;
        Vector3 fwd = transform.forward;

        float r2 = neighborRadius * neighborRadius;
        float cosFOV = Mathf.Cos(fieldOfView * Mathf.Deg2Rad * 0.5f);

        for (int i = 0; i < All.Count; i++)
        {
            var n = All[i];
            if (n == this || n.flockID != this.flockID) continue;

            Vector3 to = n.transform.position - pos;
            float d2 = to.sqrMagnitude;
            if (d2 > r2 || d2 < 0.0001f) continue;

            // field of view check
            Vector3 dir = to.normalized;
            if (Vector3.Dot(fwd, dir) < cosFOV) continue;

            list.Add(n);
        }
        return list;
    }

    Vector3 Cohesion(List<HerdBoid> neighbors)
    {
        Vector3 center = Vector3.zero;
        foreach (var n in neighbors) center += n.transform.position;
        center /= neighbors.Count;
        Vector3 desired = (center - transform.position);
        return desired.normalized;
    }

    Vector3 Alignment(List<HerdBoid> neighbors)
    {
        Vector3 avgVel = Vector3.zero;
        foreach (var n in neighbors) avgVel += n.velocity;
        avgVel /= neighbors.Count;
        if (avgVel.sqrMagnitude < 0.0001f) return Vector3.zero;

        Vector3 desiredDir = avgVel.normalized;
        // steer toward neighbors with similar velocity
        Vector3 desiredVel = desiredDir * maxSpeed;
        return (desiredVel - velocity); // steering
    }

    Vector3 Separation(List<HerdBoid> neighbors)
    {
        Vector3 push = Vector3.zero;
        foreach (var n in neighbors)
        {
            Vector3 diff = transform.position - n.transform.position;
            float dist = diff.magnitude;
            if (dist < Mathf.Epsilon) continue;
            float strength = Mathf.Clamp01((separationDistance - dist) / separationDistance);
            push += diff.normalized * strength;
        }
        return push;
    }

    Vector3 AvoidObstacles()
    {
        // forward check
        if (Physics.Raycast(transform.position + Vector3.up * 0.25f, transform.forward, out var hit, obstacleCheckDistance, obstacleMask))
        {
            // steer away from obstacle normal and a bit to the side
            Vector3 away = Vector3.ProjectOnPlane(hit.normal + Vector3.Cross(Vector3.up, transform.forward).normalized * 0.5f, Vector3.up);
            return away.normalized * maxSpeed - velocity;
        }
        return Vector3.zero;
    }

    // Optional: visualize
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity.normalized * 2f);
    }
}
