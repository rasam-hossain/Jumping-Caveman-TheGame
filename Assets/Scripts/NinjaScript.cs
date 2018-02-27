using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class NinjaScript : MonoBehaviour
{
    public GameObject realTrailObject;         // real trail game object
                                               
    [Tooltip("This variable is set from MainScript - ignore changing it here")]
    public bool magnetismOn;
    [Tooltip("This variable is set from MainScript - ignore changing it here")]
    public bool trajectoryAssistanceOn;

    private rayCastLogic ray;
    AudioSource gameSounds;
    public AudioClip audioJump;
    public AudioClip audioFall;
    public AudioClip gameOverVoice;
    Animator animator;

    string GameSupportLevel;

    private GameObject canvasObject;
    private GameObject[] scoreObjects  = null;
    public static int score = 0;
    private static int highScore = 0;
    private bool showHighScore = false;
    public float timerPaused;

    string currentPoleColl = null;                      // the pole number where the player is right now
    public static int poleNumber = 0;

    // Setting pole variables for current and previous poles
    private float currentPoleCollX = 0;                 // setting the currentPoleX
    public static float successivePoleX = 0;            // player last succed in this pole
    public static float successivePoleY = 0;            // player last succed in this pole
    public static string successivePoleName = "";
    private float prevPoleCollX = 0;
    private double rangeOfParabola = 0;                 // distance calculated for the player to fall down
    private double requiredRange = 0;
    //private double requiredDistance = 0;
    private float playerInitialXPosition;
    private float playerInitialYPosition;

    // Assistance and Hindrance related variables
    private float assistOrHindLevel = 0;
    private float assistanceLevel = 0;
    private float hindranceLevel = 0;


    private float distanceOfNearestPoleXPosition = 0;
    private string nearestPoleName = "";
    private double maxHeightOfParabola = 0;
    private float nearestPoleXPosition = 0;
    private float nearestPoleYPosition = 0;
    private GameObject[] poleMagObjects;
    public static bool GameOverCanvasReset;

    // Timer settings
    public static float remainingTime;
    void Start()
    {
        // Setting the audio Sources
        gameSounds = GetComponent<AudioSource>();

        // Animation Stuffs
        animator = transform.GetComponentInChildren<Animator>();
        
        // Initially setting all the animation to false
        animator.SetBool("doJump", false);
        animator.SetBool("doIdle", false);
        animator.SetBool("doRun", false);
    }


    void FixedUpdate()
    { 
        if (animator.GetBool("doRun"))
        {
            CameraTracksPlayer.playerOnLastPole = true;

            // Make the player run to the x - axis and to the right direction
            Vector2 pos = transform.position;
            pos.x += Time.deltaTime * 4;
            transform.position = pos;
        }
    }

    void Jump(float jumpForce)
    {
        // Get the initial positions of the player
        GameObject player = GameObject.Find("playerPrefab(Clone)");
        playerInitialXPosition = player.transform.position.x;
        playerInitialYPosition = player.transform.position.y;
        


        // Set the animator to jump mode 
        animator.SetBool("doJump", true);
        animator.SetBool("doIdle", false);
        animator.SetBool("doRun", false);

        // Play the Jump sound
        gameSounds.PlayOneShot(audioJump, 0.5F);

        // Adding a vertical force to make the ninja jump and play the audio
        float verticalVelocity = jumpForce * 1.25f;
        float horizontalVelocity =  3.75F;
    

        // Depreciated for the 2nd Experiemnt (CHIPlay 2017) - This is where the actual player gets the velocity
        // Same code is written at the end of this function
        //Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        //if (playerTrailVisible)
        //{
        //    GetComponentInChildren<TrailRenderer>().enabled = true;
        //    rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        //}
        //else
        //{
        //    GetComponentInChildren<TrailRenderer>().enabled = false;
        //    rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        //}



        // Calculating the theta to find out the angle of the parabola of the player's direction and the actual velocity
        double thetaRad = Math.Atan(verticalVelocity / horizontalVelocity);
        var velocity = horizontalVelocity / Math.Cos(thetaRad);                         // The real input velocity

        rangeOfParabola = ((velocity * velocity) * Math.Sin(2 * thetaRad)) / 9.81;      // Math to calculate the next player falling position
        rangeOfParabola = playerInitialXPosition + rangeOfParabola;                     // The input Range


        // Calculating the maximum height
        maxHeightOfParabola = (Math.Pow(velocity,2) * Math.Pow(Math.Sin(thetaRad),2)) / (9.81 * 2);
        maxHeightOfParabola = maxHeightOfParabola + playerInitialYPosition;

        // Set the magnetism effect of the poles reading from the file
        // Load the text file using Reources.Load
        GameObject gameEngine = GameObject.Find("GameEngine");
        MainScript mainScript = gameEngine.GetComponent("MainScript") as MainScript;
        TextAsset textFile = Resources.Load(mainScript.fileToLoad) as TextAsset;
        string fs = textFile.text;
        string[] textFileLines = Regex.Split(fs, "\r\n");


        var columnX = new List<string>();                // for first column of poleX position
        var columnMagnetismLevel = new List<string>();   // This is for the magnetism column
        var columnAssistanceLevel = new List<string>();   // This is for the asssitance/hindrance column
        for (int i = 0; i < textFileLines.Length; i++)
        {
            string valueLine = textFileLines[i];
            string[] values = Regex.Split(valueLine, ",");
            columnX.Add(values[0]);
            columnMagnetismLevel.Add(values[2]);
            if (mainScript.trajectoryAssistanceOn == true && mainScript.assistanceType == MainScript.TypeOfAssitance.assistanceFromConfig)
                columnAssistanceLevel.Add(values[6]);
        }


        // Testing for the distance between the pole x positions and the player fall down x position and compare
        double lowest = 0;          // this is where the lowest value will be set and used later
        double preLowest = 99;      // this is a temporary high preLow value set to 99 to an impossilby high number
        double newLowest = 0;       // this is the calculated new distance 

        int counter = -1;
        int finalCounter = 0;
        var newLowestPosOrNeg = 0.0F; // A special variable to find out whether the anticipated pole is back or forward of the player
        foreach (var poleXposition in columnX)
        {
            counter += 1;
            // Checking the distance between magnetismVal and rangeofparabola
            newLowest = Math.Abs(float.Parse(poleXposition.ToString()) - rangeOfParabola);
            if (newLowest < preLowest)
            {
                lowest = newLowest;
                newLowestPosOrNeg = (float)(float.Parse(poleXposition.ToString()) - rangeOfParabola);
                preLowest = newLowest;
                finalCounter = counter;
            }
            else continue;
        }


        
        // Find the nearest Pole Number for the logger method
        if (finalCounter < 10) nearestPoleName = "Pole" + "0" + finalCounter;
        else nearestPoleName = "Pole" + finalCounter;

        // Set the distance between the nearest pole X position - where the player falls
        distanceOfNearestPoleXPosition = (float)lowest;


        // Now set the pole magnetism turned on only for one pole that is highly possible for the player to jump onto - but skip the one he was previously on
        GameObject[] poleMagObjects;
        if (mainScript.magnetismOn) poleMagObjects = GameObject.FindGameObjectsWithTag("PoleMagnetism");
        else if (mainScript.trajectoryAssistanceOn) poleMagObjects = GameObject.FindGameObjectsWithTag("Pole");
        else poleMagObjects = null;
        counter = 0;
        foreach (GameObject pole in poleMagObjects)
        {
            if(float.Parse(currentPoleColl.Substring(currentPoleColl.Length - 2)) == finalCounter) // find the exact pole number by getting last two digits
            {
                break;
            }

            if (counter == finalCounter)            // turn on assistance only on the pole the player is go
            {
                nearestPoleXPosition = pole.transform.position.x;
                nearestPoleYPosition = pole.transform.position.y;

                // To turn on Trajectory Assistance you must turn on both Trajectory Assistance and any other assistance method in the inspector
                if (mainScript.trajectoryAssistanceOn && mainScript.assistanceType == MainScript.TypeOfAssitance.assistanceFromConfig)
                {
                    assistOrHindLevel = float.Parse(columnAssistanceLevel[counter]);
                    if (assistOrHindLevel > 0) assistanceLevel = assistOrHindLevel / 100F;
                    else if (assistOrHindLevel < 0) hindranceLevel = Math.Abs(assistOrHindLevel / 100F);
                }
                // Random Threshold Method (Trajectory Asssitance must be on too !) 
                else if(mainScript.trajectoryAssistanceOn && mainScript.assistanceType == MainScript.TypeOfAssitance.randomThresholdMethod)
                {
                    
                    System.Random rnd = new System.Random();
                    GameSupportLevel = PlayerPrefs.GetString("GameSupport");

                    switch (GameSupportLevel)
                    {
                        case "High Assistance":
                            assistanceLevel = rnd.Next(70, 80);
                            break;
                        case "Low Assistance":
                            assistanceLevel = rnd.Next(30, 40);
                            break;
                        case "Neutral":
                            assistanceLevel = rnd.Next(0, 0);
                            break;
                        case "Low Hindrance":
                            hindranceLevel = rnd.Next(50, 60);
                            break;
                        case "High Hindrance":
                            hindranceLevel = rnd.Next(80, 120);
                            break;
                        default:
                            break;
                    }

                    if (assistanceLevel != 0)
                    {
                        assistanceLevel = assistanceLevel / 100F;
                    }
                    else if (hindranceLevel != 0)
                    {
                        hindranceLevel = hindranceLevel / 100F;
                    }
                }

                // Calculated Assistance or Hindrance
                if (assistOrHindLevel > 0) hindranceLevel = 0;
                else if (assistOrHindLevel < 0) assistanceLevel = 0;

                //Debug.Log("Assistance Level: " + assistanceLevel);
                //Debug.Log("Hindrance Level: " + hindranceLevel);

                // First criteria is to see what type of magnetism area has to be implemented
                // Important Reminder: Set the condition of MainScript.Magnetism if this section is ever Un-Commented

                //var rad = pole.GetComponentInChildren<CircleCollider2D>().radius;
                //if (mainScript.type == MainScript.magnetismArea.low)
                //    rad = rad * 0.50F;
                //else if (mainScript.type == MainScript.magnetismArea.medium)
                //    rad = rad * 0.75F;
                //else rad = rad * 1.0F;

                //pole.GetComponentInChildren<CircleCollider2D>().radius = rad;   // assign the calculated radius to the effective area

                // Turn on the magnetism only on that specific pole and check if the distance is less than 1.0 unity unit
                // newLowestPosOrNeg -> this guy checks for preceeding/posterior value in positive or negative
                // the nested loop checks if the posterior pole is really near then forget about magnetism
                //Debug.Log("newLowestPosOrNeg: " + newLowestPosOrNeg);


                //-- This Section is for implementing Magnetism
                if (magnetismOn == true)
                {
                    if (newLowestPosOrNeg < 1.7F && newLowestPosOrNeg > -3.4F) // checks for the later poles for less than 2.0F distance
                    {
                        {
                            if (newLowestPosOrNeg < 0.5F && newLowestPosOrNeg > 0.0F) break; // checks for later poles that is just within 0 - 0.5F distance
                            pole.GetComponentInChildren<PointEffector2D>().enabled = true;
                        }
                    }

                    else break;
                }

            }
            // No magnetism needed for other poles
            else
            {
                if (mainScript.magnetismOn)
                    pole.GetComponentInChildren<PointEffector2D>().enabled = false;
            }
            counter++;
        }


        // The velocity is given here
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        float requiredVelocity = 0.0F;
        // UPDATE : Implementing Required Velocity
        float playerOffset = 5.75F;

        // Calculating the X distance and Y Distance from the next possible Pole
        float deltaX = (float)(Math.Abs(playerInitialXPosition - nearestPoleXPosition));
        float deltaY = (float)(Math.Abs(playerInitialYPosition - nearestPoleYPosition) - playerOffset);

        //Debug.Log("Delta Y" + deltaY);

        // Final Calcualtion for the Required Velocity to reach the next pole
        float gravity = -Physics.gravity.y;
        requiredVelocity = (float)(0.5 * gravity * deltaX * deltaX);
        requiredVelocity /= (float)(deltaX * Math.Tan(thetaRad) + deltaY);
        requiredVelocity = (float)(Math.Sqrt(requiredVelocity));
        requiredVelocity *= (float)(1 / Math.Cos(thetaRad));

        if (trajectoryAssistanceOn && requiredVelocity != 0.0F)
        {
            float inputVelocity = horizontalVelocity / (float)(Math.Cos(thetaRad));
            float diff_bet_velocity = Math.Abs(requiredVelocity - inputVelocity);

            // For the Assistance Case
            if(GameSupportLevel.Equals("High Assistance") || GameSupportLevel.Equals("Low Assistance") || GameSupportLevel.Equals("Neutral") && assistanceLevel != 0)
            {
                if (diff_bet_velocity <= 0.25F) ; //Debug.Log("No Assistance is provided"); // do nothing 
                else if (requiredVelocity > inputVelocity)
                {
                    inputVelocity += diff_bet_velocity * assistanceLevel;
                }
                else if (requiredVelocity < inputVelocity)
                {
                    inputVelocity -= diff_bet_velocity * assistanceLevel;
                }
            }

            // For the Hindrance Case
            else if (GameSupportLevel.Equals("High Hindrance") || GameSupportLevel.Equals("Low Hindrance") && hindranceLevel != 0)
            {
                // The player was already supposed to lose
                if (diff_bet_velocity >= 1.00F)
                {
                    //Do Nothing
                }
                // Player might succeed - drag him to somewhere else - Small Range
                else if (diff_bet_velocity <= 0.25F && diff_bet_velocity > 0.15F)
                {
                    if (requiredVelocity > inputVelocity)
                    {
                        inputVelocity -= diff_bet_velocity * hindranceLevel * 3;
                    }
                    else if (requiredVelocity < inputVelocity)
                    {
                        inputVelocity += diff_bet_velocity * hindranceLevel * 3;
                    }
                }
                // Player is sure to succeed - drag him to somewhere else 
                // Extremely Small Range
                else if (diff_bet_velocity <= 0.15F)
                {
                    if (requiredVelocity > inputVelocity)
                    {
                        inputVelocity -= diff_bet_velocity * hindranceLevel * 6;
                    }
                    else if (requiredVelocity < inputVelocity)
                    {
                        inputVelocity += diff_bet_velocity * hindranceLevel * 6;
                    }
                }
                else if (requiredVelocity > inputVelocity)
                {
                    inputVelocity -= diff_bet_velocity * hindranceLevel;
                }
                else if (requiredVelocity < inputVelocity)
                {
                    inputVelocity += diff_bet_velocity * hindranceLevel;
                }
            }

            // Implementing the newly calculated the horizontal and verical velocity
            // Vx
            float requiredHorizontalVelocity = (float)(Math.Cos(thetaRad) * inputVelocity);
            //Vy
            float requiredVerticalVelocity = (float)(Math.Sin(thetaRad) * inputVelocity);

            //Debug.Log("Trajectory On Only");
            if (deltaX < 1) rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
            else rigidbody.velocity = new Vector2(requiredHorizontalVelocity, requiredVerticalVelocity);
        }
        else
        {
            //Debug.Log("Magnetism On Only");
            rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }

        // Rigidbody2D for the RealTrailClone at Participant's given velocity
        realTrailObject = GameObject.Find("realTrail(Clone)");
        Rigidbody2D rigidbodyOfRealTrail = realTrailObject.GetComponent<Rigidbody2D>();
        rigidbodyOfRealTrail.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(float.Parse(rangeOfParabola.ToString()), float.Parse(playerInitialYPosition.ToString()), 1), 0.5F);
        Gizmos.DrawWireSphere(new Vector3((float.Parse((rangeOfParabola).ToString()) + float.Parse(playerInitialXPosition.ToString()))/2, float.Parse(maxHeightOfParabola.ToString()), 1), 0.5F);
    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        GameObject loggerObject = GameObject.Find("Logger");
        LoggerScript logger = loggerObject.GetComponent<LoggerScript>();
        float poleDistance_fl = 0;
        float poleHeight = 0;
        string currentPoleMagnetism = null;
        string currentPoleFriction = null;
        string currentPoleMagnetismArea = null;
        if (collider.gameObject.tag == "Pole" && collider.gameObject.name != "PoleEnd")
        {
            // Play the animation for the player
            animator.SetBool("doJump", false);
            animator.SetBool("doIdle", true);
            animator.SetBool("doRun", false);

            // Get the current Pole name with which the pole get collided
            currentPoleColl = collider.gameObject.name;     // Name of the pole the player just got hit


            // if the player is on the first pole
            if (currentPoleColl == "Pole00")
            {
                currentPoleCollX = collider.gameObject.transform.position.x;
                //Debug.Log("For Pole 01: " + currentPoleCollX);
                prevPoleCollX = currentPoleCollX;       // after player hits the first pole we set the previous pole to Pole00
                poleDistance_fl = 0;
                GameOverCanvasReset = true; // Resetting GameOverCanvas back to false
                //Debug.Log("Is Game Over Canvas Reset: " + GameOverCanvasReset);
                successivePoleX = currentPoleCollX;
                successivePoleY = collider.gameObject.transform.position.y;
            }
            
            // but if the collder is other than the first pole then 
            else
            {
                currentPoleCollX = collider.gameObject.transform.position.x;    // current pole X is the current collider's X position
                poleDistance_fl = currentPoleCollX - prevPoleCollX;             // pole distance calculation
                prevPoleCollX = currentPoleCollX;                               // setting the current pole to prev pole again
            }

            poleHeight = collider.gameObject.transform.position.y;
            currentPoleMagnetism = collider.gameObject.GetComponentInChildren<PointEffector2D>().forceMagnitude.ToString();
            currentPoleFriction = collider.gameObject.GetComponent<BoxCollider2D>().sharedMaterial.friction.ToString();
            currentPoleMagnetismArea = collider.gameObject.GetComponentInChildren<CircleCollider2D>().radius.ToString();

            if (currentPoleMagnetismArea == "6") currentPoleMagnetismArea = "H";
            else if (currentPoleMagnetismArea == "3") currentPoleMagnetismArea = "L";
            else currentPoleMagnetismArea = "M";
            
            // Call the raycasting here
            var ray = GetComponent<rayCastLogic>();
            if (ray.isSpotted())
            {
                // Start a coroutine to set up small wait until the player stops
                StartCoroutine(WaitForPlayerToStop(poleDistance_fl, poleHeight, currentPoleColl, currentPoleMagnetism, currentPoleFriction, currentPoleMagnetismArea));      
            }
        }
        else if (collider.gameObject.tag == "Ground")
        {
            //print("Collsion occured with: " + collider.gameObject.tag + "Player has fall down");
            //animator.SetBool("doJump", false);
            //animator.SetBool("doIdle", false);
            animator.SetTrigger("doDeath");
            gameSounds.PlayOneShot(audioFall, 0.5F);

            MainScript.playerIsAlive = false;


            // Log in to files
            logger.loggerMethod(0, 0, 0, "null", "null", "null", score, "null", nearestPoleName, distanceOfNearestPoleXPosition, assistanceLevel, hindranceLevel);

            // Checking if the ninja hits the ground; in this case restart the game
            //if (!audioFall.isPlaying) audioFall.Play();
            if(GameOverCanvasReset == true)
            {
                GameOverCanvasReset = false;
                //Debug.Log("Is Game Over Canvas Reset: " + GameOverCanvasReset);
                StartCoroutine(GameEnd()); // shortly wait for the unity engine to reload the scene
            }
                
        }
        else if(collider.gameObject.name == "PoleEnd")
        {
            var ray = GetComponent<rayCastLogic>();
            if (ray.isSpotted())
            {
                // Update Score
                ScoreAdded(collider.gameObject.name);
                
                // Set player to !Alive to stop further mouse button click
                MainScript.playerIsAlive = false;
                
                // Start running
                animator.SetBool("doRun", true);

                // Log 
                logger.loggerMethod(1, poleDistance_fl, poleHeight, "PoleEnd", "0", "0", score, "null", "poleEnd", distanceOfNearestPoleXPosition, assistanceLevel, hindranceLevel);
                StartCoroutine(GameComplete());
            }
        }        
       
        if (collider.gameObject.name != "PoleEnd")
        {

            // Set the pole's magnetism false when player collides with it
            poleMagObjects = GameObject.FindGameObjectsWithTag("PoleMagnetism");
            foreach (GameObject pole in poleMagObjects)
            {
                if (pole.GetComponent<PointEffector2D>().enabled) pole.GetComponent<PointEffector2D>().enabled = false;
            }
        }
    }

    public IEnumerator WaitForPlayerToStop(float poleDistance, float poleHeight, string currentPoleColl, string currentPoleMagnetism, string currentPoleFriction, string currentPoleMagnetismArea)
    {
        string currentPoleName = currentPoleColl;
        string currentPoleMag  = currentPoleMagnetism;
        string currentPoleFric = currentPoleFriction;
        string currentPoleMagAr = currentPoleMagnetismArea;

        // Setting a weird algorithm to check whether ray still can be spotted or not !
        // Here, we are setting a short amount of delay when the player has certain level of velocity 
        float playersXvelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.x;
        if (playersXvelocity > 3)
            yield return new WaitForSeconds(0.75F);
        if (playersXvelocity <= 3 && playersXvelocity > 2)
            yield return new WaitForSeconds(0.50F);
        //yield return new WaitUntil(playersXvelocity == 0);
        else if (playersXvelocity <= 2 && playersXvelocity > 1)
            yield return new WaitForSeconds(0.5F);
        else if (playersXvelocity <= 1 && playersXvelocity > 0)
            yield return new WaitForSeconds(0.25F);
        else yield return new WaitForSeconds(0.10F);


        GameObject loggerObject = GameObject.Find("Logger");
        LoggerScript logger = loggerObject.GetComponent<LoggerScript>();
        var ray = GetComponent<rayCastLogic>();
        if (ray.isSpotted())
        {
            ScoreAdded(currentPoleName);


            // Write in to Files - although the file writing procedure is on database right now
            logger.loggerMethod(1, poleDistance, poleHeight, currentPoleName, currentPoleMag, currentPoleFric, score, currentPoleMagAr, nearestPoleName, distanceOfNearestPoleXPosition, assistanceLevel, hindranceLevel);

            //Debug.Log(currentPoleName);
            // Find the Successive Pole Name here
            GameObject whatPole = GameObject.Find(currentPoleName);
            successivePoleX = whatPole.transform.position.x;
            successivePoleY = whatPole.transform.position.y;
            successivePoleName = currentPoleName;
        }

        else
        {
            logger.loggerMethod(0, poleDistance, poleHeight, currentPoleName, currentPoleMag, currentPoleFric, score, currentPoleMagAr, nearestPoleName, distanceOfNearestPoleXPosition, assistanceLevel, hindranceLevel);
        }

    }

    public void ScoreAdded(string currentPoleName)
    {
        // Get the high score object as it is not tagged as "Score"
        GameObject[] highScoreObjects = GameObject.FindGameObjectsWithTag("HighScore");

        // All the other score objects are tagged as "Score"
        GameObject[] scoreObjects = GameObject.FindGameObjectsWithTag("Score");
        if(currentPoleName == "PoleEnd" && MainScript.updateScore)
        {
            score = score + 1;
        }
        else if (currentPoleName != "Pole00" && MainScript.updateScore)
        {
            int playerJumpedOnPole = 0;
            //Debug.Log(currentPoleName);
            // Finding the current Pole int number
            if (currentPoleName.Length == 8)
                playerJumpedOnPole = int.Parse(currentPoleName.Substring(currentPoleName.Length - 3));
            else if (currentPoleName.Length <= 7)
                playerJumpedOnPole = int.Parse(currentPoleName.Substring(currentPoleName.Length - 2));
            if (playerJumpedOnPole == (poleNumber + 1))
            {
                score = score + 1;
            }
            // extra score added if the player jumps double poles or more     
            else
            {
                score += (playerJumpedOnPole - poleNumber) * 2;
            }
            poleNumber = playerJumpedOnPole;
        }
        else if(currentPoleName == "Pole00")
        {
            score = 0;
            poleNumber = 0;
        }


        // If current score is higher than the high score
        if (score > highScore)
        {
            highScore = score;
            showHighScore = true;
        }


        // Set the score values
        foreach (GameObject scoreObject in scoreObjects)
        {
            if (score < 10)
            {
                scoreObject.GetComponent<Text>().text = "00" + score.ToString();
            }

            else if (score >= 10 && score < 100)
            {
                scoreObject.GetComponent<Text>().text = "0" + score.ToString();
            }

            else if (score >= 100)
            {
                scoreObject.GetComponent<Text>().text = score.ToString();
            }
        }


        // Set the High Score Values
        foreach (GameObject highScoreObject in highScoreObjects)
        {
            if (highScore < 10)
            {
                highScoreObject.GetComponent<Text>().text = "00" + highScore.ToString();
            }

            else if (highScore >= 10 && highScore < 100)
            {
                highScoreObject.GetComponent<Text>().text = "0" + highScore.ToString();
            }

            else if (score >= 100)
            {
                highScoreObject.GetComponent<Text>().text = highScore.ToString();
            }
        }
    }

    IEnumerator GamePause(int seconds)
    {
        yield return new WaitForSeconds(5);     // wait for several seconds
    }

    IEnumerator GameComplete()
    {
        yield return new WaitForSeconds(6);     // wait for 6 seconds

        //Show the + Dialog
        canvasObject = GameObject.Find("LevelComplete");
        Instantiate(canvasObject);
        canvasObject.GetComponent<Canvas>().enabled = true;



        yield return new WaitForSeconds(5);     // wait for ten seconds
        // Destroy everything in the scene
        foreach (GameObject o in UnityEngine.Object.FindObjectsOfType<GameObject>())
            Destroy(o);
        Application.ExternalCall("EndScene");
    }

    IEnumerator GameEnd() {
        yield return new WaitForSeconds(1);     // wait for one seconds
        
        // Enable Canvas section 
        GameObject canvasObject = GameObject.Find("Canvas");
        Instantiate(canvasObject);
        canvasObject.GetComponent<Canvas>().enabled = true;
        Image panelImage = canvasObject.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>();
        StartCoroutine(playGameAnimation(panelImage, 0.05F));
        //panelImage.color = new Color(0, 0, 0, 0.75f); // slight change in the alpha value to make the background less transparent

        // Enable Game Over Image
        GameObject gameOverObj = GameObject.Find("GameOverImage");
        gameOverObj.GetComponent<Image>().enabled = true;

        // Play the Game Over VOice Over
        gameSounds.PlayOneShot(gameOverVoice, 0.5F);

        // Disable TimeOut Image
        Text TimeOutObj = GameObject.Find("TimeOutText").GetComponent<Text>();
        TimeOutObj.enabled = false;

        // Enable the restart button
        GameObject buttonRestart = GameObject.Find("ButtonRestart");
        buttonRestart.GetComponent<Image>().enabled = true;
        buttonRestart.GetComponent<Button>().enabled = true;

        GameObject panel = GameObject.Find("Panel");
        Color colorToFadeTo;
        colorToFadeTo = new Color(100f, 100f, 100f, 0f);
        float fadeTime = 3f;
        panel.GetComponent<Image>().CrossFadeColor(colorToFadeTo, fadeTime, true, true);
        MainScript.timePaused = true;
        remainingTime = MainScript.timer; 
    }

    private IEnumerator playGameAnimation(Image panelImg, float v)
    {
        yield return new WaitForSeconds(v);
        panelImg.color = new Color(0, 0, 0, panelImg.color.a + v);
        if (panelImg.color.a != 1)
            StartCoroutine(playGameAnimation(panelImg, 0.05F));
    }
}

//public class Debug
//{
//    public static void Log(object obj)
//    {
//        UnityEngine.Debug.Log(System.DateTime.Now.ToLongTimeString() + " : " + obj);

//    }
//}
