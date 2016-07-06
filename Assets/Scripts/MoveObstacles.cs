using UnityEngine;
using System.Collections;

public class MoveObstacles : MonoBehaviour {

    public float speed;
    private float screenCenterX;
    private float moveVertical;

    new Rigidbody2D rigidbody;

    // Use this for initialization
    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();

        screenCenterX = Screen.width * 0.5f;
        moveVertical = 0.2f;
    }

    // Update is called once per frame
    void Update() {
        // movimento utente blocchi
        transform.Translate(Vector3.down * moveVertical);
        // if there are any touches currently
        if (Input.touchCount > 0)
        {
            // get the first one
            Touch firstTouch = Input.GetTouch(0);

            // if it began this frame
            if (firstTouch.phase == TouchPhase.Moved || firstTouch.phase == TouchPhase.Stationary)
            {
                if (firstTouch.position.x > screenCenterX)
                {
                    // if the touch position is to the right of center
                    // move right
                    transform.Translate(speed, Vector3.down.y * moveVertical, 0);
                }
                else if (firstTouch.position.x < screenCenterX)
                {
                    // if the touch position is to the left of center
                    // move left
                    transform.Translate(-speed, Vector3.down.y * moveVertical, 0);
                }
            }
        }
    
    // movimento verticale costante blocchi
    /*float moveVertical = -0.1f;

    Vector2 movement = new Vector2(moveHorizontal, moveVertical);
    rigidbody.velocity = movement * speed;
    //Debug.Log(transform.position.x);
    if (moveHorizontal > 0) transform.Translate(Vector3.right * Time.deltaTime);
    else if (moveHorizontal < 0) transform.Translate(Vector3.left * Time.deltaTime);*/

}

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "Player")
            Time.timeScale = 0;

    }
}
