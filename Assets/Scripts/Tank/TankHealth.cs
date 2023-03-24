using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f; //starting health set to 100         
    public Slider m_Slider; //slider handles the values of player health                
    public Image m_FillImage; //image of the slider               
    public Color m_FullHealthColor = Color.green;  //colour set at full health
    public Color m_ZeroHealthColor = Color.red;    //colour set at no health 
    public GameObject m_ExplosionPrefab; //public object set to use when player dies
    
    
    private AudioSource m_ExplosionAudio; //audio sound at death          
    private ParticleSystem m_ExplosionParticles; //particle system at death  
    private float m_CurrentHealth;  //current health of player
    private bool m_Dead; // bool set to decide if player is alive or dead           


    private void Awake()
    {
		m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>(); //create an instance of explosionprefab and assigning particle system to it 
		m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>(); //create an instance of explosionprefab and assigning an audiosource to it 

        m_ExplosionParticles.gameObject.SetActive(false); //inactive in the hierachy for the beginning of the game
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth; //reset health to starting health 
        m_Dead = false; //sets it to being alive

        SetHealthUI(); //updates the UI to the value needed
    }
    

    public void TakeDamage(float amount) //the damage taken by the player
    {
        
		m_CurrentHealth -= amount; // Adjust the player's current health

		SetHealthUI ();// update the UI based on the new health

		if (m_CurrentHealth <= 0f && !m_Dead) {
			OnDeath (); //check whether or not the tank is dead.
		}

	}


    private void SetHealthUI()
    {
         
		m_Slider.value = m_CurrentHealth; //sets slider value as health value 

		m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth); //sets the color of the slider to fill to health full and health empty values, divides health full with health empty to set colour for part damage
	}


    private void OnDeath()
    {
		m_Dead = true; //sets dead to true
        // Play the effects for the death of the tank and deactivate it.
		m_ExplosionParticles.transform.position = transform.position; //move particles to your position
		m_ExplosionParticles.gameObject.SetActive (true); //turns on explosionparticles

		m_ExplosionParticles.Play (); //plays particle effect

		m_ExplosionAudio.Play (); //plays particle audio

		gameObject.SetActive (false); //turns off the player 
	}
}