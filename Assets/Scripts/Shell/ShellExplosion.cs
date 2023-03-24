using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; //chooses which layer to effect
    public ParticleSystem m_ExplosionParticles; //play the particles when bullet explodes                
    public float m_MaxDamage = 100f; //max damage possible to do                  
    public float m_ExplosionForce = 1000f; //amount of force of the explosion           
    public float m_MaxLifeTime = 2f; //max lifetime of the bullet                  
    public float m_ExplosionRadius = 5f; //how fire effect the player when it explodes             


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime); //after two seconds destroy this object
    }


    private void OnTriggerEnter(Collider other) //occurs when collides with an object
	{
		// Find all the tanks in an area around the shell and damage them.
		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);//creates a sphere that collects information from the layer of the players  
	
		for (int i = 0; i < colliders.Length; i++) {//for loop to loop through all the colliders 
			Rigidbody targetRigidbody = colliders [i].GetComponent<Rigidbody> ();//searches for colliders with rigidbodies
		
			if (!targetRigidbody) //if the target has no rigidbody continue to the next
				continue;

			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius); //adds force so players move when hit

			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> (); //find the tankhealth script in the rigidbody

			if (!targetHealth)//if no tankhealth go to next gameobject
				continue;

			float damage = CalculateDamage (targetRigidbody.position);//calculate the damage the player should take based on distance

			targetHealth.TakeDamage (damage); //deal this damage to the player 
		}
	
		m_ExplosionParticles.transform.parent = null;//take the particles away from being the parent of the bullet
	
		m_ExplosionParticles.Play();//play the particle system

		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration); //destroy the gameobject of the particles when they finish
	
		Destroy (gameObject); //Destroy the bullet

	}

    private float CalculateDamage(Vector3 targetPosition)
    {
		Vector3 explosionToTarget = targetPosition - transform.position; //create a vector from the target to the bullet

		float explosionDistance = explosionToTarget.magnitude;//find the distance of the bullet to the player

		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; //find the proportion of the max distance the player is away
       
		float damage = relativeDistance * m_MaxDamage;//calculate the damage from this distance

		damage = Mathf.Max (0f, damage); //min damage is always zero

		return damage; 

       
    }
}