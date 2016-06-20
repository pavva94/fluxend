using UnityEngine;
using System.Collections;

public class MoveObstacles : MonoBehaviour {

    public float speed;
    private float minX, maxX, minY, maxY;

    new Rigidbody2D rigidbody;

    // Use this for initialization
    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();

        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        minX = bottomCorner.x;
        maxX = topCorner.x;
        minY = bottomCorner.y;
        maxY = topCorner.y;
    }

    // Update is called once per frame
    void Update() { 
        // movimento utente blocchi
        float moveHorizontal = Input.GetAxis("Horizontal");
        // movimento verticale costante blocchi
        float moveVertical = -0.1f;
        
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rigidbody.velocity = movement * speed;
        //Debug.Log(transform.position.x);
        if (moveHorizontal > 0) transform.Translate(Vector3.right * Time.deltaTime);
        else if (moveHorizontal < 0) transform.Translate(Vector3.left * Time.deltaTime);

    }
}
