/*
 * Shooting spawner Definition.
 * Author: Nicolas Van Wallendael <nicolas.vanwallendael@student.uclouvain.be>
 * Antoine Van Malleghem  <antoine.vanmalleghem@student.uclouvain.be>
 * Copyright (C) 2015, Université catholique de Louvain
 */

using UnityEngine;
using System.Collections;

public class Reload_Spawner : MonoBehaviour
{
	public Human_Player _player;
	public GameObject circle;


	// Update is called once per frame
	void Update() {
		if (_player.Power >= 10)
			circle.SetActive (false);
		else
			circle.SetActive(true);

	}


	void OnMouseDown(){
		// this object was clicked - do something

		//int power = player.Power % Skeleton.SHOOT_POWER[shoot_power];

		if (!_player.isBusy && _player.canReload () && !_player.checkGameOver ()) {
			_player.Reload ();
		}
	}

}

