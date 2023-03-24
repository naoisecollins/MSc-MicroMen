using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f; //variable used to show the time it takes for the camera to get to position                
    public float m_ScreenEdgeBuffer = 4f; // variable added to the sides to keep the players on the screen           
    public float m_MinSize = 6.5f; // variable to make sure the camera doesn't zoom too close                 
   [HideInInspector]  public Transform[] m_Targets; //array of transforms of all the players


    private Camera m_Camera; //reference to the camera                       
    private float m_ZoomSpeed; //reference to how fast the camera is zooming                      
    private Vector3 m_MoveVelocity; //reference to how fast the camera is moving                  
    private Vector3 m_DesiredPosition; //position that the camera is trying to reach              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>(); //finds the camera in the cameraRig
    }


    private void FixedUpdate()//calls move and zoom functions of the camera updating at the same time as the players 
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();//finds the average position and sets the desired position to it 

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);//smoothly moves the camera between the desired position and the current position, takes in how fast its moving and resets to new speed. damp time sets how long it takes 
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3(); //creates new blank vector3
        int numTargets = 0; //number of targets averaging over

        for (int i = 0; i < m_Targets.Length; i++) //for loop loops while i is less than targets length
        {
            if (!m_Targets[i].gameObject.activeSelf) // continues to next entry in the loop if target is not active
                continue; 

            averagePos += m_Targets[i].position; //adds players position to average position
            numTargets++; //incremented by number of players currently
        }

        if (numTargets > 0) //if number of targets are greater than zero divide average position by how many there are 
            averagePos /= numTargets;

        averagePos.y = transform.position.y; //keeps the y position frozen at 0

        m_DesiredPosition = averagePos; //set the desired position to the average position
    }


    private void Zoom()//zoom the camera to the player
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
	} ////smoothly zooms the camera between the orthographicSize and the required size, takes in how fast its moving and resets to new speed. damp time sets how long it takes 


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);//finding the position of the camera in the camera rig's local space

        float size = 0f; //size variable 

		for (int i = 0; i < m_Targets.Length; i++) //loop through all the targets
        {
            if (!m_Targets[i].gameObject.activeSelf) //if target is not active continue to the next
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); //finding target in the local position of the cameraRig

			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;//Find the position of the target from the desired position of the camera's local space.

			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y)); // Choose the largest out of the current size and the distance of the player from the camera.

			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect); // Choose the largest out of the current size and size of the player being right or left on the camera.
        }
        
        size += m_ScreenEdgeBuffer; //adds a buffer

        size = Mathf.Max(size, m_MinSize); //doesn't let the camera size set below a minimum

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition(); //find the position

        transform.position = m_DesiredPosition; //set camera position to desired position without damping 

        m_Camera.orthographicSize = FindRequiredSize(); //find and set size of camera
    }
}