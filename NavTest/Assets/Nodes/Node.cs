using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent (typeof (SphereCollider))]
public class Node : MonoBehaviour {
	#region structs 
	[System.Serializable]
	public struct NodeDistance {
		[SerializeField]  public NodeDirection  nodes;
		[SerializeField]  public float distance;
		public NodeDistance (NodeDirection nodes, float distance) {
			this.nodes	  = nodes;
			this.distance = distance;
		}
	}
	[System.Serializable]
	public class NodeDirection {
		public Node targetNode;
		public Node nextNode;
		public NodeDirection (Node targetNode, Node nextNode) {
			this.targetNode = targetNode;
			this.nextNode = nextNode;
		}
	}
	#endregion

	[HideInInspector] List <NodeDistance> nodesDictionary;
	[HideInInspector] public List <Node> connectedNodes;


	//static long counter = 0;
	public static List <Node> nodeInstances;
	void Awake () {
		if (nodeInstances == null)
			nodeInstances = new List<Node> ();
		connectedNodes = new List<Node> ();
		for (int i = 0; i < nodeInstances.Count; i++) {
			if (nodeInstances[i] != null) {
				Vector3 delta = nodeInstances[i].transform.position - transform.position;
				RaycastHit [] hit = Physics.RaycastAll (transform.position, delta, delta.magnitude);
				if (hit.Length == 1) {
					if (!connectedNodes.Contains (nodeInstances[i]))
						 connectedNodes.Add (nodeInstances[i]);
					if (!nodeInstances [i].connectedNodes.Contains (this))
						 nodeInstances [i].connectedNodes.Add (this);
				}
			}
		}
		nodeInstances.Add (this);
	}
	void Start () {
		List <Node> visited = new List<Node> ();
		nodesDictionary 	= new List<NodeDistance> ();
		visited.Add (this);
		nodesDictionary = nodesDistances (visited, 0f, this);
	}
	//void Update () {
	//	if (Input.GetMouseButtonDown (0))
	//		print (counter);
	//}
	public Node NextTarget (Node mainTarget) {
		for (int i = 0; i < nodesDictionary.Count; i++) {
			int index;
			if (TargetContained (nodesDictionary,mainTarget, out index))
				return nodesDictionary[index].nodes.nextNode;
		}
		return this;
	}
	public List <NodeDistance> nodesDistances (List<Node> visited, float weight, Node caller) {
		List <Node> newVisited = new List<Node> ();
		foreach (Node node in visited) {
			newVisited.Add (node);
		}
		newVisited.Add (this);

		List <NodeDistance> distances = new List<NodeDistance> ();
		distances.Add (new NodeDistance (new NodeDirection (this, this), weight));
		for (int i = 0; i < connectedNodes.Count; i++) {
			if (!newVisited.Contains (connectedNodes[i])) {
				if (connectedNodes [i] == null)
					continue;
				List <NodeDistance> newDistances = connectedNodes [i].nodesDistances (newVisited, weight + Vector3.Distance (transform.position, connectedNodes[i].transform.position), this);
				for (int j = 0; j < newDistances.Count; j++) {
					int index;
					if (TargetContained (distances, newDistances[j].nodes.targetNode, out index)) {
						if (distances[index].distance > newDistances[j].distance)
							distances[index] = newDistances [j];
					} else {
						distances.Add (newDistances[j]);
					}
				}
			}
		}
		if (caller != this)
			for (int i = 0; i < distances.Count; i++)
				distances [i].nodes.nextNode = this;
		//counter++;
		return distances;
	}
	public static bool TargetContained (List <NodeDistance> distances, Node mainTarget, out int index, bool targetNode = true) {
		for (int i = 0; i < distances.Count; i++) {
			if ((targetNode ? distances [i].nodes.targetNode : distances[i].nodes.nextNode) == mainTarget) {
				index = i;
				return true;
			}
		}
		index = -1;
		return false;
	}
	#region Gizmos
	void OnDrawGizmos () {
		if (connectedNodes != null)
			for (int i = 0; i < connectedNodes.Count; i++) {
				if (connectedNodes[i] != null) {
					Gizmos.DrawLine (transform.position, connectedNodes[i].transform.position);
				}
			}
	}
	#endregion
}
