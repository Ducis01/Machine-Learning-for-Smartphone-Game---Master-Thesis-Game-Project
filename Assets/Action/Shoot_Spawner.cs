/*
 * Shooting spawner Definition.
 * Author: Nicolas Van Wallendael <nicolas.vanwallendael@student.uclouvain.be>
 * Antoine Van Malleghem  <antoine.vanmalleghem@student.uclouvain.be>
 * Copyright (C) 2015, Université catholique de Louvain
 */

using UnityEngine;
using System.Collections;

public class Shoot_Spawner : MonoBehaviour
{
	public Human_Player _player;
	public Skeleton.Actions shoot_power;
	public Rigidbody2D weapon;

	public static float WeaponSpeed = 20f;

	void OnMouseDown(){
		// this object was clicked - do something

		//int power = player.Power % Skeleton.SHOOT_POWER[shoot_power];

		if (!_player.isBusy && _player.canShoot (shoot_power) && !_player.checkGameOver ()) {
			_player.Shoot (shoot_power, weapon, 1);

		}
	}

}

