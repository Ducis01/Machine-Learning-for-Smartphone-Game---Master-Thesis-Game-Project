using UnityEngine;
using System.Collections;

public class MovePanelSpace : MonoBehaviour {

	public int direction = 1;
	public int directionC = 1;
	public float speed = 0.5f;
	public float speedC = 0.5f;
	public GameObject canon;

	private Agent _ennemy;
	public int shoot_power = 1;

	private float elapse = 0f;

	// Use this for initialization
	void Start ()
	{
		_ennemy = (Agent) GameObject.FindObjectOfType(typeof(Agent));
	}

	// Update is called once per frame
	void Update () {

		if (_ennemy.Power >= shoot_power && elapse < 1.5f) {
			elapse += Time.deltaTime;
			transform.Translate (Vector3.up * direction * speed * Time.deltaTime, Space.World);
			canon.transform.Translate(Vector3.right * directionC * speedC * Time.deltaTime, Space.World);

		} else if (_ennemy.Power < shoot_power && elapse > 0) {
			elapse -= Time.deltaTime;
			transform.Translate (Vector3.up * -direction * speed * Time.deltaTime, Space.World);
			canon.transform.Translate(Vector3.right * -directionC * speedC * Time.deltaTime, Space.World);

		}
	}
}
