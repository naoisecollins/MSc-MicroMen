using UnityEngine;
using System.Collections;

public class TankMovement : MonoBehaviour
{
	Animator animator; 
    public int m_PlayerNumber = 1;      //number of the player script references   
    public float m_Speed = 12f;         //speed of player movement   
    public float m_TurnSpeed = 180f;    //how fast the player can turn   
    public AudioSource m_MovementAudio;   //audio that plays during movement 
    public AudioClip m_EngineIdling;      //audio that plays when player idle 
    public AudioClip m_EngineDriving;      //audio that plays when player moves
    public float m_PitchRange = 0.2f;		//the range of the pitch the audio can play at

   
    private string m_MovementAxisName;     //which movement axis is being referenced to move up and down
    private string m_TurnAxisName;         //which turn axis is referenced to rotate
    private Rigidbody m_Rigidbody;         //players rigidbody reference 
    private float m_MovementInputValue;    //the  current input value of the player for movement 
    private float m_TurnInputValue;        //the current turning input value
    private float m_OriginalPitch;         // the pitch of the audio in the beginning 


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>(); //references the rigidbody on awake 
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false; //makes sure the player is not kinematic
        m_MovementInputValue = 0f; //resets the movement input to 0 
        m_TurnInputValue = 0f; //resets the turning input to 0 
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true; //sets the player to kinematic when it is turned off 
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber; //the axis name is the player number and verticle axis
        m_TurnAxisName = "Horizontal" + m_PlayerNumber; //the axis name is the player number and horizontal axis
		animator = GetComponentInChildren<Animator>();
        m_OriginalPitch = m_MovementAudio.pitch; //stores the original pitch
    }
    

    private void Update()
    {
		
		m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);
		EngineAudio (); // Store the player's input and plays the audio .
	}


    private void EngineAudio()
	{
		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < .1f) {//if player not moving 
			if (m_MovementAudio.clip == m_EngineDriving) {//and audio source is playing driving clip 
				m_MovementAudio.clip = m_EngineIdling; //change to idle clip 
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play (); // Play the correct audio clip based on whether or not the player is moving and what audio is currently playing.
			}
		} else {
			if (m_MovementAudio.clip == m_EngineIdling) {//if idle is playing and player is moving
				m_MovementAudio.clip = m_EngineDriving; //change to the driving clip 
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange); //randomise the pitch
				m_MovementAudio.Play (); //play
			}
		}
	}


    private void FixedUpdate()
    {
      	
		Move();
		Turn (); // Move and turn the player.
		animator.SetFloat("vAxisInput", m_MovementInputValue);
		animator.SetFloat ("RInput", m_TurnInputValue);
    }


    private void Move()
    {
       
		Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; //decide on amount to move forward by input speed and frames
    	
		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);// Adjust the position of the tank based on the player's input.
	}


    private void Turn()
    {
        
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime; //decide on degrees to turn by input and time in frames and speed 
	
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f); // make rotation on y axis
	
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);//apply to the rigidbody rotation 
	}
}