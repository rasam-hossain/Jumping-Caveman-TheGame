using UnityEngine;
using System.Collections;

public class StartScreenScript : MonoBehaviour {

    public float duration = 5;
    public float smooth = 0.02F;
    Color colorStart, colorEnd;
    Color currentColor;
    static bool sawOnce = false;
    public static int gameInitialised = 999;

	// Use this for initialization
	void Start () {
        currentColor = GetComponent<SpriteRenderer>().color;

        colorStart = GetComponent<SpriteRenderer>().color;
        colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0.0F);

        if (!sawOnce)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            gameInitialised = 1;
        }
        sawOnce = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            StartCoroutine("LerpColor");
            gameInitialised = 0;
        }
	}
    IEnumerator LerpColor()
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smooth/duration; //The amount of change to apply.
        while (progress < 1)
        {
            currentColor = Color.Lerp(colorStart, colorEnd, progress);
            GetComponent<SpriteRenderer>().color = currentColor;
            progress += increment;
            yield return new WaitForSeconds(smooth);
        }
        yield return true;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
