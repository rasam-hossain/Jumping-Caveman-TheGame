//using DigitalRuby.WeatherMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class MainScript : MonoBehaviour
{

    // All these variables will be declared in the inspector

    [Header("Player and other properties")]
    [Tooltip("The Player Object of this game")]
    public GameObject playerObject;                 // the ninja prefab
    public GameObject realTrailObject;

    [Tooltip("The Power Bar")]
    public GameObject powerBarObject;               // the powerBar prefab

    private GameObject[] poleFrictionObjects;       // pole friction object
    private float jumpForce;                        // vertical jump force

    [Tooltip("Maximum Possible Jump Force that the player can perform")]
    [Range(0, 12)]
    public float maxJumpForce = 11;                 // max vertical jump force

    [Header("Game Assistance Type 01")]
    [Tooltip("Turn on the Magnetism?")]
    public bool MagnetismOn = false;                // magnetism?on

    [Header("Game Assistance Type 02")]
    [Tooltip("Turn on the Trajectory Assistance?")]
    public bool TrajectoryAssistanceOn = false;     // Trajectory Assistance?on

    // Difficulty Level to set up manually
    public enum TypeOfAssitance
    {
        assistanceFromConfig,
        randomThresholdMethod
    }
    [Tooltip("What is the current asssitance method?")]
    public TypeOfAssitance assistanceType = new TypeOfAssitance();

    // Difficulty Level to set up manually
    //public enum Difficulty
    //{
    //    HighTrajectory,
    //    LowTrajectory,       
    //    Neutral,
    //    Mixed,
    //    LowHindrance,
    //    HighHindrance
    //}
    [Tooltip("What is the current difficulty?")]
    //private string GameSupportLevel;

    [Header("Respawn")]
    public bool respawnPlayer;

    [Header("Input Config File")]
    [Tooltip("Which input file?")]
    public string fileToLoad = "";                  // Which input file
    public static string refFileName;     

    [Header("Pole Objects")]
    [Tooltip("The Rock-look-a-like Pole Object")]
    public GameObject poleObjectRock;           // the rock pole prefab

    [Tooltip("The Brick-look-a-like Pole Object")]
    public GameObject poleObjectBrick;          // the brick pole prefab

    [Tooltip("The Brick-look-a-like Pole Object with green grass on top")]
    public GameObject poleObjectBrickAndGrass;  // the brick and glass pole prefab

    [Tooltip("The Pipe-look-a-like Pole Object that has icy substance")]
    public GameObject poleObjectPipeAndIce;     // the pipe and ice pole prefab

    [Tooltip("The Ice-look-a-like Pole Object that has icy substance")]
    public GameObject poleObjectIce;            // the ice pole prefab

    [Tooltip("The last pole")]
    public GameObject poleEndObject;            // the last pole

    [Tooltip("The last pole starting position")]
    public GameObject startOfPoleEndObject;            // this special start position of this pole

    [Header("Daylight Settings")]
    [Range(0, 1.5F)]
    private float currentLightSettings = 1.0F;

    [Header("Timer Settings (in minutes)")]
    [Tooltip("Set the total game time here")]
    [Range(0, 15.0F)]
    private float totalGameTime;
    public static float timer;
    public static float elapsedTime = 0F;


    public static bool timeStarted = false;
    public static bool timePaused = false;

	// These variables are set only inside the script
	private bool isCharging = false;        // is the ninja charging the jump?
    public static bool playerIsAlive = true;
    bool pressedOnce = false;


    // Click this to turn the red and green trajectory by deafult
    //[Header("Game Debugger Settings")]
    //private bool DebuggerMode = false;

    // Server URL
    public string url = "";
    //public static string server_url = "http://127.0.0.1:5000";
    public static string server_url = "http://hci-mturk.usask.ca:10030";
    //public static string next_page_after_game_end = "/redirect_next_page";

    //Variables for Timer settings
    private bool calledOnce = false;
    //public static MainScript instance;
    public static bool updateScore;
    //Animator
    //Animator animator;

    // To use the redirectLink plugin
    [DllImport("__Internal")]
    public static extern void openWindow(string url);

    // This function is responsible to get executed whenever we launch the game
    void Start()
    {
        // --  Default Game Options Settings --

        //DayLight Settings
        if (PlayerPrefs.GetString("DayLight").Equals("True"))
            currentLightSettings = 0.2F;
        else
            currentLightSettings = 1.0F;

        // Total Game Time
        totalGameTime = 10.0F;

        // Player Respwan Mechanism
        if (respawnPlayer)
        {
            // player offset while 
            float playerOffset = 5.7F;

            // If game has been already initialised then respawn from the last succeeded pole
            if(StartScreenScript.gameInitialised == 0)
            {
                // Background is set according to player's current position
                GameObject BG = GameObject.Find("BG");
                BG.transform.position = new Vector2((NinjaScript.successivePoleX - (-3.41F)), 11F);

                // GameReset is called passing the poleX and poleY variables
                // The offset 5.7F is the distance between the avatar and the pole
                GameReset(NinjaScript.successivePoleX, NinjaScript.successivePoleY + playerOffset);
               
                NinjaScript.GameOverCanvasReset = true;          // Set GameCanvasReset to True
                updateScore = false;                             // Don't update Score here. Use the previous score
                NinjaScript.poleNumber = Int32.Parse(NinjaScript.successivePoleName.Remove(0,4));
            }
            else GameReset(0, 11F); // else start from the beginning
        }
        // If Respawn player is off then always start from the beginning
        else
        {
            GameReset(0, 11F);
            updateScore = true;
        }

        //Set Game Support Level
        //GameSupportLevel = PlayerPrefs.GetString("GameSupport");




        // Type of Assistance
        //TypeOfAssitance assistanceFromConfig = TypeOfAssitance.assistanceFromConfig;
        //TypeOfAssitance RTM = TypeOfAssitance.randomThresholdMethod;

        //Debug.Log("This is the selected Assistance Level:" + MainMenu.assistanceHindranceDropdown);
        // Type of Game Difficulty
        //Difficulty highT = Difficulty.HighTrajectory;
        //Difficulty lowT = Difficulty.LowTrajectory;
        //Difficulty neutral = Difficulty.Neutral;
        //Difficulty mixed = Difficulty.Mixed;
        //Difficulty lowHnd = Difficulty.LowHindrance;
        //Difficulty highHnd = Difficulty.HighHindrance;

        //Play the background music here !
        //AudioSource audio = GetComponent<AudioSource>();
        //if(!audio.isPlaying)
        //audio.Play();
        //Cursor.lockState = CursorLockMode.Confined;


        // url settings
        server_url = url;
    }

    void GameReset(float _poleX, float _poleY)
    {
        // We will instantiate each of the objects here sequentially
        // Player Object
        
        Instantiate(playerObject, new Vector2(_poleX, _poleY), Quaternion.identity);
        playerIsAlive = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<BoxCollider2D>().sharedMaterial = (PhysicsMaterial2D)Resources.Load("Material/1.0");

        //RealTrailObject
        Instantiate(realTrailObject, new Vector2(player.transform.position.x, player.transform.position.y), Quaternion.identity);

        // Pole Object
        // Depreciated - set all the poles directly from the file
        //poleObject =(GameObject)Instantiate(poleObject, new Vector2(-3.6f,-3f), Quaternion.identity);   // placing the first pole object on the stage
        placePole();    // Call the place pole function to instantiate all poles

        // Power Bar Object                                                                                                                                                //poleObject.transform.position = new Vector2(-3.5f, -0.6f);  // setting the pole position
        Instantiate(powerBarObject);

        // Timer settings
        if (StartScreenScript.gameInitialised == 1)
        {
            timer = totalGameTime * 60;
        }
        else if (StartScreenScript.gameInitialised == 0)
        {
            timer = NinjaScript.remainingTime;
        }

        // Start the timer
        timeStarted = true;
        timePaused = false;
    }

    void placePole()
    {
        // Columns of the config file is described here
        /*
         * __________________________________________________________________________________________________________________________________
         * |---PoleX---|---PoleY---|---Magnetism Level---|---Type of Pole---|---Friction Value---|---Magnetism Area---|---Assistance Level---|
         * |----5.26---|----4.86---|-----------5---------|----------2-------|--------0.8---------|----------L---------|----------75----------|
         * |---10.98---|----3.75---|-----------7---------|----------5-------|--------0.5---------|----------H---------|----------75----------|
         * |---15.45---|----2.10---|-----------9---------|----------4-------|--------0.6---------|----------M---------|----------75----------|
         * |---18.75---|----6.46---|----------10---------|----------3-------|--------0.4---------|----------L---------|----------75----------|
         * |---23.36---|----8.54---|-----------4---------|----------1-------|--------0.5---------|----------M---------|----------75----------|
         * |---30.48---|----3.60---|-----------0---------|----------6-------|--------0.7---------|----------L---------|----------75----------|
         * -----------------------------------------------------------------------------------------------------------------------------------
        */

        var columnX = new List<string>();                   // for first column of poleX position
        var columnY = new List<string>();                   // for second column of poleY position
        var columnMagnetismLevel = new List<string>();      // This is for the magnetism level - third
        var columnPoleType = new List<string>();            // for forth column of pole class
        var columnF = new List<string>();                   // for fifth column of pole friction
        var columnMagnetismArea = new List<string>();       // This is for the magnetism area - sixth

        // Setting the directories for the file path
        //Load the text file using Reources.Load
        TextAsset textFile = Resources.Load(fileToLoad) as TextAsset;
        refFileName = textFile.name;
        string fs = textFile.text;
        string[] textFileLines = Regex.Split(fs, "\r\n");

        for (int i = 0; i < textFileLines.Length; i++)
        {
            string valueLine = textFileLines[i];
            string[] values = Regex.Split(valueLine, ",");
            columnX.Add(values[0]);          
            columnY.Add(values[1]);
            columnMagnetismLevel.Add(values[2]);
            columnPoleType.Add(values[3]);
            columnF.Add(values[4]);
            columnMagnetismArea.Add(values[5]);
        }
        var counter = textFileLines.Length;


        // PoleObjects are the gameobject to hold that specific pole
        GameObject poleObject = null;      
        GameObject finalPole = null;



        // This is responsible to place all the poles in proper position
        for (int i = 0; i < counter; i++)
        {
            //print("See the Pole instantiate is getting called");
            if (columnPoleType[i] == "1") 
                poleObject = (GameObject)Instantiate(poleObjectRock, new Vector2(float.Parse(columnX[i]), float.Parse(columnY[i])), Quaternion.identity);
            else if (columnPoleType[i] == "2")
                poleObject = (GameObject)Instantiate(poleObjectBrick, new Vector2(float.Parse(columnX[i]), float.Parse(columnY[i])), Quaternion.identity);
            else if (columnPoleType[i] == "3")
                poleObject = (GameObject)Instantiate(poleObjectBrickAndGrass, new Vector2(float.Parse(columnX[i]), float.Parse(columnY[i])), Quaternion.identity);
            else if (columnPoleType[i] == "4")
                poleObject = (GameObject)Instantiate(poleObjectPipeAndIce, new Vector2(float.Parse(columnX[i]), float.Parse(columnY[i])), Quaternion.identity);
            else if (columnPoleType[i] == "5")
                poleObject = (GameObject)Instantiate(poleObjectIce, new Vector2(float.Parse(columnX[i]), float.Parse(columnY[i])), Quaternion.identity);
            else if (columnPoleType[i] == "6")  
            {
                //final pole
                finalPole = (GameObject)Instantiate(poleEndObject, new Vector2(float.Parse(columnX[i]), float.Parse(columnY[i])), Quaternion.identity);
                finalPole.name = "PoleEnd";
            }
            
            //assigning pole names
            if (columnPoleType[i] != "6") poleObject.name = "Pole" + "0" + i;           // We are assigning i + 1 because i starts from 0 and we already had Pole1 in the scene
        }

        ////Setting the Pole Magnetism
        // We will find the poles tagged with PoleMagnetism - which means only the collider area over the poles
        GameObject[] poleMagObjects;
        poleMagObjects = GameObject.FindGameObjectsWithTag("PoleMagnetism");
        //float PoleFriction;
        counter = 0;
        //var rad = 0.0F;
        foreach (GameObject pole in poleMagObjects)
        {
            // If the currentPoleColl is the one where the player resides then don't turn on magnetism for that pole
            //if (pole.transform.parent.gameObject.name == currentPoleColl) continue;


            // Setting the point effector value over here
            pole.GetComponentInChildren<PointEffector2D>().forceMagnitude = float.Parse(columnMagnetismLevel[counter].ToString());

            //Calculate appropriate drag based on this magnetism
            //Debug.Log(pole.GetComponentInChildren<PointEffector2D>().forceMagnitude);
            if (pole.GetComponentInChildren<PointEffector2D>().forceMagnitude == -30.0F)
            {
                // Case 01 Highest Magnetism
                pole.GetComponentInChildren<PointEffector2D>().drag = 5.25F;
            }
            else if (pole.GetComponentInChildren<PointEffector2D>().forceMagnitude == -15.0F)
            {
                // Case 02 Medium Magnetism
                pole.GetComponentInChildren<PointEffector2D>().drag = 2.625F;
            }
            else
            {
                // Case 03 No Magnetism
                pole.GetComponentInChildren<PointEffector2D>().drag = 1;
            }

                // MagnetismArea gets calculated here based on the config file that we had
                var rad = pole.GetComponentInChildren<CircleCollider2D>().radius;

            if (columnMagnetismArea[counter].ToString() == "L")
                rad = rad * 0.50F;
            else if (columnMagnetismArea[counter].ToString() == "M")
                rad = rad * 0.75F;
            else
                rad = rad * 1.00F;

            pole.GetComponentInChildren<CircleCollider2D>().radius = rad;
            pole.GetComponentInChildren<PointEffector2D>().enabled = false;             // set the magnetism false by default 
            counter++;
        }



        //// Setting the Pole Friction
        poleFrictionObjects = GameObject.FindGameObjectsWithTag("Pole");
        //Debug.Log(poleFrictionObjects.Length);
        counter = 0;
        foreach (GameObject pole in poleFrictionObjects)
        {
            // Setting the friction value over here
            float frictionValue = float.Parse(columnF[counter].ToString());           
            pole.GetComponent<BoxCollider2D>().sharedMaterial = (PhysicsMaterial2D)Resources.Load("Material/" + frictionValue.ToString("0.00"));
            counter++;
        }
    }


    public void ChangeScene(string sceneName)
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        canvasObject.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(sceneName);
    }

    // This function is executed at each of our game frame
    void Update()
    {
        //if(Cursor.lockState != CursorLockMode.Confined)
        //Cursor.lockState = CursorLockMode.Confined; // keep confined in the game window
        //// Depreciated for UNITY 5_3
        // Check the Day/Night Settings
        GameObject SunLight = GameObject.Find("Sun");
        SunLight.GetComponent<Light>().intensity = currentLightSettings;

        // Turning on the moon when the light intensity goes down
        GameObject Moon = GameObject.Find("MoonCrescent");
        GameObject SunHalo = GameObject.Find("SunHalo");
        GameObject Pl1 = GameObject.Find("Point light_1");
        GameObject Pl2 = GameObject.Find("Point light_2");
        GameObject Pl3 = GameObject.Find("Point light_3");
        if (currentLightSettings < 0.25)
        {
            SunHalo.GetComponent<SpriteRenderer>().enabled = false; // Turn off Sun
            SunHalo.GetComponent<SpriteRenderer>().enabled = false;
            Moon.GetComponent<SpriteRenderer>().enabled = true;     // Turn on Moon

            Pl1.GetComponent<Light>().enabled = true;
            Pl2.GetComponent<Light>().enabled = true;
            Pl3.GetComponent<Light>().enabled = true;
        }
        else
        {
            SunHalo.GetComponent<SpriteRenderer>().enabled = true; // Turn on Sun
            Moon.GetComponent<SpriteRenderer>().enabled = false;   // Turn off Moon

            Pl1.GetComponent<Light>().enabled = false;
            Pl2.GetComponent<Light>().enabled = false;
            Pl3.GetComponent<Light>().enabled = false;
        }

        // Find the player object
        playerObject = GameObject.FindGameObjectWithTag("Player");
        realTrailObject = GameObject.FindGameObjectWithTag("RealTrail");


        //Add a shortcut button for turning on the trajectory
        TrajectoryPredictor trajectoryScript01 = playerObject.GetComponent("TrajectoryPredictor") as TrajectoryPredictor;
        TrajectoryPredictor trajectoryScript02 = realTrailObject.GetComponent("TrajectoryPredictor") as TrajectoryPredictor;

        if(PlayerPrefs.GetString("DebuggerMode").Equals("True"))
        {
            //// For Player
            trajectoryScript01.drawDebugOnUpdate = true;
            trajectoryScript01.drawDebugOnPrediction = true;

            //// For Real Trail
            trajectoryScript02.drawDebugOnUpdate = true;
            trajectoryScript02.drawDebugOnPrediction = true;
        }

        else
        {
            bool ctrl = Input.GetKey(KeyCode.RightControl);
            bool alt = Input.GetKey(KeyCode.RightAlt);
            bool t = Input.GetKeyDown(KeyCode.T);


            if ((ctrl && alt && t) || (alt && ctrl && t))
            {
                if (!pressedOnce) pressedOnce = true;
                else pressedOnce = false;
            }

            if (pressedOnce)
            {
                // For Player
                trajectoryScript01.drawDebugOnUpdate = true;
                trajectoryScript01.drawDebugOnPrediction = true;

                // For Real Trail
                trajectoryScript02.drawDebugOnUpdate = true;
                trajectoryScript02.drawDebugOnPrediction = true;
            }
            else if (!pressedOnce)
            {
                // For Player
                trajectoryScript01.drawDebugOnUpdate = false;
                trajectoryScript01.drawDebugOnPrediction = false;

                // For Real Trail
                trajectoryScript02.drawDebugOnUpdate = false;
                trajectoryScript02.drawDebugOnPrediction = false;
            }
        }




        // Check the Magnetism Properties and trail renderer properties
        NinjaScript ninjaScript = playerObject.GetComponent("NinjaScript") as NinjaScript;
        if (MagnetismOn)
            ninjaScript.magnetismOn = true;
        else
            ninjaScript.magnetismOn = false;

        // Check the Trajectory Assistance Properties and trail renderer properties
        if (TrajectoryAssistanceOn)
            ninjaScript.trajectoryAssistanceOn = true;
        else
            ninjaScript.trajectoryAssistanceOn = false;



        // Timer Game Object
        GameObject timerObject = GameObject.Find("Timer");
        // Timer settings shown on top of the screen
        if (timeStarted == true && timePaused == false && timer > 1)
        {
            timer -= Time.deltaTime;
            elapsedTime += Time.deltaTime;
            //elapsedTime = totalGameTime * 60 - timer;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer - minutes * 60);
            string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
            timerObject.GetComponent<Text>().text = niceTime;
        }
        else if (timer <= 1 && timeStarted == true && timePaused == false && !calledOnce)
        {
            timeStarted = false;
            timePaused = true;


            // Show the Game Over Screen again but with Time Out wriiten
            // Enable Canvas section 
            GameObject canvasObject = GameObject.Find("Canvas");
            Instantiate(canvasObject);
            canvasObject.GetComponent<Canvas>().enabled = true;
            Image panelImage = canvasObject.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 1); //newPanelColor;

            // Disable Game Over Image
            GameObject gameOverObj = GameObject.Find("GameOverImage");
            gameOverObj.GetComponent<Image>().enabled = false;

            // Enable TimeOut Image
            Text TimeOutObj = GameObject.Find("TimeOutText").GetComponent<Text>();
            TimeOutObj.enabled = true;

            // Disbale the restart button
            GameObject buttonRestart = GameObject.Find("ButtonRestart");
            buttonRestart.GetComponent<Image>().enabled = false;
            buttonRestart.GetComponent<Button>().enabled = false;
            
            // Destroy the player first to remove any additional log
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Destroy(player);
            playerIsAlive = false;

            // pause for 5 seconds and then destroy everything
            StartCoroutine(TimeUp(5));
        }
        else if (timePaused)
        {
            timerObject.GetComponent<Text>().text = "PAUSED";
            RectTransform rt = timerObject.GetComponent<RectTransform>();
            rt.localPosition = new Vector2(35, 0);
            rt.localScale = new Vector2(0.60F,0.60F);
        }
        //float playerYPosition = GameObject.FindWithTag("Player").transform.position.y;
        float playersXvelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.x;
        float playersYvelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.y;

        // Mouse button is down, but ninja isn't jumping/falling (its y velocity is zero) and the ninja is not charging
        if (Input.GetButtonDown("Fire1") && playersXvelocity == 0 && playersYvelocity == 0 && !isCharging && playerIsAlive && !timePaused)
        {
            // check if the real trail exist or not
            if (GameObject.FindWithTag("RealTrail") != null)
            {
                //Destroy(GameObject.FindWithTag("RealTrail"));
                GameObject.FindWithTag("RealTrail").transform.position = playerObject.transform.position;
                GameObject.FindWithTag("RealTrail").transform.rotation = playerObject.transform.rotation;

                //Debug.Log("realTrailObject.transform.position: " + realTrailObject.transform.position);
            }
            isCharging = true;    	// now the ninja will be charging 
        }

        // Mouse button released and the ninja is charging but not jumping/falling (its y velocity is zero) 
        if (Input.GetButtonUp("Fire1") && playersXvelocity == 0 && playersYvelocity == 0 && isCharging && playerIsAlive && !timePaused)
        {
            isCharging = false;                                                             // player is no longer charging so set the isCharging to false
            GameObject powerObject = GameObject.FindWithTag("Power");                       // get the game object tagged as "Power"
            PowerScript script = powerObject.GetComponent("PowerScript") as PowerScript;    // inside the object tagged as "Power", get PowerScript script
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            player.SendMessage("Jump", maxJumpForce * script.chargePowerSender);            // find the object tagged as "Player" and send "Jump" message, with the proper force
            updateScore = true;                                                             // Set the updateScore to true because the player just jumped
        }
    }
    private IEnumerator TimeUp(int seconds)
    {
        yield return new WaitForSeconds(seconds);     // wait for several seconds
        
        //// Destroy everything in the scene
        foreach (GameObject o in UnityEngine.Object.FindObjectsOfType<GameObject>())
            Destroy(o);
        Application.ExternalCall("EndScene");
        //openWindow(server_url + next_page_after_game_end);
        //Application.OpenURL(server_url + next_page_after_game_end);
    }
}

