using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    Stanby,
    Start,
    TsunderadorFirst,
    TsunderadorSecond,
    Review,
    Finish
};

public class StateManager
{
    State state = State.Start;
    public StateManager(string state = "Staby")
    {
        if (state == "Stanby") this.state = State.Stanby;
        else if (state == "Start") this.state = State.Start;
        else if (state == "TsunderadorFirst") this.state = State.TsunderadorFirst;
        else if (state == "TsunderadorSecond") this.state = State.TsunderadorSecond;
        else if (state == "Review") this.state = State.Review;
        else if (state == "Finish") this.state = State.Finish;
    }

    public void nextState()
    {
        if (this.state == State.Stanby) this.state = State.Start;
        else if (this.state == State.Start) this.state = State.TsunderadorFirst;
        else if (this.state == State.TsunderadorFirst) this.state = State.TsunderadorSecond;
        else if (this.state == State.TsunderadorSecond) this.state = State.Review;
        else if (this.state == State.Review) this.state = State.Finish;
        else if (this.state == State.Finish) this.state = State.Start;
    }

    public string getState()
    {
        string stateString = "";
        if (this.state == State.Stanby) stateString = "Stanby";
        else if (this.state == State.Start) stateString = "Start";
        else if (this.state == State.TsunderadorFirst) stateString = "TsunderadorFirst";
        else if (this.state == State.TsunderadorSecond) stateString = "TsunderadorSecond";
        else if (this.state == State.Review) stateString = "Review";
        else if (this.state == State.Finish) stateString = "Finish";
        return stateString;
    }
}
