using UnityEngine;


public class Obstacles : MonoBehaviour {

    private Vector2 bottomCorner;

    void Start()
    {
        bottomCorner = GameManager.bottomCorner;
    }

    void Update()
    {
        // TODO find che coordinates of bottom screen
        if (transform.position.y < bottomCorner.y - 10)
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
