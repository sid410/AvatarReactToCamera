using System.Collections;
using System.Collections.Generic;
using System;
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
    bool firstRotateGoDistination = false;
    bool firstMoveGoDistination = false;
    bool seconRotateGoDistiantion = false;
    bool isArriveDistination = false;
    bool isGreetingFinished = false;
    bool isAngered = false;
    public float interactionHiTime = 6.0f;
    public float interactionGetAwayTime = 6.75f;
    public float intearactionCallOutTime = 6.25f;
    public float inteactionDownheartTime = 8.75f;
    public float interactionAngryTime = 9.0f;
    bool firstCallInteractionHi = true;
    bool firstCallInteractionGetAway = true;
    bool firstCallInteractionCallOut = true;
    bool firstCallInteractionDownheart = true;
    bool firstCallInteractionAngry = true;
    string animationName;

    Animator animator = null;

    string userDistance = ""; //far, stable, close
    string isWaving = ""; //true, false
    bool isWaved = false;

    enum State
    {
        close,
        far,
        waved,
        angry,
        navigate,
        downheart,
        getAway,
        callOut,
        finished,
    };
    State nowState = State.far;
    private void Start()
    {
        animator = GetComponent<Animator>();

        this.gameObject.transform.position = startPoint;
        this.gameObject.transform.rotation = Quaternion.Euler(startAngle.x, startAngle.y, startAngle.z);
    }

    void Update()
    {
        motionController();
    }

    void motionController()
    {
        if (message.Split(',').Length == 2)
        {
            userDistance = message.Split(',')[0];
            isWaving = message.Split(',')[1];
            Debug.Log(userDistance + ", " + isWaving + ", " + nowState);
            if ((userDistance == "stable" && isWaving == "true" && isGreetingFinished) || isAngered) // stable,true
                nowState = State.navigate;
            else if (userDistance == "stable" && isWaving == "false" && isGreetingFinished && !isWaved) // stable,false
                nowState = State.callOut;
            else if (userDistance == "stable" && isWaving == "false" && isGreetingFinished && isWaved) // stable,false
                nowState = State.angry;
            else if (userDistance == "far" && isAngered) // far,false / far,true
                nowState = State.downheart;
            else if (userDistance == "close" && isAngered) // close,false / close,true
                nowState = State.getAway;
            else if (userDistance == "far") // far,false / far,true
                nowState = State.downheart;
            else if (userDistance == "close") // close,false / close,true
                nowState = State.getAway;
            else if (userDistance == "stable" && isWaving == "false") // stable,false
                nowState = State.waved;
        }

        if (nowState == State.finished)
        {
            setBools("all");
        }
        else
        {
            switch (nowState)
            {
                case State.waved:
                    if (isArriveDistination || goDistination(this.gameObject, distinationPoint, interactionTargetPoint))
                    {
                        isArriveDistination = true;
                        if (interactionHi())
                        {
                            isGreetingFinished = true;
                            nowState = State.finished;
                            message = "finished";
                        }
                    }
                    break;

                case State.downheart:
                    if (interactionDownheart())
                    {
                        nowState = State.finished;
                        message = "finished";
                    }
                    break;

                case State.getAway:
                    if (interactionGetAway())
                    {
                        nowState = State.finished;
                        message = "finished";
                    }
                    break;

                case State.callOut:
                    if (interactionCallOut())
                    {
                        isWaved = true;
                        nowState = State.finished;
                        message = "finished";
                    }
                    break;

                case State.angry:
                    if (interactionAngry())
                    {
                        isAngered = true;
                        nowState = State.finished;
                        message = "finished";
                    }
                    break;

                case State.navigate:
                    setBools("isNavigate");
                    break;

                default:
                    break;
            }
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
        if (Time.time - startTime < interactionHiTime)
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

    bool interactionGetAway()
    {
        if (firstCallInteractionGetAway)
        {
            startTime = Time.time;
            firstCallInteractionGetAway = false;
        }
        if (Time.time - startTime < interactionGetAwayTime)
        {
            animator.SetBool("isAutoGetAway", true);
            return false;
        }
        else
        {
            firstCallInteractionGetAway = true;
            animator.SetBool("isGetAway", false);
            return true;
        }
    }

    bool interactionCallOut()
    {
        if (firstCallInteractionCallOut)
        {
            startTime = Time.time;
            firstCallInteractionCallOut = false;
        }
        if (Time.time - startTime < intearactionCallOutTime)
        {
            animator.SetBool("isCallOut", true);
            return false;
        }
        else
        {
            firstCallInteractionCallOut = true;
            animator.SetBool("isCallOut", false);
            return true;
        }
    }

    bool interactionAngry()
    {
        if (firstCallInteractionAngry)
        {
            startTime = Time.time;
            firstCallInteractionAngry = false;
        }
        if (Time.time - startTime < interactionAngryTime)
        {
            animator.SetBool("isAngry", true);
            return false;
        }
        else
        {
            firstCallInteractionAngry = true;
            animator.SetBool("isAngry", false);
            return true;
        }
    }

    bool interactionDownheart()
    {
        if (firstCallInteractionDownheart)
        {
            startTime = Time.time;
            firstCallInteractionDownheart = false;
        }
        if (Time.time - startTime < inteactionDownheartTime)
        {
            animator.SetBool("isDownheart", true);
            return false;
        }
        else
        {
            firstCallInteractionDownheart = true;
            animator.SetBool("isDownheart", false);
            return true;
        }
    }

    void setBools(string boolName)
    {
        switch (boolName)
        {
            case "isAutoWalk":
                animator.SetBool("isAutoWalk", true);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", false);
                break;
            case "isAutoHi":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", true);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", false);
                break;
            case "isDownheart":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", true);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", false);
                break;
            case "isAngry":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", true);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", false);
                break;
            case "isCallOut":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", true);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", false);
                break;
            case "isGetAway":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", true);
                animator.SetBool("isNavigate", false);
                break;
            case "isNavigate":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", true);
                break;
            case "all":
                animator.SetBool("isAutoWalk", false);
                animator.SetBool("isAutoHi", false);
                animator.SetBool("isDownheart", false);
                animator.SetBool("isAngry", false);
                animator.SetBool("isCallOut", false);
                animator.SetBool("isGetAway", false);
                animator.SetBool("isNavigate", false);
                break;
            default:
                break;
        }
    }
}
