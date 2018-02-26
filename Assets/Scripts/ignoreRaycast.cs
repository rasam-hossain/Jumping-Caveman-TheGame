using UnityEngine;
using System.Collections;

public class ignoreRaycast : MonoBehaviour {
    public LayerMask mask = -1;
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        if (Physics.Raycast(transform.position, transform.forward, 100, mask.value))
            Debug.Log("Hit something");
    }
}
