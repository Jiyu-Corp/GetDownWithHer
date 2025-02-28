using UnityEngine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Tasks.Actions;
using CleverCrow.Fluid.BTs.Trees;

public class KnightAi : MonoBehaviour
{
    [SerializeField]
    private BehaviorTree _tree;

    private void Awake()
    {
        _tree = new BehaviorTreeBuilder(gameObject)
            .Selector()
                .Sequence()
                    .Condition("Is princess in range" ,() => IsPrincessInRange())
                    .Do("Capture Princess", () => CapturePrincess())
                .End()
                .Do("Move to Princess" ,() => MoveToPrincess())
            .End()
        .Build();
    }

    private void Update()
    {
        _tree.Tick();
    }

    private bool IsPrincessInRange() {
        var princess = GameObject.FindWithTag("Princess");
        var distance = Vector2.Distance(transform.position, princess.transform.position);
        Debug.DrawLine(transform.position, princess.transform.position, Color.red);
        return distance < 1;
    }

    private TaskStatus MoveToPrincess() {
        var princess = GameObject.FindWithTag("Princess");
        var direction = (princess.transform.position - transform.position).normalized;
        Debug.DrawLine(transform.position, princess.transform.position, Color.green);
        transform.position += direction * Time.deltaTime;
        return TaskStatus.Success;
    }

    private TaskStatus CapturePrincess() {
        Debug.Log("I WON");
        return TaskStatus.Success;
    }
}
