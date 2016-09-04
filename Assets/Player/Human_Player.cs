/*
 * Human Player Definition.
 * Author: Nicolas Van Wallendael <nicolas.vanwallendael@student.uclouvain.be>
 * Antoine Van Malleghem  <antoine.vanmalleghem@student.uclouvain.be>
 * Copyright (C) 2015, Université catholique de Louvain
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Human_Player : Skeleton {

	private Agent _ennemy;
	public GameObject AnimationShield;
	public int taunts;

	public GameObject EndBox;
	public GameObject EndText;
	private ScoreKeeper Score;

	private float elapse = 0f;
	private bool new_action;

	private static string _victory = "Bouyaa WIN!!\n\nScore : ";
	private static string _defeat  = "Ohhh! We lost.\n\nScore : ";
	private static string _tie	   = "It's a tie boyz !!\n\nScore : ";

	// Use this for initialization
	protected override void Start () {

		// Update variables 
		player_name = "Player";

		// Get ennemy
		_ennemy = (Agent) GameObject.FindObjectOfType(typeof(Agent));
		Score = (ScoreKeeper) GameObject.FindObjectOfType(typeof(ScoreKeeper));

		// Call parent
		base.Start ();


	}

	// Update is called once per frame
	void Update () {

		if (new_action && _lastAction == Actions.SHIELD && elapse == 0) { 		

			elapse += Time.deltaTime;
			new_action = false;

		} else if (elapse > 0 && elapse < 1.5f) {

			elapse += Time.deltaTime;
			AnimationShield.SetActive (true);
			AnimationShield.GetComponent<Animator> ().Play (
				PlayerShieldHP > 6 ? "ShieldShipBig" : (PlayerShieldHP > 3 ? "ShieldShipMedium" : "ShieldShipSmall"));

			if (PlayerHP < 0) {

				AnimationShield.GetComponent<BoxCollider2D> ().isTrigger = true;
			}

		} else {
			AnimationShield.SetActive (false);

			elapse = 0;
		}


	}

	IEnumerator WaitForAnimation () {

		yield return new WaitForSeconds(1f);
		print("STOP WAITING");
		_play = false;

	}


	public void reloadGame () {

		// Better to reset Agents. - In fact no need !
		Time.timeScale = 1;
		SceneManager.LoadScene ("Game");
	}

	public void loadMenu () {

		Time.timeScale = 1;
		SceneManager.LoadScene ("MainMenu");
	}

	public bool checkGameOver() {
		return (PlayerHP <= 0 || _ennemy.PlayerHP <= 0);
	}

	IEnumerator isGameOver () {

		if ( checkGameOver() ) {

			Score.gameNumber += 1;
			Score.tieTotal += (PlayerHP <= 0 && _ennemy.PlayerHP <= 0) ? 1 : 0;
			Score.winAITotal += (PlayerHP <= 0 && _ennemy.PlayerHP > 0) ? 1 : 0;

			print (Score.gameNumber + " " + Score.tieTotal + " " + Score.winAITotal);

			yield return new WaitForSeconds (1.5f);
			EndText.transform.GetChild(0).GetComponent<Text>().text = 
				((PlayerHP <= 0 && _ennemy.PlayerHP <= 0) ? _tie : (PlayerHP <= 0 ? _defeat : _victory)) + (100 - Score.winRate) +
				" %\n\n";
			EndBox.SetActive (true);
			EndText.SetActive (true);
			Time.timeScale = 0;
		}

		yield break;
	}

	public void tauntAlien () {

		// 0 is Taunt placeholder.
		int i = Random.Range (1, taunts+1);
		int j = 0;

		foreach (Transform child in transform.GetComponentsInChildren<Transform>(true)) {
			child.gameObject.SetActive (i==j || j==0);
			j++;
		}
		print (j);
	}

	// Call the ennemy action before taking our
	// TODO : Verfify that we can play the action

	public override void Shoot( Actions shoot, Rigidbody2D weapon, int direction) {

		_play = true;
		StartCoroutine (WaitForAnimation ());

		_lastAction = shoot;
		new_action = true;
		tauntAlien ();

		// call ennemy
		//Coroutine computeAIMove = 
		StartCoroutine( _ennemy.Play() );

		// Move to Position on screen ;


		// Wait end of animation and end of Move compuatation.



		// play action chosen
		base.Shoot (shoot, weapon, direction);				// Lower Dmg
		_ennemy.takeDamage (shoot);		// Touch HP
		_ennemy.whatDidYouPlay();
		print ("STATUS = " + PlayerShieldHP + "|" + Power + "|" + _ennemy.PlayerShieldHP + "|" + _ennemy.Power + " >> " + PlayerHP + _ennemy.PlayerHP);

		StartCoroutine( isGameOver ());
	}


	public override void Reload () {

		_play = true;
		StartCoroutine (WaitForAnimation ());

		_lastAction = Actions.RELOAD;
		new_action = true;
		tauntAlien ();

		// call ennemy
		StartCoroutine( _ennemy.Play () );

		// play action chosen
		base.Reload();
		_ennemy.whatDidYouPlay();
		print ("STATUS = " + PlayerShieldHP + "|" + Power + "|" + _ennemy.PlayerShieldHP + "|" + _ennemy.Power + " >> " + PlayerHP + _ennemy.PlayerHP);

		StartCoroutine(isGameOver ());
	}


	public override void Shield () {

		_play = true;
		StartCoroutine (WaitForAnimation ());

		_lastAction = Actions.SHIELD;
		new_action = true;
		tauntAlien ();

		// call ennemy
		StartCoroutine( _ennemy.Play () );

		// play action chosen
		base.Shield ();
		_ennemy.whatDidYouPlay();
		print ("STATUS = " + PlayerShieldHP + "|" + Power + "|" + _ennemy.PlayerShieldHP + "|" + _ennemy.Power + " >> " + PlayerHP + _ennemy.PlayerHP);

		StartCoroutine(isGameOver ());
	}
}
