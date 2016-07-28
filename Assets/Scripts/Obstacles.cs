using UnityEngine;


public class Obstacles : MonoBehaviour { 

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "Player")
        {
            GameManager.playerCollision();
        }
    }
}
