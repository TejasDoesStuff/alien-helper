using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SolitaryBoid : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 2.5f;
    public float maxAccel = 6f;
    public float turnRate = 6f;

    [Header("Wandering")]
    public float wanderStrength = 1f;
    public float wanderChangeRate = 2f;   // seconds between wander decisions
    [Range(0f, 1f)] public float pauseChance = 0.3f; // chance to pause instead of wandering
    public Vector2 pauseDurationRange = new Vector2(2f, 5f);

    [Header("Obstacle Avoidance")]
    public float obstacleCheckDistance = 3f;
    public LayerMask obstacleMask = ~0;

    [Header("Animation")]
    public float idleSpeedThreshold = 0.05f;
    public string walkParam = "isWalking";
    public string idleParam = "isIdle";

    private Vector3 velocity;
    private Vector3 wanderDir;
    private Animator animator;

    private float nextDecisionTime;
    private bool isPaused = false;
    private float pauseEndTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        velocity = transform.forward * maxSpeed * 0.5f;
        wanderDir = transform.forward;
        nextDecisionTime = Time.time + wanderChangeRate;
    }

    void Update()
    {
        // --- Pause logic ---
        if (isPaused)
        {
            velocity = Vector3.zero;

            if (Time.time >= pauseEndTime)
            {
                isPaused = false;
                nextDecisionTime = Time.time + wanderChangeRate;
            }

            UpdateAnimation();
            return;
        }

        // --- Decision making ---
        if (Time.time >= nextDecisionTime)
        {
            if (Random.value < pauseChance)
            {
                // Enter pause
                isPaused = true;
                pauseEndTime = Time.time + Random.Range(pauseDurationRange.x, pauseDurationRange.y);
                UpdateAnimation();
                return;
            }
            else
            {
                // Pick new wander direction
                wanderDir = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * wanderDir;
                wanderDir.Normalize();
                nextDecisionTime = Time.time + wanderChangeRate;
            }
        }

        // --- Steering: wander + avoid obstacles ---
        Vector3 steer = wanderDir * wanderStrength;
        steer += AvoidObstacles() * 3f;

        steer = Vector3.ClampMagnitude(steer, maxAccel) * Time.deltaTime;

        // --- Velocity update ---
        velocity = Vector3.ClampMagnitude(velocity + steer, maxSpeed);
        velocity.y = 0f;

        // --- Move + rotate ---
        transform.position += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > 0.001f)
        {
            var face = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, face, Time.deltaTime * turnRate);
        }

        UpdateAnimation();
    }

    Vector3 AvoidObstacles()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.25f, transform.forward, out var hit, obstacleCheckDistance, obstacleMask))
        {
            Vector3 away = Vector3.ProjectOnPlane(hit.normal, Vector3.up);
            return away.normalized * maxSpeed - velocity;
        }
        return Vector3.zero;
    }

    void UpdateAnimation()
    {
        bool moving = velocity.magnitude > idleSpeedThreshold && !isPaused;
        animator.SetBool(walkParam, moving);
        animator.SetBool(idleParam, !moving);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isPaused ? Color.blue : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * obstacleCheckDistance);
    }
}
