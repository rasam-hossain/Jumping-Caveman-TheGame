using UnityEngine;

public class PoleScript : MonoBehaviour
{
    public LayerMask m_MagneticLayers;
    public Vector3 m_Position;
    public float m_Radius;
    public float m_Force;
    

    void Start()
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        // tagging the object as Pole
        gameObject.tag = "Pole";
    }

    void FixedUpdate()
    {
        Collider2D[] colliders;
        Rigidbody2D rigidbody;

        colliders = Physics2D.OverlapCircleAll(transform.position + m_Position, m_Radius, m_MagneticLayers);
        //Debug.Log(colliders);

        foreach (Collider2D collider in colliders)
        {
            Debug.Log(collider.name);
            rigidbody = (Rigidbody2D)collider.gameObject.GetComponent(typeof(Rigidbody2D));
            Debug.Log(rigidbody.name);
            if (rigidbody == null)
            {
                Debug.Log("Magnetism doesn't work");
            }
            rigidbody.AddForce((collider.transform.position - transform.position) * m_Force * Time.smoothDeltaTime, ForceMode2D.Force);
            Debug.Log(transform.position * m_Force);
            Debug.Log("Magnetism works");

        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + m_Position, m_Radius);
    }

    void scroll()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(-3f, 0f);				// assigning pole an horizontal speed
    }

    void stop()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);					// stopping the pole's x axis speed
    }
}
