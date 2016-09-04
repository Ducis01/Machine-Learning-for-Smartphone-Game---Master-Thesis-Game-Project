using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShieldTextUpdate : MonoBehaviour {

	public Skeleton _player;
	public bool ShieldOrAmmo = true;

	public GameObject Anim1;

	private Text t;
	private int previousP =0;
	private float elapse;

	void Start () {

		t = GetComponent<Text> ();
		previousP = ShieldOrAmmo ? 9 : 0;
	}

	
	// Update is called once per frame
	void Update () {

		t.text = ShieldOrAmmo ? _player.PlayerShieldHP.ToString () : _player.Power.ToString ();



		if (_player.Power > previousP && elapse < 1.5f) {

			t.text = ShieldOrAmmo ? _player.PlayerShieldHP.ToString () : _player.Power.ToString ();

			elapse = 0.0001f;
			previousP =  ShieldOrAmmo ? _player.PlayerShieldHP : _player.Power;

			if (Anim1) {
				Anim1.SetActive (true);
			}

		} else if (elapse > 0f && elapse < 1.5f) {

			if (!ShieldOrAmmo)
				t.text = "+1";
			//t.color = Color.red;

			elapse += Time.deltaTime;

		} else if (elapse >= 1.5f) {

			t.text = ShieldOrAmmo ? _player.PlayerShieldHP.ToString () : _player.Power.ToString ();
			t.color = Color.white;

			elapse = 0;
			if (Anim1) {
				Anim1.SetActive (false);
			}

		}

		if (elapse == 0 || previousP > _player.Power) {

			previousP = ShieldOrAmmo ? _player.PlayerShieldHP : _player.Power;;
		}
	
	}
}
