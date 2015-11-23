using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	
	public Node targetNode;
	public Node mainTarget;

	public float Speed;
	public float distanceForTargetChange;

	void Update () {
		if (targetNode != null) {
			transform.position = Vector3.MoveTowards (transform.position, targetNode.transform.position, Speed * Time.deltaTime);
			if (Vector3.Distance (transform.position, targetNode.transform.position) < distanceForTargetChange) {
				targetNode = targetNode.NextTarget (mainTarget);
			}
		}
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
				Node node = hit.transform.GetComponent <Node> ();
				if (node != null) {
					mainTarget = node;
					if (targetNode == null)
						targetNode = mainTarget;
				}
			}
		}
	}
}
