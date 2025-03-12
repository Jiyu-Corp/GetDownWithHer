using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Trees;

public class KnightAi : MonoBehaviour
{
    public float speed = 2f;
    public float jumpThreshold = 1f;
    public float jumpForce = 5f;
    public float waypointTolerance = 0.5f;
    public Transform princess;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private NavigationGraphGenerator navGraphGen;
    private List<Vector2> currentPath;
    private int currentWaypointIndex;
    private Vector2 lastPrincessPosition;
    public float recalcThreshold = 1.5f;

    [SerializeField]
    private BehaviorTree _tree;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        navGraphGen = FindAnyObjectByType<NavigationGraphGenerator>();
        lastPrincessPosition = princess.position;

        _tree = new BehaviorTreeBuilder(gameObject)
            .Selector()
                .Sequence()
                    .Condition("Is princess in range" ,() => IsPrincessInRange())
                    .Do("Capture Princess", () => CapturePrincess())
                .End()
                .Sequence()
                    .Do("Calculate Path", () => CalculatePathToPrincess())
                    .Do("Follow Path", () => FollowPath())
            .End()
        .Build();
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(princess.position, lastPrincessPosition) > recalcThreshold)
        {
            CalculatePathToPrincess();
            lastPrincessPosition = princess.position;
        }

        _tree.Tick();
    }

    bool IsPrincessInRange()
    {
        float distance = Vector2.Distance(transform.position, princess.position);
        return distance < 1.0f;
    }

    TaskStatus CalculatePathToPrincess()
    {
        if (navGraphGen == null || navGraphGen.graph == null)
            return TaskStatus.Failure;
        
        currentPath = AStarPathfinder.FindPath(transform.position, princess.position, navGraphGen.graph);
        currentWaypointIndex = 0;
        return (currentPath != null && currentPath.Count > 0) ? TaskStatus.Success : TaskStatus.Failure;
    }

    TaskStatus FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            return TaskStatus.Failure;
        }
        
        if (currentWaypointIndex >= currentPath.Count)
        {
            return TaskStatus.Success;
        }

        Vector2 target = currentPath[currentWaypointIndex];
        Vector2 currentPos = transform.position;

        // Calculate horizontal difference and determine movement direction.
        float horizontalDiff = target.x - currentPos.x;
        float directionX = Mathf.Sign(horizontalDiff);

        // Set horizontal velocity while preserving the current vertical velocity.
        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);


        if (ShouldJump(target))
        {
            Jump();
        }

        // Check if we've reached the waypoint using a tolerance value.
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), target);
        if (distanceToTarget < waypointTolerance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= currentPath.Count)
            {
                return TaskStatus.Success;
            }
        }
        return TaskStatus.Continue;
    }

    bool ShouldJump(Vector2 target)
    {
        // Only consider jumping if the target is above our current position.
        if (target.y <= transform.position.y) return false;
        
        // Determine movement direction (assumes right is positive x).
        float directionX = Mathf.Sign(target.x - transform.position.x);
        
        // Define a point ahead of the knight's feet.
        Vector2 rayOrigin = new Vector2(transform.position.x + directionX * 0.5f, transform.position.y);
        
        // The raycast length can be tuned based on your game scale.
        float rayDistance = 1.0f;
        
        // Cast a ray downward from this point.
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, groundLayer);
        
        // If no ground is detected, there’s a gap.
        return hit.collider == null;
    }

    void Jump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private TaskStatus CapturePrincess() {
        return TaskStatus.Success;
    }
}
