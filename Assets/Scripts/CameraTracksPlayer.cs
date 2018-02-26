using UnityEngine;

public class CameraTracksPlayer : MonoBehaviour
{
    //public static CameraTracksPlayer instance;
    Transform player;
    public static bool playerOnLastPole = false;

    // Use this for initialization
    void Start()
    {
        GameObject player_go = GameObject.FindWithTag("Player");
        if (player_go == null)
        {
            Debug.Log("Couldn't find an object with tag 'Player' !");
            return;
        }
        player = player_go.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Checking whether there is player in the scene and the velocity of the pole is 0
        // -- Changed done -- This might have to be changed to track the camera while the player is also bouncing
        GameObject player_go = GameObject.FindWithTag("Player");
        player = player_go.transform;
        if (player != null && !playerOnLastPole)
        {
            Vector3 pos = transform.position;
            pos.x = player.position.x;
            transform.position = pos;
        }
    }
}
