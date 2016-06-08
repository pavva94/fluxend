using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour
{
	public float speed;
	public Boundary boundary;
	public GameObject startPoint;

	new Rigidbody2D rigidbody;

    private float minX, maxX, minY, maxY;

    void Start()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
		Reset ();
        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        minX = bottomCorner.x;
        maxX = topCorner.x;
        minY = bottomCorner.y;
        maxY = topCorner.y;
    }

	void Update ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
        rigidbody.velocity = movement * speed;

        // Get current position
        Vector3 pos = transform.position;

        // Horizontal contraint
        if (pos.x < minX) pos.x = minX;
        if (pos.x > maxX) pos.x = maxX;

        // vertical contraint
        if (pos.y < minY) pos.y = minY;
        if (pos.y > maxY) pos.y = maxY;

        // Update position
        transform.position = pos;
    }

	void OnBecameInvisible() {
		Stop ();
	}

	public void Reset() {
		rigidbody.position = startPoint.transform.position;
		Debug.Log (startPoint.transform.position);
		Stop ();

	}

	void Stop() {
		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0.0f;
	}
}