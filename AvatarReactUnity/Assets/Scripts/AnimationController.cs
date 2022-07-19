using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private string message = "finished";
    [SerializeField]
    private float moveSpeed = 0.02f;

    public string MovePositionControllerMessage
    {
        get { return message; }
        set { message = value; }
    }

    public Vector3 startPoint;
    public Vector3 startAngle;
    public Vector3 destinationPoint;
    public Vector3 interactionTargetPoint;
    public Vector3 stanbyPoint;

    public Vector3 migratoryPoint1 = new Vector3(4, 0, 7.5f);
    public Vector3 migratoryPoint2 = new Vector3(4, 0, -8);
    public Vector3 migratoryPoint3 = new Vector3(-4, 0, -8);
    public Vector3 migratoryPoint4 = new Vector3(-4, 0, 7.5f);

    enum migratoryState
    {
        start,
        point1ToPoint2,
        point2ToPoint3,
        point3ToPoint4,
        point4ToPoint1,
    }
    migratoryState nowMigratoryState = migratoryState.start;
    bool isMigratory = false;
    bool firstCallMotionController = true;

    float rotateSpeed = 0.05f;
    float destinationDistance = 0.1f;
    float previousAngle;
    float startTime;
    float interactionTime = 8.0f;
    bool firstRotateGoDestination = false;
    bool firstMoveGoDestination = false;
    bool seconRotateGoDistiantion = false;
    bool firstCallInteractionHi = true;
    string animationName;

    private Animator animator = null;

    // ------------for State and Logic Control of Interactions------------
    public enum InteractionState
    {
        Stop, Start
    }
    public InteractionState State
    {
        get; set;
    }
    private void ChangeState(InteractionState newState)
    {
        if (State != newState) State = newState;
    }

    public enum InteractionGesture
    {
        Idle, Wave, Nyan, Nico, Moe
    }
    public InteractionGesture Gesture
    {
        get; set;
    }
    private void ChangeGesture(InteractionGesture newGesture)
    {
        if (Gesture != newGesture) Gesture = newGesture;
    }
    // ------------for State and Logic Control of Interactions------------


    private void Start()
    {
        animator = GetComponent<Animator>();

        this.gameObject.transform.position = startPoint;
        this.gameObject.transform.rotation = Quaternion.Euler(startAngle.x, startAngle.y, startAngle.z);
        animationName = message;
    }

    private void Update()
    {
        if (message == "goDestination" || isMigratory)
        {
            isMigratory = true;
            nowMigratoryState = migratoryState.start;
            if (firstCallMotionController)
            {
                firstCallMotionController = false;
                firstRotateGoDestination = false;
                firstMoveGoDestination = false;
                seconRotateGoDistiantion = false;
                firstCallMotionController = false;
            }
            MovePositionController();
            AnimateUnityChan();
        }
        else
        {
            migratoryMove(migratoryPoint1, migratoryPoint2, migratoryPoint3, migratoryPoint4);
        }
    }

    private void AnimateUnityChan()
    {
        // Add more gestures here - for managing the gesture state to its corresponding methods
        if (Gesture == InteractionGesture.Wave) InteractionWave();
        if (Gesture == InteractionGesture.Nyan) InteractionNyan();
        if (Gesture == InteractionGesture.Nico) InteractionNico();
        if (Gesture == InteractionGesture.Moe) InteractionMoe();
    }


    private void MovePositionController()
    {
        if (message != animationName && animationName != "finished")
            message = animationName;
        else
            switch (message)
            {
                case "goDestination":
                    if (goDestination(this.gameObject, destinationPoint, interactionTargetPoint))
                    {
                        message = animationName = "finished";
                        ChangeState(InteractionState.Start);
                    }
                    else
                        animationName = message;
                    break;

                case "backStartPosition":
                    ChangeState(InteractionState.Stop);
                    if (goDestination(this.gameObject, startPoint, stanbyPoint))
                    {
                        firstCallMotionController = true;
                        isMigratory = false;
                        message = animationName = "finished";
                    }
                    else
                        animationName = message;
                    break;

                case "interactionHi":
                    if (interactionHi())
                    {
                        message = animationName = "finished";
                    }
                    else
                        animationName = message;
                    break;
            }
    }

    private bool goDestination(GameObject target, Vector3 to, Vector3 interactDirection)
    {
        if (firstRotateGoDestination || headDestination(target, to))
        {
            firstRotateGoDestination = true;
            if (firstMoveGoDestination || moveToDestination(target, to))
            {
                firstMoveGoDestination = true;
                if (seconRotateGoDistiantion || headDestination(target, interactDirection))
                {
                    firstRotateGoDestination = false;
                    firstMoveGoDestination = false;
                    seconRotateGoDistiantion = false;
                    animator.SetBool("isAutoWalk", false);
                    return true;
                }
            }
        }
        animator.SetBool("isAutoWalk", true);
        return false;
    }

    private bool headDestination(GameObject target, Vector3 to)
    {
        float angle = Vector3.Angle(target.transform.position - to, target.transform.forward);
        if (Mathf.Abs(previousAngle - angle) < 0.01f)
        {
            previousAngle = 0.0f;
            return true;
        }
        else
        {
            Vector3 targetDir = to - target.transform.position;
            if (targetDir != Vector3.zero)
                target.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(target.transform.forward, targetDir, rotateSpeed, 10.0F));
            else
                target.transform.rotation = Quaternion.identity;
        }
        previousAngle = angle;
        return false;
    }

    private bool moveToDestination(GameObject target, Vector3 to)
    {
        if (Vector3.Distance(target.transform.position, to) < destinationDistance)
            return true;
        else
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, to, moveSpeed);
            return false;
        }
    }

    // methods for controlling animations
    private void InteractionWave()
    {
        if (firstCallInteractionHi)
        {
            startTime = Time.time;
            firstCallInteractionHi = false;
        }

        // Changed so that it will interrupt gesture animation when person leave the interaction area
        if (Time.time - startTime < interactionTime && State == InteractionState.Start)
        {
            animator.SetBool("isAutoHi", true);
        }
        else
        {
            firstCallInteractionHi = true;
            animator.SetBool("isAutoHi", false);
            ChangeGesture(InteractionGesture.Idle);
        }
    }

    bool interactionHi()
    {
        if (firstCallInteractionHi)
        {
            startTime = Time.time;
            firstCallInteractionHi = false;
        }
        if (Time.time - startTime < interactionTime)
        {
            animator.SetBool("isAutoHi", true);
            return false;
        }
        else
        {
            firstCallInteractionHi = true;
            animator.SetBool("isAutoHi", false);
            return true;
        }
    }


    private void InteractionNyan()
    {
        //put here the animation logic like the one above in InteractionWave
        // and make sure to return to idle state after finishing animation
        Debug.Log("NyanNyan!");
        ChangeGesture(InteractionGesture.Idle);
    }
    private void InteractionNico()
    {
        Debug.Log("NicoNicoNii!");
        ChangeGesture(InteractionGesture.Idle);
    }
    private void InteractionMoe()
    {
        Debug.Log("MoeMoeKyun!");
        ChangeGesture(InteractionGesture.Idle);
    }

    void migratoryMove(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        switch (nowMigratoryState)
        {
            case migratoryState.start:
                if (goDestination(this.gameObject, point1, point2))
                    nowMigratoryState = migratoryState.point1ToPoint2;
                break;
            case migratoryState.point1ToPoint2:
                if (goDestination(this.gameObject, point2, point3))
                    nowMigratoryState = migratoryState.point2ToPoint3;
                break;
            case migratoryState.point2ToPoint3:
                if (goDestination(this.gameObject, point3, point4))
                    nowMigratoryState = migratoryState.point3ToPoint4;
                break;
            case migratoryState.point3ToPoint4:
                if (goDestination(this.gameObject, point4, point1))
                    nowMigratoryState = migratoryState.point4ToPoint1;
                break;
            case migratoryState.point4ToPoint1:
                if (goDestination(this.gameObject, point1, point2))
                    nowMigratoryState = migratoryState.point1ToPoint2;
                break;
        }
    }
}