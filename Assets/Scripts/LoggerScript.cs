using UnityEngine;
using System.Collections;

public class LoggerScript : MonoBehaviour {

    //string playerScoreWithoutMagnetism = ""; // Setting up the playerScoreWithoutMagnetism 
    //string playerScoreWithoutAssistance = ""; // Setting up the playerScoreWithoutMagnetism 
    string successWithoutAnyTypeOfAssist = "";
    string prePoleNumber = "", prePower = "", preDistanceToTheNearestPole = "";
    bool LogData;
    string poleSection = "";
    private MainScript mainScript;
    private GameObject gameEngine;
    static float eTNeutral = 0F;
    static float eTOther = 0F;
    static bool isFirstOccurence = true;

    public void loggerMethodFromRealTrail(string successWithoutAnyAssist)
    {
        successWithoutAnyTypeOfAssist = successWithoutAnyAssist;
    }

    public void loggerMethod(int currentSuccess, float poleDistance, float poleHeight, string currentPoleName, string currentPoleMag, string currentPoleFric, int getScore, string currentPoleMagAr, string nearestPoleName, float distanceOfNearestPoleXPosition, float assistanceLevel, float hindranceLevel)
    {
        GameObject powerObject = GameObject.FindWithTag("Power");                       // get the game object tagged as "Power"
        PowerScript powerScript = powerObject.GetComponent("PowerScript") as PowerScript;    // inside the object tagged as "Power", get PowerScript script

        // We actually do not need this section -- but it's still here ! What a waste of energy and time
        // Since everything from scripts are getting here without a proper serialization as we are updating the project quite a few times 
        // this section makes everything in a serial order for the log file
        string PoleNumber = currentPoleName;                // the current pole number where the player is      
        string PoleDistance = poleDistance.ToString();      // the current pole X position
        string PoleHeight = poleHeight.ToString();          // the current pole Y position
        string PoleMagnetism = currentPoleMag;              // the current pole's magnetism
        string PoleFriction = currentPoleFric;              // the current pole's materials friction
        string Power = powerScript.mouseDepressPower.ToString(); // the power that is set to the      
        string PoleMagnetismArea = currentPoleMagAr;        // Magnetism Area of the pole
        string DistanceToTheNearestPole = distanceOfNearestPoleXPosition.ToString();
        string trajectoryAssistance = assistanceLevel.ToString();
        string trajectoryHindrance = hindranceLevel.ToString();
        int currentScore = getScore;        // the current score of the player
        int Success = currentSuccess;       // success ?
        int Failure = 0;
        if (currentSuccess == 0) Failure = 1;      // if not success then failed


        // Check for duplicate log 
        // It looks like due to duplicate hit there might be more than one log for a single pole
        // Hacked!
        if (prePoleNumber == PoleNumber &&
            prePower == Power &&
            preDistanceToTheNearestPole == DistanceToTheNearestPole)
        {
            LogData = false;
        }
        else
            LogData = true;

        // Identifying Round Sections
        // This section must be deleted after the peak-end study
        // First check if the nearest Pole Number has a length more than 4
        int poleNumberID = 0;
        if (nearestPoleName.Length > 4 && !nearestPoleName.Equals("poleEnd"))
            poleNumberID = int.Parse(nearestPoleName.Substring(4));


        // Get the string of the filename
        string nameOfFile = MainScript.refFileName.ToString();

        if ((poleNumberID >= 1 && poleNumberID <= 8)
            || (poleNumberID >= 13 && poleNumberID <= 20))
        {
            poleSection = "Neutral";
        }
        else if (((poleNumberID >= 9 && poleNumberID <= 12)
            || (poleNumberID >= 21 && poleNumberID <= 24))
            && MainScript.refFileName.Substring(nameOfFile.Length-8).Equals("Positive"))
        {
            poleSection = "Positive";
        }

        else if (((poleNumberID >= 9 && poleNumberID <= 12)
                    || (poleNumberID >= 21 && poleNumberID <= 24))
                    && MainScript.refFileName.Substring(nameOfFile.Length - 8).Equals("Negative"))
        {
            poleSection = "Negative";
        }
        else
        {
            poleSection = "";
        }


        // Reset ElapsedTime timer after specific PoleSection
        if (PoleNumber.Equals("Pole08") && isFirstOccurence)
        {
            eTNeutral = MainScript.elapsedTime;
            MainScript.elapsedTime = 0;
            isFirstOccurence = false;
        }
        else if(PoleNumber.Equals("Pole09") || PoleNumber.Equals("Pole010") || PoleNumber.Equals("Pole011"))
        {
            isFirstOccurence = true;
        }
        else if (PoleNumber.Equals("Pole012") && isFirstOccurence)
        {
            eTOther = MainScript.elapsedTime;
            MainScript.elapsedTime = 0;
            isFirstOccurence = false;
        }
        else if (PoleNumber.Equals("Pole013") || PoleNumber.Equals("Pole014") || PoleNumber.Equals("Pole015"))
        {
            isFirstOccurence = true;
        }
        else if(PoleNumber.Equals("Pole020") && isFirstOccurence)
        {
            eTNeutral += MainScript.elapsedTime;
            MainScript.elapsedTime = 0;
            isFirstOccurence = false;
        }
        else if (PoleNumber.Equals("Pole021") || PoleNumber.Equals("Pole022") || PoleNumber.Equals("Pole023"))
        {
            isFirstOccurence = true;
        }
        else if(PoleNumber.Equals("PoleEnd") && isFirstOccurence)
        {
            eTOther += MainScript.elapsedTime;
            MainScript.elapsedTime = 0;
        }





        prePoleNumber = PoleNumber;                             // set pre for this data
        prePower = Power;                                       // set Power equal to prePower 
        preDistanceToTheNearestPole = DistanceToTheNearestPole; // set DistanceToTheNearestPole equal to the preDistanceToTheNearestPole

        if (LogData)
        {
            string url = MainScript.server_url + "/loggame";

            WWWForm form = new WWWForm();
            form.AddField("poleNumber", PoleNumber);
            form.AddField("poleDistance", PoleDistance);
            form.AddField("poleHeight", PoleHeight);
            form.AddField("currentTime", MainScript.timer.ToString());
            form.AddField("elapsedTimeNeutral", eTNeutral.ToString());
            form.AddField("elapsedTimePeakEnd", eTOther.ToString());
            form.AddField("nearestPoleName", nearestPoleName);
            form.AddField("poleSection", poleSection);
            form.AddField("distanceToTheNearestPole", DistanceToTheNearestPole);
            form.AddField("magnetismLevel", PoleMagnetism);
            form.AddField("magnetismArea", PoleMagnetismArea);
            form.AddField("trajectoryAssistance", trajectoryAssistance);
            form.AddField("trajectoryHindrance", trajectoryHindrance);
            form.AddField("friction", PoleFriction);
            form.AddField("power", Power);
            form.AddField("score", currentScore);
            form.AddField("success", Success);
            form.AddField("failure", Failure);
            form.AddField("successWithoutAnyAssist", successWithoutAnyTypeOfAssist);
            // Has the game has been initialised or not. Restarting after death will be set to 0
            form.AddField("gameInitialised", StartScreenScript.gameInitialised);


            //Log data in the unity inspector
            // Un-Comment the next section for showing it into the unity inspector
            //Debug.Log("PoleNumber: " + PoleNumber + "\n" +
            //"Pole Distance: " + PoleDistance + "\n" +
            //"PoleHeight: " + PoleHeight + "\n" +
            //"Current Time in Sec: " + MainScript.timer.ToString() + "\n" +
            //"Elapsed Time Neutral: " + eTNeutral.ToString() + "\n" +
            //"Elapsed Time Peak End: " + eTOther.ToString() + "\n" +
            //"Nearest Pole Number: " + nearestPoleName + "\n" +
            //"Pole Section: " + poleSection + "\n" +
            //"DistanceToTheNearestPole: " + DistanceToTheNearestPole + "\n" +
            //"PoleMagnetism: " + PoleMagnetism + "\n" +
            //"Magnetism Area: " + currentPoleMagAr + "\n" +
            //"Trajectory Assistance: " + trajectoryAssistance + "\n" +
            //"Trajectory Hindrance: " + trajectoryHindrance + "\n" +
            //"Pole Friction: " + PoleFriction + "\n" +
            //"Power: " + Power + "\n" +
            //"Current Score: " + currentScore + "\n" +
            //"Success: " + Success + "\n" +
            //"Failure: " + Failure + "\n" +
            //"Player Score Without Magnetism: " + playerScoreWithoutMagnetism + "\n" +
            //"Game Initialised: " + StartScreenScript.gameInitialised);


            WWW www = new WWW(url, form);

            StartCoroutine(WaitForRequest(www));
        }
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            //Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            //Debug.Log("WWW Error: " + www.error);
        }
    }
}
