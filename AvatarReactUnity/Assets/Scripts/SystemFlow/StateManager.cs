using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    Start,
    Greeting,
    TsunderadorFirst,
    TsunderadorSecond,
    Cheki,
    Finish
};

public class StateManager
{
    State state = State.Start;
    public StateManager(string state = "Start")
    {
        if (state == "Start") this.state = State.Start;
        else if (state == "Greeting") this.state = State.Greeting;
        else if (state == "TsunderadorFirst") this.state = State.TsunderadorFirst;
        else if (state == "TsunderadorSecond") this.state = State.TsunderadorSecond;
        else if (state == "Cheki") this.state = State.Cheki;
        else if (state == "Finish") this.state = State.Finish;
    }

    public void nextState()
    {
        if (this.state == State.Start) this.state = State.Greeting;
        else if (this.state == State.Greeting) this.state = State.TsunderadorFirst;
        else if (this.state == State.TsunderadorFirst) this.state = State.TsunderadorSecond;
        else if (this.state == State.TsunderadorSecond) this.state = State.Cheki;
        else if (this.state == State.Cheki) this.state = State.Finish;
        else if (this.state == State.Finish) this.state = State.Start;
    }

    public string getState()
    {
        string stateString = "";
        if (this.state == State.Start) stateString = "Start";
        else if (this.state == State.Greeting) stateString = "Greeting";
        else if (this.state == State.TsunderadorFirst) stateString = "TsunderadorFirst";
        else if (this.state == State.TsunderadorSecond) stateString = "TsunderadorSecond";
        else if (this.state == State.Cheki) stateString = "Cheki";
        else if (this.state == State.Finish) stateString = "Finish";
        return stateString;
    }
}
