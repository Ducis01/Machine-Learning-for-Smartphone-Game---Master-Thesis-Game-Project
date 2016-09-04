/*
 * General Player Definition.
 * Author: Nicolas Van Wallendael <nicolas.vanwallendael@student.uclouvain.be>
 * 		   Antoine Van Malleghem  <antoine.vanmalleghem@student.uclouvain.be>
 * Copyright (C) 2015, Université catholique de Louvain
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is the skeleton of an agent to play the Cowboy game.
public class Skeleton : MonoBehaviour {

	// Can we play a new round ?
	[HideInInspector]
	public bool _play = false;

	// Define the power of each shot.
	public enum Actions{SHOOT_1, SHOOT_3, SHOOT_5, SHOOT_10, RELOAD, SHIELD};
	public static Dictionary<Actions, int> SHOOT_POWER = new Dictionary<Actions, int> (){
		{Actions.SHOOT_1, 1},
		{Actions.SHOOT_3, 3},
		{Actions.SHOOT_5, 5},
		{Actions.SHOOT_10,10}
	};

	public static Dictionary<string, Actions> PATTERNS_TO_ACTIONS = new Dictionary<string, Actions> (){
		{"r", Actions.RELOAD},
		{"s", Actions.SHIELD},
		{"1", Actions.SHOOT_1},
		{"3", Actions.SHOOT_3},
		{"5", Actions.SHOOT_5},
		{"b", Actions.SHOOT_10}
	};

	public static Dictionary<Actions, string> ACTIONS_TO_PATTERN = new Dictionary<Actions, string> (){
		{Actions.RELOAD,  "r"},
		{Actions.SHIELD,  "s"},
		{Actions.SHOOT_1, "1"},
		{Actions.SHOOT_3, "3"},
		{Actions.SHOOT_5, "5"},
		{Actions.SHOOT_10,"b"}
	};
		
	// CST
	protected int MAX_POWER		  = 10; 
	protected int MAX_SHIELD_HP	  = 9;
	protected int MIN_SHIELD_HP	  = 0;


	// Basic stats
	protected int _power    	  = 0;
	protected int _playerHP       = 1;
	protected int _playerShieldHP = 9;

	// Value Storage
	protected Actions _lastAction;

	// Agent initialyser
	public string player_name = "Skeleton Agent";
	public string player 	  = "player1";

	public Dictionary<string, int> arg = new Dictionary<string, int>();

	// Setters and getters 
	public bool isBusy {
		get { return _play; }
		// no outside set !
	}

	public int Power {
		get { return _power; }
		// no outside set !
	}

	public int PlayerHP {
		get { return _playerHP; }
		// no outside set !
	}

	public int PlayerShieldHP {
		get { return _playerShieldHP; }
		// no outside set !
	}

	public Actions LastAction {
		get { return _lastAction; }
		// no outside set !
	}


	// Unity things
	protected virtual void Start ()
	{
		// Create the object
		toString();
	}


	public virtual void toString(){
		
		Debug.Log ("I'm a Skeleton instance with name : " + player_name);
	}

	public void takeDamage ( Actions shoot ) {

		if (_lastAction == Actions.SHIELD) {

			_playerShieldHP -= SHOOT_POWER [shoot];
			_playerHP += _playerShieldHP > 0 ? 0 : _playerShieldHP;
			_playerShieldHP = _playerShieldHP >= 0 ? _playerShieldHP : 0;
		} else {

			_playerHP -= SHOOT_POWER [shoot];
		}
	}

	// Update player value followinf input actions 
	public virtual void Shoot( Actions shoot, Rigidbody2D weapon, int direction ) {

		if (canShoot (shoot)) {

			_power -= SHOOT_POWER [shoot];

			// Animate Weapon
			if (weapon) {
				Rigidbody2D rocketInstance = Instantiate(weapon, weapon.position, Quaternion.Euler(new Vector3(0,0,10))) as Rigidbody2D;
				rocketInstance.velocity = new Vector2(direction * Shoot_Spawner.WeaponSpeed, Shoot_Spawner.WeaponSpeed * 75/90);
			}

			
			switch (shoot) {
			case Actions.SHOOT_1:
				print (name + " Little scrach after little scrach i'll get you !");
				break;
			case Actions.SHOOT_3:
				print (name + " Take that XD !");
				break;
			case Actions.SHOOT_5:
				print (name + " Die motherfucker !!");
				break;
			case Actions.SHOOT_10:
				print (name + " GOOOING for the oneshot =D !");
				break;
			default:
				print (name + " *Lifting your hands up*, i cant do that man =/.");
				break;
			}
		} else {
			print (name + " *Lifting your hands up*, i cant do that man =/.");
		}				
	}

	public bool canShoot ( Actions shoot ) {

		return ( _power >= SHOOT_POWER [shoot] );
	}

	public bool canShield ( ) {

		return ( _playerShieldHP > MIN_SHIELD_HP );
	}

	public bool canReload ( ) {

		return ( _power < MAX_POWER );
	}

	public virtual void Reload () {

		//play reload animation

		if (_power < MAX_POWER) {
			print ("Reload +1, niark niark !" + _power);
			_power += 1;
		} else {
			print("MAX power reached");
		}
	}

	public virtual void Shield () {

		if (_playerShieldHP > 0) {
			//play shield animation
			print (name + " Captain america here =D !" + _playerShieldHP);
		} else {

			print (name + " OUTTA SHIELD MAN ! ");
		}
	}

	public virtual List<Actions> getPossibleActions () {

		List<Actions> possible_actions = new List<Actions>();

		foreach (Actions action in SHOOT_POWER.Keys) {

			if (canShoot (action)) {
				possible_actions.Add (action);	
			}
		}

		if ( canReload() )
			possible_actions.Add (Actions.RELOAD);

		if ( canShield() )
			possible_actions.Add (Actions.SHIELD);


		return possible_actions;
	}
}
