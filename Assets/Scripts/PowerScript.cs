using System;
using UnityEngine;
using UnityEngine.UI;

public class PowerScript : MonoBehaviour
{
    GameObject playerObject;
    public float chargePower = 0;
    public float chargePowerSender = 0;
    public Scrollbar powerbar;
    public float mouseDepressPower;

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");   // Find the player gameObject with Player Tag
        tag = "Power";              // tagging the object as "Power"
        powerbar.size = 0f;         // Initially the size of the power bar is 0%  
    }

    void Update()
    {
        //float playerYPosition = playerObject.transform.position.y;    // Find the player's Y position
        float playersXvelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.x;
        float playersYvelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.y;

        if (Input.GetMouseButton(0) && playersXvelocity == 0 && playersYvelocity == 0 && MainScript.playerIsAlive == true)
        {
            chargePower = Mathf.Min(chargePower + Time.deltaTime, 1f);      // creating amount of chargePower based on the amount of time mouse button was clicked
            powerbar.size = chargePower * 1f;                               // setting the powerbar based on the 
        }
        else
        {
            chargePowerSender = chargePower;			// chargePowerSender is responsible to send the power value of current jump to main script
            chargePower = 0;							// setting the temporary chargePower value back to 0	
            powerbar.size = 0f;							// powerbar set back to zero again for the next jump
        }

        if (chargePowerSender > 0f)
        {
            saveChargePower(chargePowerSender); // here, a new function is called to send it right away to the ninja script
        }
    }

    public void saveChargePower(float chargePowerSender)
    {
        mouseDepressPower = chargePowerSender;
    }
}
