//----------------------------------------------------------------------------------------------
// Object with this script hold waypoints as path and visualize it
// If list of waypoints is empty - Script will try to gather all child objects as waypoints on start
//----------------------------------------------------------------------------------------------using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaypointsHolder : MonoBehaviour 
{
	public Color color = new Color (0, 1, 0, 0.5f); 		// Debug path lines color
	public List<Waypoint> waypoints = new List<Waypoint>();	// List of all waypoints assigned to this path
	public bool colorizeWaypoints = true;   				// Repaint all waypoints in the color


	//=============================================================================================================
	// If list of waypoints is empty - try to gather all child objects(with waypoint script attached) as waypoints
	void Awake () 
	{
		if (waypoints == null  ||  waypoints.Count == 0)
		{
			Waypoint[] childrenWaypoints = GetComponentsInChildren<Waypoint>();
			foreach (Waypoint waypoint in childrenWaypoints) 
				AddWaypoint(waypoint);
		}

		Clean ();
	}


    // Creates a line renderer that follows a Sin() function
    // and animates it.

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
   

    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.positionCount = waypoints.Count;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;
    }

    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var t = Time.time;
        for (int i = 0; i < waypoints.Count; i++)
        {
            lineRenderer.SetPosition(i,waypoints[i].gameObject.transform.position);
        }
    }


    //----------------------------------------------------------------------------------
    //Remove missing Waypoints
    public void Clean () 
	{
		for (int i = 0; i < waypoints.Count; i++)
			if (waypoints[i] == null)
			{
				waypoints.RemoveAt (i);
				i--;
			}
	}

	//----------------------------------------------------------------------------------
	// Add existing waypoint to the end of path
	public void AddWaypoint (Waypoint _newWaypoint) 
	{
		if (colorizeWaypoints) 
			_newWaypoint.color = color;
		
		waypoints.Add (_newWaypoint);
	}

	//----------------------------------------------------------------------------------
	// Create new waypoint in  specified coordinates and add it to the end of path
	public void CreateWaypoint (Vector3 _position, string name = "waypoint") 
	{
		GameObject newWaypoint = new GameObject();
		newWaypoint.name = name;
		newWaypoint.transform.parent = transform;
		newWaypoint.transform.position = _position;

		AddWaypoint (newWaypoint.AddComponent<Waypoint>());
	}

	//----------------------------------------------------------------------------------
	// Draw debug visualization
	//void OnDrawGizmos() 
	//{
	//	Gizmos.color = color;
        

 //       if (waypoints.Count > 0)
	//		for (int i = 0; i<(waypoints.Count-1); i++)
	//			if (waypoints[i] && waypoints[i+1])  
	//			{
	//				Gizmos.DrawLine (waypoints[i].gameObject.transform.position, waypoints[i+1].gameObject.transform.position);
	//				if (colorizeWaypoints) 
	//					waypoints[i+1].color = color;
	//			}
	//}

	//----------------------------------------------------------------------------------
}