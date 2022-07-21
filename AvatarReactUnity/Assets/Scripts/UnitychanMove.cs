using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitychanMove : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 startAngle;
    public Vector3 distinationPoint;
    public Vector3 interactionTargetPoint;
    public Vector3 stanbyPoint;
    public string message;
    public float moveSpeed;

    float rotateSpeed = 0.05f;
    float distinationDistance = 0.1f;
    float previousAngle;
    float startTime;
<<<<<<< Updated upstream
    float interactionTime = 8.0f;
    bool firstRotateGoDistination = false;
    bool firstMoveGoDistination = false;
=======
    float interactionWaveTime = 8.0f;
    float interactionMoeTime = 8.0f;
    bool firstRotateGoDestination = false;
    bool firstMoveGoDestination = false;
>>>>>>> Stashed changes
    bool seconRotateGoDistiantion = false;
    bool firstCallInteractionWave = true;
    bool firstCallInteractionMoe = false;
    string animationName;

<<<<<<< Updated upstream
    Animator animator = null;
=======
    private Animator animator = null;

    [SerializeField]
    private GameObject anotherObj;
    UnitychanMove anotherScript;
    bool calledObject = false;

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
>>>>>>> Stashed changes


    private void Start()
    {
        animator = GetComponent<Animator>();
        anotherScript = anotherObj.GetComponent<UnitychanMove>();
        if (this.gameObject.name == "unitychan") calledObject = true;
        this.gameObject.transform.position = startPoint;
        this.gameObject.transform.rotation = Quaternion.Euler(startAngle.x, startAngle.y, startAngle.z);
        animationName = message;
    }

    void Update()
    {
<<<<<<< Updated upstream
        motionController();
=======
        if (!anotherScript.calledObject){
            calledObject = true;
            MovePositionController();
            AnimateUnityChan();
        }
>>>>>>> Stashed changes
    }

    void motionController()
    {
        if (message != animationName && animationName != "finished")
            message = animationName;
        else
            switch (message)
            {
                case "goDistination":
                    if (goDistination(this.gameObject, distinationPoint, interactionTargetPoint))
                        message = animationName = "finished";
                    else
                        animationName = message;
                    break;

                case "interactionHi":
                    if (interactionHi())
                        message = animationName = "finished";
                    else
                        animationName = message;
                    break;

                case "backStartPosition":
                    if (goDistination(this.gameObject, startPoint, stanbyPoint))
                        message = animationName = "finished";
<<<<<<< Updated upstream
=======
                        calledObject = false;
                        anotherScript.calledObject = true;
                    }
>>>>>>> Stashed changes
                    else
                        animationName = message;
                    break;

                case "":
                    break;
            }
    }

    bool goDistination(GameObject target, Vector3 to, Vector3 interactDirection)
    {
        if (firstRotateGoDistination || headDistination(target, to))
        {
            firstRotateGoDistination = true;
            if (firstMoveGoDistination || moveToDistination(target, to))
            {
                firstMoveGoDistination = true;
                if (seconRotateGoDistiantion || headDistination(target, interactDirection))
                {
                    firstRotateGoDistination = false;
                    firstMoveGoDistination = false;
                    seconRotateGoDistiantion = false;
                    animator.SetBool("isAutoWalk", false);
                    return true;
                }
            }
        }
        animator.SetBool("isAutoWalk", true);
        return false;
    }

    bool headDistination(GameObject target, Vector3 to)
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

    bool moveToDistination(GameObject target, Vector3 to)
    {
        if (Vector3.Distance(target.transform.position, to) < distinationDistance)
            return true;
        else
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, to, moveSpeed);
            return false;
        }
    }

    bool interactionHi()
    {
        if (firstCallInteractionWave)
        {
            startTime = Time.time;
            firstCallInteractionWave = false;
        }

<<<<<<< Updated upstream
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
=======
        // Changed so that it will interrupt gesture animation when person leave the interaction area
        if (Time.time - startTime < interactionWaveTime && State == InteractionState.Start)
        {
            animator.SetBool("isAutoWave", true);
        }
        else
        {
            firstCallInteractionWave = true;
            animator.SetBool("isAutoWave", false);
            ChangeGesture(InteractionGesture.Idle);
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
        // Debug.Log("MoeMoeKyun!");
        // ChangeGesture(InteractionGesture.Idle);
        if (firstCallInteractionMoe)
        {
            startTime = Time.time;
            firstCallInteractionMoe = false;
        }

        // Changed so that it will interrupt gesture animation when person leave the interaction area
        if (Time.time - startTime < interactionMoeTime && State == InteractionState.Start)
        {
            animator.SetBool("isAutoMoe", true);
        }
        else
        {
            firstCallInteractionMoe = true;
            animator.SetBool("isAutoMoe", false);
            ChangeGesture(InteractionGesture.Idle);
        }
    }
>>>>>>> Stashed changes
}