using UnityEngine;
using System.Collections;
using System;

public class realTrailRendererScript : MonoBehaviour {

    private TrailRenderer trail;
    string playerScoreWithoutMagnetism = "";
    string playerScoreWithoutAssistance = "";
    private NinjaScript ninjaScript;
    private LoggerScript logger;
    private MainScript mainScript;
    private GameObject gameEngine;
    private GameObject loggerObject;


    // Use this for initialization
    void Start()
    {
        gameEngine = GameObject.Find("GameEngine");
        mainScript = gameEngine.GetComponent("MainScript") as MainScript;
        loggerObject = GameObject.Find("Logger");
        logger = loggerObject.GetComponent<LoggerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject RealTrail = GameObject.FindGameObjectWithTag("RealTrail");
        //if (RealTrail.GetComponent<Rigidbody2D>().velocity.y == 0)
        //    trail.time = 0;
        //else trail.time = 99;


        var ray = GetComponent<rayCastLogic>();
        if (ray.isSpotted())
        {
            RayIsSpotted();
        }
        else
        {
            if(mainScript.TrajectoryAssistanceOn)
            {
                playerScoreWithoutAssistance = "0";
                logger.loggerMethodFromRealTrail(playerScoreWithoutAssistance);
            }
            else if (mainScript.MagnetismOn)
            {
                playerScoreWithoutMagnetism = "0";
                logger.loggerMethodFromRealTrail(playerScoreWithoutMagnetism);
            }
            else
            {
                playerScoreWithoutAssistance = "null";
                playerScoreWithoutMagnetism = "null";
            }
        }
    }

    private void RayIsSpotted()
    {

        // Check whenever Ray Left and Ray Right become unspotted
        var ray = GetComponent<rayCastLogic>();
        if ( ray.spottedLeft == true || ray.spottedRight == true)
        {
            if (mainScript.TrajectoryAssistanceOn)
            {
                playerScoreWithoutAssistance = "1";
                logger.loggerMethodFromRealTrail(playerScoreWithoutAssistance);
            }
            else if(mainScript.MagnetismOn)
            {
                playerScoreWithoutMagnetism = "1";
                logger.loggerMethodFromRealTrail(playerScoreWithoutMagnetism);
            }
            else
                logger.loggerMethodFromRealTrail("null");
        }
    }
}
