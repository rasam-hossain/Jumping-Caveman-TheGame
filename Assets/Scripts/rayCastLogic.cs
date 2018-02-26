using UnityEngine;
using System.Collections;

public class rayCastLogic : MonoBehaviour {

    public Transform LowerLeftStart, LowerLeftEnd, LowerRightStart, LowerRightEnd;
    public bool spotted = false;
    public bool spottedLeft = false;
    public bool spottedRight = false;
    

    // Update is called once per frame
    void Update ()
    {
        isSpotted();
    }

    public bool isSpotted()
    {
        //Debug.DrawLine(LowerLeftStart.position, LowerLeftEnd.position, Color.green);
        //Debug.DrawLine(LowerRightStart.position, LowerRightEnd.position, Color.red);
        spottedLeft = Physics2D.Linecast(LowerLeftStart.position, LowerLeftEnd.position, 1 << LayerMask.NameToLayer("Pole"));
        spottedRight = Physics2D.Linecast(LowerRightStart.position, LowerRightEnd.position, 1 << LayerMask.NameToLayer("Pole"));
        if (spottedLeft || spottedRight) spotted = true;
        else spotted = false;
        return spotted;
    }
    public void Behaviors()
    {
        //print("here is behaviors");
    }
}
