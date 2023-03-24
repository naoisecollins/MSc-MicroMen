using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	

		public int m_NumRoundsToWin = 5;  // number of rounds to win the game       
		public float m_StartDelay = 3f;   //delay between the round starting      
		public float m_EndDelay = 3f;     //delay at the end of the round       
		public CameraControl m_CameraControl;   //reference to the camera control for use between rounds 
		public Text m_MessageText;            //reference to the winning text   
		public GameObject m_TankPrefab;         //reference to the prefab the player controls 
		public TankManager[] m_Tanks;         //managers for enabling and disabling the players   


		private int m_RoundNumber;              //round number that the game is on 
		private WaitForSeconds m_StartWait;     //delay for when the round starts
		private WaitForSeconds m_EndWait;       //delay for the round ending 
		private TankManager m_RoundWinner;        //references who wins a specific round
		private TankManager m_GameWinner;       //references who wins the game 


		private void Start()
		{
			//creates the delays for the game 
			m_StartWait = new WaitForSeconds(m_StartDelay);
			m_EndWait = new WaitForSeconds(m_EndDelay);

			SpawnAllTanks();
			SetCameraTargets();


			StartCoroutine(GameLoop()); //once the players are spawned and the camera is reset the game begins
		}


		private void SpawnAllTanks()
		{
			for (int i = 0; i < m_Tanks.Length; i++) //for all the players 
			{
				m_Tanks [i].m_Instance = 
					Instantiate (m_TankPrefab, m_Tanks [i].m_SpawnPoint.position, m_Tanks [i].m_SpawnPoint.rotation) as GameObject; //create them in their specific spawnpoint
				m_Tanks[i].m_PlayerNumber = i + 1; //set the number of the player
				m_Tanks[i].Setup();
			}
		}


		private void SetCameraTargets()
		{
			Transform[] targets = new Transform[m_Tanks.Length]; //creates a transform for each player 

			for (int i = 0; i < targets.Length; i++) //for each transform of a player 
			{
				targets[i] = m_Tanks[i].m_Instance.transform; //set it to the same size as the player
			}

			m_CameraControl.m_Targets = targets; //set the camera to follow these targets 
		}


		private IEnumerator GameLoop() //waits for the phase before to finish and runs through each phase of the game
		{
			yield return StartCoroutine(RoundStarting()); //runs the "roundstarting" coroutine but doesn't return until its finished
			yield return StartCoroutine(RoundPlaying()); //once the previous coroutine is finished it runs "Roundplaying" but doesn't return till finished
			yield return StartCoroutine(RoundEnding()); //once the previous coroutine is finsihed it plays the "roundEnding" coroutine but doesn't return till its finished 

			if (m_GameWinner != null)//after these coroutines it checks for a game winner if one is found
			{
				SceneManager.LoadScene(0); //restart the level
			}
			else
			{
				StartCoroutine(GameLoop()); //if there isnt a winner restart the coroutine from the beginning 
			}
		}


		private IEnumerator RoundStarting()
		{
			ResetAllTanks (); //as soon as round starts restart the players 
			DisableTankControl(); //stops the players from being able to move 

			m_CameraControl.SetStartPositionAndSize (); //resets the camera control to the normal size and position 

			m_RoundNumber++; //increment the round 
			m_MessageText.text = "Round " + m_RoundNumber; //show the text showing which roundnumber is playing
			yield return m_StartWait; // wait a specific delay until starting the next round 
		}


		private IEnumerator RoundPlaying()
		{

			EnableTankControl ();//at the start of the round lets players control their character


			m_MessageText.text = string.Empty; //Clears message text


			while (!OneTankLeft ()) {//While there is more than one player left 


				yield return null;     //return on the next frame 
			}
		}


		private IEnumerator RoundEnding()
		{
			DisableTankControl (); //stops the players moving 


			m_RoundWinner = null; //clears the last winner 


			m_RoundWinner = GetRoundWinner ();//See if there is a winner at end of round 


			if (m_RoundWinner != null) 
				m_RoundWinner.m_Wins++;//if winner increment their score


			m_GameWinner = GetGameWinner ();     //check if someone won the game 

			//
			string message = EndMessage (); 
			m_MessageText.text = message; //get a message of scores and if there is a game winner


			yield return m_EndWait;//wait a certain amount of time until going through coroutine again 
		}


		private bool OneTankLeft()//to check if there is only one player left to end the round 
		{

			int numTanksLeft = 0;//start the count of players at 0 


			for (int i = 0; i < m_Tanks.Length; i++)//go through each player 
			{

				if (m_Tanks[i].m_Instance.activeSelf)
					numTanksLeft++;//if they are active increment 
			}

			return numTanksLeft <= 1;//if one or less players return true
		}


		private TankManager GetRoundWinner()//to find out if there is a winner to the round
		{

			for (int i = 0; i < m_Tanks.Length; i++)//go through all the players 
			{

				if (m_Tanks[i].m_Instance.activeSelf)//if one active it is the winner 
					return m_Tanks[i];
			}

			return null; //if none active return null 
		}


		private TankManager GetGameWinner()//check for a winner in the game 
		{
			for (int i = 0; i < m_Tanks.Length; i++)// go through all the players
			{
				if (m_Tanks[i].m_Wins == m_NumRoundsToWin) //if active it is the winner return it
					return m_Tanks[i];
			}

			return null; //if none active return null 
		}


		private string EndMessage()// returns a string at the end of a round
		{
			string message = "DRAW!"; //default message is draw 

			if (m_RoundWinner != null)
				message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";//if the round winner is not null return the player who wins the round with string

			message += "\n\n\n\n"; //adds line breaks 

			for (int i = 0; i < m_Tanks.Length; i++) //go through all the players 
			{
				message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n"; //adds each of their scores to the message 
			}

			if (m_GameWinner != null)
				message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!"; //if there is a game winner show the winner with a string 

			return message;
		}


		private void ResetAllTanks() //function to turn on the players and reset positions 
		{
			for (int i = 0; i < m_Tanks.Length; i++)
			{
				m_Tanks[i].Reset();
			}
		}


		private void EnableTankControl()
		{
			for (int i = 0; i < m_Tanks.Length; i++)
			{
				m_Tanks[i].EnableControl();
			}
		}


		private void DisableTankControl()
		{
			for (int i = 0; i < m_Tanks.Length; i++)
			{
				m_Tanks[i].DisableControl();
			}
		}
	} 
