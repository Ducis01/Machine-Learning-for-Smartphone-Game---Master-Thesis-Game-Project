using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Button play;
	public Button how;
	public Button credits;

	public GameObject loading;
	public Text loadingText;

	public GameObject howtoBox;
	public Button howtoButton;
	public Text howto1;
	public Text howto2;
	public Text howto3;
	public Text howto4;
	public Text howto5;

	public Button creditButton;
	public Text creditText;

	private ArrayList howtoList;

	private int howto = 0;
	private bool launchGame = false;
	private bool howToOn 	= false;

	private AsyncOperation async;

	// Use this for initialization
	void Start () {

		play = play.GetComponent<Button> ();
		how  = how.GetComponent<Button> ();
		credits = credits.GetComponent<Button> ();

		howtoList = new ArrayList ();
		howtoList.Add (howto1);
		howtoList.Add (howto2);
		howtoList.Add (howto3);
		howtoList.Add (howto4);
		howtoList.Add (howto5);


		StartCoroutine(loadLevel());

	}

	public void playGame () {

		launchGame = true;
		loading.SetActive (true);


		loadingText.gameObject.SetActive (true);

	}

	public void showHowTo () {

		if (howto == 0) {

			howToOn = true;
			switchUI ();

			((Text)howtoList [howto]).gameObject.SetActive (true);
			howto += 1;
		
		} else {

			((Text)howtoList [howto-1]).gameObject.SetActive (false);

			if (howto == 5) {
				switchUI ();
				howToOn = false;

			} else {
				((Text)howtoList [howto]).gameObject.SetActive (true);
			}

			howto += 1;
			howto %= 6;
		}

	}

	public void showCredits () {

		switchUI ();

	}



	private void switchUI () {

		play.gameObject.SetActive( !play.gameObject.activeSelf);
		how.gameObject.SetActive( !how.gameObject.activeSelf);
		credits.gameObject.SetActive( !credits.gameObject.activeSelf);

		howtoBox.SetActive (!howtoBox.activeSelf);
		if (howToOn)
			howtoButton.interactable = !howtoButton.interactable;
		else {
			creditText.gameObject.SetActive (!creditText.gameObject.activeSelf);
			creditButton.interactable = !creditButton.interactable;
		}
			
	}

	IEnumerator loadLevel() {

		yield return null;

		async = SceneManager.LoadSceneAsync("Game");
		async.allowSceneActivation = false;

		while (!async.isDone) {
			
			if (async.progress == 0.9f && launchGame) {
				async.allowSceneActivation = true;
			}

			yield return null;
		}
	}


}
