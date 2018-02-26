using UnityEngine;

public class BGLooperScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {

        // Collision check for the background and the clouds only
        if (collider.name == "bg_0" || collider.name == "bg_1" || collider.name == "bg_2" || collider.name == "bg_3")          
        {
            float widthOfBGObject = ((BoxCollider2D)collider).size.x;   // check the widht of the background object
            Vector3 pos = collider.transform.position;
            pos.x += (widthOfBGObject * 8) - 0.38f;                     // transform the background to right
            collider.transform.position = pos;
        }

        else if (collider.name == "blueCloud1" || 
                 collider.name == "blueCloud2" || 
                 collider.name == "blueCloud3" ||
                 collider.name == "blueCloud4" ||
                 collider.name == "blueCloud5" ||
                 collider.name == "blueCloud6" ||
                 collider.name == "blueCombo")
        {
            //float widthOfBGObject = ((BoxCollider2D)collider).size.x;   // check the widht of the background cloud object
            Vector3 pos = collider.transform.position;
            pos.x += 31f * 4;                             // transform the background to right
            collider.transform.position = pos;
        }

        // Collision check for the ground as they are 1/3 size of the background and the clouds
        else
        {
            float widthOfBGObject = ((BoxCollider2D)collider).size.x;   // check the widht of the background object
            Vector3 pos = collider.transform.position;
            //pos.x += (widthOfBGObject * 16) - widthOfBGObject/2;       // dropped this particular line after updating to 5.6
            pos.x += (widthOfBGObject * 16);       // transform the background to right
            collider.transform.position = pos;
        }    
     }
}
