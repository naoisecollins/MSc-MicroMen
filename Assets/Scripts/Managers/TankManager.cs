using System;
using UnityEngine;

[Serializable]
public class TankManager
{
	public Color m_PlayerColor;  //colour of the player           
	public Transform m_SpawnPoint; //position and direction of the player when spawn         
	[HideInInspector] public int m_PlayerNumber; //which player it is managing             
	[HideInInspector] public string m_ColoredPlayerText; //string to represent player with their player character
	[HideInInspector] public GameObject m_Instance;  //reference to the player controller         
	[HideInInspector] public int m_Wins;  //number of wins the player has                    


	private TankMovement m_Movement;     //reference to movement script  
	private TankShooting m_Shooting;    //reference to shoot script
	private GameObject m_CanvasGameObject;    // to disable and enable UI of the rounds 


	public void Setup()
	{
		m_Movement = m_Instance.GetComponent<TankMovement> (); //Get the reference to player movement
		m_Shooting = m_Instance.GetComponent<TankShooting> ();// Get the reference to the player shooting
		m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas> ().gameObject;//Get the reference to the canvas with the ui for the rounds

		m_Movement.m_PlayerNumber = m_PlayerNumber;//setting the player number on the movement script
		m_Shooting.m_PlayerNumber = m_PlayerNumber;//setting the player number on the shooting script 

		m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB (m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";//makes a string with the right colour from the player saying the player name and colour 

		SkinnedMeshRenderer[] renderers = m_Instance.GetComponentsInChildren<SkinnedMeshRenderer> (); //get the player renderers
		for (int i = 0; i < renderers.Length; i++) { // Go through all the renderers 
			renderers [i].material.color = m_PlayerColor; // set their material color to the color specific to this player.

		}
	}

	public void DisableControl() //disables control of the player when the player shouldn't be able to use it 
	{
		m_Movement.enabled = false;
		m_Shooting.enabled = false;

		m_CanvasGameObject.SetActive(false);
	}


	public void EnableControl() // enables the control of the player when the player should be able to use it 
	{
		m_Movement.enabled = true;
		m_Shooting.enabled = true;

		m_CanvasGameObject.SetActive(true);
	}


	public void Reset() //resets the position of the player and their rotation and spawns them 
	{
		m_Instance.transform.position = m_SpawnPoint.position;
		m_Instance.transform.rotation = m_SpawnPoint.rotation;

		m_Instance.SetActive(false);
		m_Instance.SetActive(true);
	}
}
