using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunderadarMove : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.02f;
    public string message;
    public Vector3 startPoint;
    public Vector3 startAngle;
    public Vector3 DestinationPoint;
    public Vector3 interactionTargetPoint;
    public Vector3 stanbyPoint;

    private Animator animator = null;
    bool firstRotateGoDestination = false;
    bool firstMoveGoDestination = false;
    bool secondRotateGoDistination = false;
    float rotateSpeed = 0.05f;
    float DestinationDistance = 0.1f;
    float previousAngle;
    float startTime;

    enum TsundereState {
        Stanby,
        Start,
        FirstStep,
        SecondStep,
        Finish,
    };
    TsundereState state;
    void nextState() {
        if (state == TsundereState.Stanby) state = TsundereState.Start;
        else if (state == TsundereState.Start) state = TsundereState.FirstStep;
        else if (state == TsundereState.FirstStep) state = TsundereState.SecondStep;
        else if (state == TsundereState.SecondStep) state = TsundereState.Finish;
        else if (state == TsundereState.Finish) state = TsundereState.Stanby;
    }

    string keyword;
    string stateMessage;
    public bool isFinish;
    bool isfirstCalFirstInteraction;
    bool isfirstCalSecondInteraction;
    float firstStepTime = 6.0f;
    float secondStepTime = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        this.gameObject.transform.position = startPoint;
        this.gameObject.transform.rotation = Quaternion.Euler(startAngle.x, startAngle.y, startAngle.z);
        state = TsundereState.Stanby;
        keyword = "state1";
        isFinish = true;
        isfirstCalFirstInteraction = true;
        isfirstCalSecondInteraction = true;
    }

    // Update is called once per frame
    void Update()
    {
        stateMessage = GetComponent<RecAudio>().InputState;
        if (isFinish && stateMessage == keyword)
        {
            nextState();
            isFinish = false;
        }

        if (state == TsundereState.Start && !isFinish)
        {
            if (goDestination(this.gameObject, DestinationPoint, interactionTargetPoint))
            {
                message = "finished";
                keyword = "state2";
                isFinish = true;
            }
            else isFinish = false;
        }
        else if (state == TsundereState.FirstStep && !isFinish)
        {
            if (firstInteraction())
            {
                message = "finished";
                keyword = "state3";
                isFinish = true;
            }
            else isFinish = false;
        }
        else if (state == TsundereState.SecondStep && !isFinish)
        {
            if (secondInteraction())
            {
                message = "finished";
                keyword = "state4";
                isFinish = true;
            }
            else isFinish = false;
        }
        else if (state == TsundereState.Finish && !isFinish)
        {
            if (goDestination(this.gameObject, startPoint, stanbyPoint))      
            {
                message = "finished";
                keyword = "state1";
                isFinish = true;
                nextState();
            }
            else isFinish = false;
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
                if (secondRotateGoDistination || headDestination(target, interactDirection))
                {
                    firstRotateGoDestination = false;
                    firstMoveGoDestination = false;
                    secondRotateGoDistination = false;
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
        if (Vector3.Distance(target.transform.position, to) < DestinationDistance)
            return true;
        else
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, to, moveSpeed);
            return false;
        }
    }

    private bool firstInteraction()
    {
        if (isfirstCalFirstInteraction)
        {
            startTime = Time.time;
            isfirstCalFirstInteraction = false;
        }

        if (Time.time - startTime < firstStepTime)
        {
            animator.SetBool("isFirstStep", true);
            return false;
        }
        else
        {
            isfirstCalFirstInteraction = true;
            animator.SetBool("isFirstStep", false);
            return true;
        }
    }
    private bool secondInteraction()
    {
        if (isfirstCalSecondInteraction)
        {
            startTime = Time.time;
            isfirstCalSecondInteraction = false;
        }

        if (Time.time - startTime < secondStepTime)
        {
            animator.SetBool("isSecondStep", true);
            return false;
        }
        else
        {
            isfirstCalSecondInteraction = true;
            animator.SetBool("isSecondStep", false);
            return true;
        }
    }

}
