using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract classes are incomplete and must be completed in the derived class
public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f; //seconds
	public LayerMask blockingLayer; // this layer will check collisions, prefabs already are assigned 
	// Use this for initialization

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime; // movement calculations

	// protected virtual functions can be overwritten by its inherited classes, this is useful is we want one of these classes to have a different implementation of start
	protected virtual void Start () {

		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime; // we'll use this way because multiplying is more efficient computationally than dividding
	}

	protected bool Move (int xDir, int yDir, out RaycastHit2D hit){

		// we cast it to vector2 so we discard the Z axys from vector3 data
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine (SmoothMovement (end));
			return true; // we were able to move
		}
		return false;

	}

	protected IEnumerator SmoothMovement (Vector3 end){

		// sqrMagnitude is cheaper than Magnitude
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon){
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			// Now we recalculate the remaining distance after we moved
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			// and we'll wait for a frame before reevaluating the condition of the loop
			yield return null;

		}
	}

	// returns void and takes ageneric parameter T and 2 integer for xDir and Ydir
	protected virtual void AttemptMove <T> (int xDir, int yDir)
	// specify the type of component we expect out units to interact if blocked
			where T: Component
	{
			RaycastHit2D hit;
			bool canMove = Move(xDir, yDir, out hit);

			if(hit.transform == null)
				return;
			T hitComponent = hit.transform.GetComponent<T>();
		
			if(!canMove && hitComponent !=null)
				OnCantMove(hitComponent);
	
	}


	// note as this is abstract it does not have enclosing brackets
	protected abstract void OnCantMove <T> (T component)
			where T: Component;

}
