using UnityEngine;
using System.Collections;

public class dontDestroyOnLoadCamera : MonoBehaviour {

    public static dontDestroyOnLoadCamera instance;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this && instance != null)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);

        DontDestroyOnLoad(transform.gameObject);
    }

    public static dontDestroyOnLoadCamera Instance { get { return instance; } }
}
