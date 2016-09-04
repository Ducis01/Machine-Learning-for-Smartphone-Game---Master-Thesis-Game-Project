using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseOrPlay : MonoBehaviour {

	public GameObject Action;
	public GameObject Other;
	public bool RunOrNot; 

	void OnMouseDown(){
		// this object was clicked - do something

		Action.SetActive (!Action.activeSelf);
		Other.SetActive (true);
		Time.timeScale = RunOrNot ? 1 : 0;

		gameObject.SetActive (false);
	}


}
