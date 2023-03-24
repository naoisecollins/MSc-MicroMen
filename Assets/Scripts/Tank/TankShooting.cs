using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;  //to identify the player     
    public Rigidbody m_Shell; //reference to the bullet            
    public Transform m_FireTransform;  //where bullets spawn  
    public Slider m_AimSlider; //displays the force           
    public AudioSource m_ShootingAudio;  //reference to shooting audio
    public AudioClip m_ChargingClip; // reference to charging audio    
    public AudioClip m_FireClip; //reference to firing audio        
    public float m_MinLaunchForce = 15f; // the min force of the shot
    public float m_MaxLaunchForce = 30f; // the max force of the shot
    public float m_MaxChargeTime = 0.75f; // how long the shot can be charged for 

   
    private string m_FireButton; //input axis for bullets         
    private float m_CurrentLaunchForce;  //force given to shell when fire is let go 
    private float m_ChargeSpeed; //how fast launch increases based on charge         
    private bool m_Fired; //bool to decide if the shell has been launched               


    private void OnEnable()
    {
		//resets the launch force and aim slider to min values 
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
		//fire button decided based on player number 
        m_FireButton = "Fire" + m_PlayerNumber;
		//The rate of charge of the launch force = the possible force by max charge
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
   

    private void Update()
	{
		//default value of slider is min launch force
		m_AimSlider.value = m_MinLaunchForce;

		//if it has moved past max force and bullet hasnt fired
		if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
			//use max force to launch the shell 
			m_CurrentLaunchForce = m_MaxLaunchForce;
			Fire ();
		}
		//if fire button just being pressed 
		else if (Input.GetButtonDown (m_FireButton)) {	
			//reset fired and force of launch
			m_Fired = false;
			m_CurrentLaunchForce = m_MinLaunchForce;

			//starting charging audio
			m_ShootingAudio.clip = m_ChargingClip;
			m_ShootingAudio.Play ();

		}
	//if fire held and bullet not fired 
	else if (Input.GetButton (m_FireButton) && !m_Fired) {
			//increment force and update the slider
			m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

			m_AimSlider.value = m_CurrentLaunchForce;
		} 
		//if fire released and bullet not fired
		else if (Input.GetButtonUp (m_FireButton) && !m_Fired) {

			//fire the bullet
			Fire ();
		}
	}

    private void Fire()
    {
		// Fire only called once
		m_Fired = true;

		//spawn bullet and get reference to its rigidbody 
		Rigidbody shellInstance =
			Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
		//set the velocity of the bullet to launch force in the fire position's forward direction
		shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ; 

		//play the firing clip
		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play (); 

		//reset launch force
		m_CurrentLaunchForce = m_MinLaunchForce;
    }
}