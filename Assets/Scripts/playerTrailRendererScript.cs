using UnityEngine;
using System.Collections;

public class playerTrailRendererScript : MonoBehaviour {
    private TrailRenderer trail;
    // Use this for initialization
    void Start ()
    {
        trail = this.GetComponent<TrailRenderer>();
        trail.sortingLayerName = "Background";
        //trail.sortingOrder = 6990;
    }
	
	// Update is called once per frame
	void Update () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<Rigidbody2D>().velocity.y == 0)
            trail.time = 1;
        else trail.time = 99;
	}
}
