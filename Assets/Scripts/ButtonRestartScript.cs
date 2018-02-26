using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonRestartScript : MonoBehaviour
{
    public float timerPaused;

    public void waitForSeconds(int sec)
    {
        
        StartCoroutine(MyMethod(sec));
    }

    IEnumerator MyMethod(int numOfsec)
    {
        yield return new WaitForSeconds(numOfsec);     // wait for two seconds
    }
}
