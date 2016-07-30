using UnityEngine;


public class Obstacles : MonoBehaviour {

    void Start()
    {
        
    }

    void Update()
    {
        //Vector3 screenPos = camera.WorldToScreenPoint(transform.position);
        //Debug.Log("target is " + screenPos.x + " pixels from the left");

        if (transform.position.y < -20)
        {
            Object.Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "Player")
        {
            GameManager.playerCollision();
        }
    }
}
