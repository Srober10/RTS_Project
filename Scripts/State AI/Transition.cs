using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition {
    public Decision decision; //decision we evaluate on 
    public State trueState;
    public State falseState;
}
