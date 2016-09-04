using UnityEngine;
using System.Collections;

public class MovePanel : MonoBehaviour {

	public int direction = 1;
	public int directionC = 1;
	public float speed = 35f;
	public float speedC = 0.5f;
	public bool moveUp = true;
	public GameObject canon;
	public GameObject circle;

	private Human_Player _player;
	public int shoot_power = 1;

	private float elapse = 0f;

	// Use this for initialization
	void Start ()
	{
		_player = (Human_Player) GameObject.FindObjectOfType(typeof(Human_Player));
	}

	// Update is called once per frame
	void Update () {

		if (_player.Power >= shoot_power && elapse < 2) {
			elapse += Time.deltaTime;
			transform.Rotate (Vector3.right * direction, speed * Time.deltaTime);
			canon.transform.Translate(Vector3.right * directionC * speedC * Time.deltaTime, Space.World);
			if (moveUp)
				canon.transform.Translate(Vector3.up * directionC * speedC * Time.deltaTime * 2f, Space.World);
			circle.SetActive (true);
		} else if (_player.Power < shoot_power && elapse > 0) {
			elapse -= Time.deltaTime;
			transform.Rotate (Vector3.right * -direction, speed * Time.deltaTime);
			canon.transform.Translate(Vector3.right * -directionC * speedC * Time.deltaTime, Space.World);
			if (moveUp)
				canon.transform.Translate(Vector3.up * -directionC * speedC * Time.deltaTime * 2f, Space.World);
			circle.SetActive (false);
		}
	}
}
