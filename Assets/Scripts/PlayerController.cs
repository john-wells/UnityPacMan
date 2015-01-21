using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	private float saveMoveSpeed;
	public float slowDown;
	public float widthCheck;
	private Vector2 direction;
	private Vector2 inputDir;

	private float timer;

	//Booleans to allow for saving of new direction
	private bool upInput;
	private bool downInput;
	private bool leftInput;
	private bool rightInput;

	// Use this for initialization
	void Start () {
		direction = Vector2.right;
		saveMoveSpeed = moveSpeed;
		upInput = false;
    	downInput = false;
    	leftInput = false;
		rightInput = false;
	}

	void OnTriggerEnter2D(Collider2D co) {
		if (co.name == "Power Pellet") {
			Destroy (co.gameObject);
			moveSpeed = moveSpeed - slowDown;
			timer = 0;
		}
		if (co.name == "TeleportR") {
			transform.position = (new Vector3(-30,2,0));
		}
		if (co.name == "TeleportL") {
			transform.position = (new Vector3(30,2,0));
		}
	}

	void FixedUpdate() {


		timer = Time.time;
		if (timer > 0.5f) {
			moveSpeed = saveMoveSpeed;
		}

		/*Statements to accept user input*/
		if (Input.GetKey (KeyCode.UpArrow)) {
			inputDir = Vector2.up;
			upInput = true;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			inputDir = -Vector2.up;
			downInput = true;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			inputDir = -Vector2.right;
			leftInput = true;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			inputDir = Vector2.right;
			rightInput = true; 
		}

		/*Statements which allow directional change*/
		if (colCheckVert (Vector2.up) && upInput) {
			direction = inputDir;
			upInput = false; 
		}
		if (colCheckVert (-Vector2.up) && downInput) {
			direction = inputDir;
			downInput = false;
		}
		if (colCheckHorz (-Vector2.right) && leftInput) {
			direction = inputDir;
			leftInput = false;
		}
		if (colCheckHorz (Vector2.right) && rightInput) {
			direction = inputDir;
			rightInput = false;
		}

		Move ();
		Animations ();

	}

	/*This function uses the global direction variable to update the animation*/
	void Animations() {
		GetComponent<Animator>().SetFloat("DirX",direction.x);
		GetComponent<Animator>().SetFloat("DirY",direction.y);
	}

	/*Movement is done outside FixedUpdate so that Pacman can move even when input is not currently being given*/
	void Move() {

			Vector2 position = new Vector2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y));
			rigidbody2D.MovePosition (Vector2.MoveTowards (transform.position, position + direction, moveSpeed));
	}

	/*Function to check if a wall has been hit*/
	bool colCheckVert(Vector2 colDir) {

		Vector3 colDir3D = new Vector3 (colDir.x, colDir.y, 0f);	//For Debug.DrawLine
		//Vector2 position = transform.position;
		 
		RaycastHit2D colL = Physics2D.Linecast (((Vector2)transform.position - new Vector2(widthCheck,0) + (colDir*2.5F)), ((Vector2)transform.position  - new Vector2(widthCheck,0)));
		RaycastHit2D colM = Physics2D.Linecast ((Vector2)transform.position + (colDir*2.5F), ((Vector2)transform.position));
		RaycastHit2D colR = Physics2D.Linecast ((Vector2)transform.position + new Vector2(widthCheck,0) + (colDir*2.5F), ((Vector2)transform.position + new Vector2(widthCheck,0)));

		Debug.DrawLine (transform.position - new Vector3(widthCheck,0) + (colDir3D*2.5F), transform.position  - new Vector3(widthCheck,0));
		Debug.DrawLine (transform.position + (colDir3D*2.5F), transform.position);
		Debug.DrawLine (transform.position + new Vector3(widthCheck,0) + (colDir3D*2.5F), transform.position + new Vector3(widthCheck,0));
		//This linecast checks for walls from 2 units ahead to Pacman
		if (!colL.rigidbody || !colM.rigidbody || !colR.rigidbody) { //if it hits something other than pacman
			return false;
		}
		return true;
	}

	bool colCheckHorz(Vector2 colDir) {
	
		Vector3 colDir3D = new Vector3 (colDir.x, colDir.y, 0f);	//For Debug.DrawLine

		RaycastHit2D colL = Physics2D.Linecast ((Vector2)transform.position - new Vector2(0,widthCheck) + (colDir*2.5F), ((Vector2)transform.position  - new Vector2(0,widthCheck)));
		RaycastHit2D colM = Physics2D.Linecast ((Vector2)transform.position + (colDir*2.5F), ((Vector2)transform.position));
		RaycastHit2D colR = Physics2D.Linecast ((Vector2)transform.position + new Vector2(0,widthCheck) + (colDir*2.5F), ((Vector2)transform.position  + new Vector2(0,widthCheck)));
		
		Debug.DrawLine (transform.position - new Vector3(0,widthCheck) + (colDir3D*2.5F), transform.position  - new Vector3(0,widthCheck));
		Debug.DrawLine (transform.position + (colDir3D*2.5F), (transform.position));
		Debug.DrawLine (transform.position + new Vector3(0,widthCheck) + (colDir3D*2.5F), transform.position  + new Vector3(0,widthCheck));
		//This linecast checks for walls from 2 units ahead to Pacman
		if (!colL.rigidbody || !colM.rigidbody || !colR.rigidbody) { //if it hits something other than pacman
			return false;
		}
		return true;
	}
}

/*
 * What the Pacman movement must be able to do
 *	1. Arrow keys can dictate movement - Works
 *	2. You can dictate move orders to pacman before he reaches an intersection - Works partially, but turns before wall
 *	3. Cannot turn into walls - Works partially, except for edge of walls near an intersection
 *	4. Can reverse direction - Works
 *	5. If pacman goes through one side of the passage he comes out the other - Not started yet
 *	6. Slows down if pacman is eating - Not started yet
 */