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
    float interactionTime = 8.0f;
    bool firstRotateGoDistination = false;
    bool firstMoveGoDistination = false;
    bool seconRotateGoDistiantion = false;
    bool firstCallInteractionHi = true;
    string animationName;

    Animator animator = null;


    private void Start()
    {
        animator = GetComponent<Animator>();

        this.gameObject.transform.position = startPoint;
        this.gameObject.transform.rotation = Quaternion.Euler(startAngle.x, startAngle.y, startAngle.z);
        animationName = message;
    }

    void Update()
    {
        motionController();
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
}