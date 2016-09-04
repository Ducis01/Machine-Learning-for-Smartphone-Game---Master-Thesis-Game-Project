/*
 * Agent Definition.
 * Author: Nicolas Van Wallendael <nicolas.vanwallendael@student.uclouvain.be>
 * Antoine Van Malleghem  <antoine.vanmalleghem@student.uclouvain.be>
 * Copyright (C) 2015, Université catholique de Louvain
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO; 
using System; // TODO -> for exceptions, find the real class sould be better

public class Agent : Skeleton {

	public enum Players{PLAYER, ENNEMY};

	private Human_Player _ennemy;
	public GameObject ShieldActivation;
	public GameObject ShieldS;
	public GameObject ShieldM;
	public GameObject ShieldH;
	public Rigidbody2D Weapon1;
	public Rigidbody2D Weapon3;
	public Rigidbody2D Weapon5;
	public Rigidbody2D Weapon10;

	private bool new_action = false;
	private float elapse = 0f;

	private ScoreKeeper Score;

	protected float _T    = 0.5f;

	protected float _stepA = (0.6f - 0.2f) / 100;
	protected float _stepG = (0.9f - 0.1f) / 100;

	protected float ALPHA = 0.6f;
	protected float GAMMA = 0.1f;

	protected string _state = "";


	// Combination of action :
	public enum ActionsComb{RELOADRELOAD, RELOADSHIELD, RELOADSHOOT_1, RELOADSHOOT_3, RELOADSHOOT_5, RELOADSHOOT_10,
		SHIELDRELOAD,  SHIELDSHIELD,  SHIELDSHOOT_1,  SHIELDSHOOT_3,  SHIELDSHOOT_5,  SHIELDSHOOT_10, 
		SHOOT_1RELOAD, SHOOT_1SHIELD, SHOOT_1SHOOT_1, SHOOT_1SHOOT_3, SHOOT_1SHOOT_5, SHOOT_1SHOOT_10, 
		SHOOT_3RELOAD, SHOOT_3SHIELD, SHOOT_3SHOOT_1, SHOOT_3SHOOT_3, SHOOT_3SHOOT_5, SHOOT_3SHOOT_10,
		SHOOT_5RELOAD, SHOOT_5SHIELD, SHOOT_5SHOOT_1, SHOOT_5SHOOT_3, SHOOT_5SHOOT_5, SHOOT_5SHOOT_10,
		SHOOT_10RELOAD , SHOOT_10SHIELD , SHOOT_10SHOOT_1 , SHOOT_10SHOOT_3 , SHOOT_10SHOOT_5 , SHOOT_10SHOOT_10 };


	protected Dictionary<Players, string> LAST_PATTERN = new Dictionary<Players, string> () {
		{ Players.PLAYER, "0"},
		{ Players.ENNEMY, "0"}
	};

	protected Dictionary<string, Dictionary<Actions,float>> PROBABILITIES = new Dictionary<string, Dictionary<Actions,float>> ();
	protected Dictionary<string, Dictionary<ActionsComb,float>> PATTERN_FILE  = new Dictionary<string, Dictionary<ActionsComb,float>> ();


	protected static Dictionary<Actions, Dictionary<Actions,ActionsComb>> AA_MATRIX = new Dictionary<Actions, Dictionary<Actions,ActionsComb>> (){
		{ Actions.SHOOT_1, new Dictionary<Actions, ActionsComb> () {{Actions.SHOOT_1,  ActionsComb.SHOOT_1SHOOT_1},  {Actions.SHOOT_3, ActionsComb.SHOOT_1SHOOT_3},  {Actions.SHOOT_5, ActionsComb.SHOOT_1SHOOT_5}, 
																	{Actions.SHOOT_10, ActionsComb.SHOOT_1SHOOT_10}, {Actions.RELOAD,  ActionsComb.SHOOT_1RELOAD},   {Actions.SHIELD,  ActionsComb.SHOOT_1SHIELD}}},
		{ Actions.SHOOT_3, new Dictionary<Actions, ActionsComb> () {{Actions.SHOOT_1,  ActionsComb.SHOOT_3SHOOT_1},  {Actions.SHOOT_3, ActionsComb.SHOOT_3SHOOT_3},  {Actions.SHOOT_5, ActionsComb.SHOOT_3SHOOT_5}, 
																	{Actions.SHOOT_10, ActionsComb.SHOOT_3SHOOT_10}, {Actions.RELOAD,  ActionsComb.SHOOT_3RELOAD},   {Actions.SHIELD,  ActionsComb.SHOOT_3SHIELD}}},
		{ Actions.SHOOT_5, new Dictionary<Actions, ActionsComb> () {{Actions.SHOOT_1,  ActionsComb.SHOOT_5SHOOT_1},  {Actions.SHOOT_3, ActionsComb.SHOOT_5SHOOT_3},  {Actions.SHOOT_5, ActionsComb.SHOOT_5SHOOT_5}, 
																	{Actions.SHOOT_10, ActionsComb.SHOOT_5SHOOT_10}, {Actions.RELOAD,  ActionsComb.SHOOT_5RELOAD},   {Actions.SHIELD,  ActionsComb.SHOOT_5SHIELD}}},
		{ Actions.SHOOT_10,new Dictionary<Actions, ActionsComb> () {{Actions.SHOOT_1,  ActionsComb.SHOOT_10SHOOT_1}, {Actions.SHOOT_3, ActionsComb.SHOOT_10SHOOT_3}, {Actions.SHOOT_5, ActionsComb.SHOOT_10SHOOT_5}, 
																	{Actions.SHOOT_10, ActionsComb.SHOOT_10SHOOT_10},{Actions.RELOAD,  ActionsComb.SHOOT_10RELOAD},  {Actions.SHIELD,  ActionsComb.SHOOT_10SHIELD}}},
		{ Actions.RELOAD,  new Dictionary<Actions, ActionsComb> () {{Actions.SHOOT_1,  ActionsComb.RELOADSHOOT_1},   {Actions.SHOOT_3, ActionsComb.RELOADSHOOT_3},   {Actions.SHOOT_5, ActionsComb.RELOADSHOOT_5}, 
																	{Actions.SHOOT_10, ActionsComb.RELOADSHOOT_10},  {Actions.RELOAD,  ActionsComb.RELOADRELOAD},    {Actions.SHIELD,  ActionsComb.RELOADSHIELD}}},
		{ Actions.SHIELD,  new Dictionary<Actions, ActionsComb> () {{Actions.SHOOT_1,  ActionsComb.SHIELDSHOOT_1},   {Actions.SHOOT_3, ActionsComb.SHIELDSHOOT_3},   {Actions.SHOOT_5, ActionsComb.SHIELDSHOOT_5}, 
																	{Actions.SHOOT_10, ActionsComb.SHIELDSHOOT_10},  {Actions.RELOAD,  ActionsComb.SHIELDRELOAD},    {Actions.SHIELD,  ActionsComb.SHIELDSHIELD}}}
	};

	private string PATH_prob = "proba";
	private string PATH_Q	 = "q";

	private string STOR_prob;
	private string STOR_Q;


	// Use this for initialization
	protected override void  Start () {

		// Update variables 
		player_name = "Agent";

		_ennemy = (Human_Player) GameObject.FindObjectOfType(typeof(Human_Player));
		Score = (ScoreKeeper) GameObject.FindObjectOfType(typeof(ScoreKeeper));

		STOR_Q 	  = Application.persistentDataPath + "/" + PATH_Q;
		STOR_prob = Application.persistentDataPath + "/" + PATH_prob;

		Load (PROBABILITIES);
		Load (PATTERN_FILE );

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
			ShieldActivation.SetActive (true);
			ShieldH.SetActive (PlayerShieldHP >= 6);
			ShieldM.SetActive (PlayerShieldHP >= 3 && PlayerShieldHP < 6);
			ShieldS.SetActive (PlayerShieldHP >= 0 && PlayerShieldHP < 3);

			if (PlayerHP <= 0) {

				ShieldH.GetComponent<BoxCollider2D> ().isTrigger = true;
				ShieldM.GetComponent<BoxCollider2D> ().isTrigger = true;
				ShieldS.GetComponent<BoxCollider2D> ().isTrigger = true;
			}

		} else {
			ShieldActivation.SetActive (false);
			ShieldS.SetActive (false);
			ShieldM.SetActive (false);
			ShieldH.SetActive (false);
			elapse = 0;
		}
	
	}

	public void OnDisable() {

		Save (PROBABILITIES);
		Save (PATTERN_FILE);

	}


	// Read the data file
	private void Load(Dictionary<string, Dictionary<Actions,float>> D)
	{
		// Handle any problems that might arise when reading the text
		try
		{
			string content;

			if (File.Exists(STOR_prob)) {
				print("FILE EXIST");
				StreamReader sr = File.OpenText(STOR_prob);
				content = sr.ReadToEnd ();
				sr.Close ();

			} else
				content = ((TextAsset)Resources.Load(PATH_prob, typeof(TextAsset))).text;

			string[] lineArray = content.Split("\n"[0]);

			/* loop over each line in the file */
			foreach ( string thisLine in lineArray ) {
				/* split each line by commas */
				string[] s = thisLine.Split("\t"[0]);

				if (s.Length > 2 ) {

					D.Add(s[0], new Dictionary<Actions,float> () {
						{Actions.RELOAD,  float.Parse(s[1])},
						{Actions.SHIELD,  float.Parse(s[2])},
						{Actions.SHOOT_1, float.Parse(s[3])},
						{Actions.SHOOT_3, float.Parse(s[4])},
						{Actions.SHOOT_5, float.Parse(s[5])},
						{Actions.SHOOT_10,float.Parse(s[6])}} );
				}
			}
		}

		catch (Exception e)
		{
			Debug.Log("Error : " + e.Message);
		}
	}

	// Read the data file
	private void Load(Dictionary<string, Dictionary<ActionsComb,float>> D)
	{
		// Handle any problems that might arise when reading the text
		try
		{
			string content;

			if (File.Exists(STOR_Q)) {
				StreamReader sr = File.OpenText(STOR_Q);
				content = sr.ReadToEnd ();
				sr.Close ();

			} else
				content = ((TextAsset)Resources.Load(PATH_Q, typeof(TextAsset))).text;

			string[] lineArray = content.Split("\n"[0]);

			/* loop over each line in the file */
			foreach ( string thisLine in lineArray ) {
				/* split each line by commas */
				string[] s = thisLine.Split("\t"[0]);

				if (s.Length > 2 ) {

					D.Add(s[0], new Dictionary<ActionsComb,float> () {
						{ActionsComb.RELOADRELOAD,    float.Parse(s[1])},
						{ActionsComb.RELOADSHIELD,    float.Parse(s[2])}, 
						{ActionsComb.RELOADSHOOT_1,   float.Parse(s[3])}, 
						{ActionsComb.RELOADSHOOT_3,   float.Parse(s[4])}, 
						{ActionsComb.RELOADSHOOT_5,   float.Parse(s[5])},
						{ActionsComb.RELOADSHOOT_10,  float.Parse(s[6])},
						{ActionsComb.SHIELDRELOAD,    float.Parse(s[7])},
						{ActionsComb.SHIELDSHIELD,    float.Parse(s[8])},
						{ActionsComb.SHIELDSHOOT_1,   float.Parse(s[9])},
						{ActionsComb.SHIELDSHOOT_3,   float.Parse(s[10])},
						{ActionsComb.SHIELDSHOOT_5,   float.Parse(s[11])},
						{ActionsComb.SHIELDSHOOT_10,  float.Parse(s[12])},
						{ActionsComb.SHOOT_1RELOAD,   float.Parse(s[13])},
						{ActionsComb.SHOOT_1SHIELD,   float.Parse(s[14])},
						{ActionsComb.SHOOT_1SHOOT_1,  float.Parse(s[15])},
						{ActionsComb.SHOOT_1SHOOT_3,  float.Parse(s[16])},
						{ActionsComb.SHOOT_1SHOOT_5,  float.Parse(s[17])},
						{ActionsComb.SHOOT_1SHOOT_10, float.Parse(s[18])},
						{ActionsComb.SHOOT_3RELOAD,   float.Parse(s[19])},
						{ActionsComb.SHOOT_3SHIELD,   float.Parse(s[20])},
						{ActionsComb.SHOOT_3SHOOT_1,  float.Parse(s[21])},
						{ActionsComb.SHOOT_3SHOOT_3,  float.Parse(s[22])},
						{ActionsComb.SHOOT_3SHOOT_5,  float.Parse(s[23])},
						{ActionsComb.SHOOT_3SHOOT_10, float.Parse(s[24])},
						{ActionsComb.SHOOT_5RELOAD,   float.Parse(s[25])},
						{ActionsComb.SHOOT_5SHIELD,   float.Parse(s[26])},
						{ActionsComb.SHOOT_5SHOOT_1,  float.Parse(s[27])},
						{ActionsComb.SHOOT_5SHOOT_3,  float.Parse(s[28])},
						{ActionsComb.SHOOT_5SHOOT_5,  float.Parse(s[29])},
						{ActionsComb.SHOOT_5SHOOT_10, float.Parse(s[30])},
						{ActionsComb.SHOOT_10RELOAD , float.Parse(s[31])},
						{ActionsComb.SHOOT_10SHIELD , float.Parse(s[32])},
						{ActionsComb.SHOOT_10SHOOT_1 ,float.Parse(s[33])},
						{ActionsComb.SHOOT_10SHOOT_3 ,float.Parse(s[34])},
						{ActionsComb.SHOOT_10SHOOT_5 ,float.Parse(s[35])},
						{ActionsComb.SHOOT_10SHOOT_10,float.Parse(s[36])}
					});
						
				}
			}
		}

		catch (Exception e)
		{
			Debug.Log("Error : " + e.Message);
		}
	}

	// Read the data file
	private void Save(Dictionary<string, Dictionary<Actions,float>> D)
	{
		// Handle any problems that might arise when reading the text
		try
		{

			StreamWriter sr = File.CreateText(STOR_prob);

			foreach ( KeyValuePair<string, Dictionary<Actions,float>> kv in D) {
				
				sr.WriteLine(kv.Key + "\t" +
					kv.Value[Actions.RELOAD] + "\t" +
					kv.Value[Actions.SHIELD] + "\t" +
					kv.Value[Actions.SHOOT_1] + "\t" +
					kv.Value[Actions.SHOOT_3] + "\t" +
					kv.Value[Actions.SHOOT_5] + "\t" +
					kv.Value[Actions.SHOOT_10]);
				
			}
			sr.Close();
			Debug.Log("END SAVE !!");

		}

		catch (Exception e)
		{
			Debug.Log("Error : " + e.Message);
		}
	}


	private void Save(Dictionary<string, Dictionary<ActionsComb,float>> D)
	{
		// Handle any problems that might arise when reading the text
		try
		{

			StreamWriter sr = File.CreateText(STOR_Q);

			string text = "";

			foreach ( KeyValuePair<string, Dictionary<ActionsComb,float>> kv in D) {

				sr.WriteLine(kv.Key + "\t" +
					kv.Value[ActionsComb.RELOADRELOAD] + "\t" +
					kv.Value[ActionsComb.RELOADSHIELD] + "\t" +
					kv.Value[ActionsComb.RELOADSHOOT_1] + "\t" +
					kv.Value[ActionsComb.RELOADSHOOT_3] + "\t" +
					kv.Value[ActionsComb.RELOADSHOOT_5] + "\t" +
					kv.Value[ActionsComb.RELOADSHOOT_10] + "\t" +
					kv.Value[ActionsComb.SHIELDRELOAD] + "\t" +
					kv.Value[ActionsComb.SHIELDSHIELD] + "\t" +
					kv.Value[ActionsComb.SHIELDSHOOT_1] + "\t" +
					kv.Value[ActionsComb.SHIELDSHOOT_3] + "\t" +
					kv.Value[ActionsComb.SHIELDSHOOT_5] + "\t" +
					kv.Value[ActionsComb.SHIELDSHOOT_10] + "\t" +
					kv.Value[ActionsComb.SHOOT_1RELOAD] + "\t" +
					kv.Value[ActionsComb.SHOOT_1SHIELD] + "\t" +
					kv.Value[ActionsComb.SHOOT_1SHOOT_1] + "\t" +
					kv.Value[ActionsComb.SHOOT_1SHOOT_3] + "\t" +
					kv.Value[ActionsComb.SHOOT_1SHOOT_5] + "\t" +
					kv.Value[ActionsComb.SHOOT_1SHOOT_10] + "\t" +
					kv.Value[ActionsComb.SHOOT_3RELOAD] + "\t" +
					kv.Value[ActionsComb.SHOOT_3SHIELD] + "\t" +
					kv.Value[ActionsComb.SHOOT_3SHOOT_1] + "\t" +
					kv.Value[ActionsComb.SHOOT_3SHOOT_3] + "\t" +
					kv.Value[ActionsComb.SHOOT_3SHOOT_5] + "\t" +
					kv.Value[ActionsComb.SHOOT_3SHOOT_10] + "\t" +
					kv.Value[ActionsComb.SHOOT_5RELOAD] + "\t" +
					kv.Value[ActionsComb.SHOOT_5SHIELD] + "\t" +
					kv.Value[ActionsComb.SHOOT_5SHOOT_1] + "\t" +
					kv.Value[ActionsComb.SHOOT_5SHOOT_3] + "\t" +
					kv.Value[ActionsComb.SHOOT_5SHOOT_5] + "\t" +
					kv.Value[ActionsComb.SHOOT_5SHOOT_10] + "\t" +
					kv.Value[ActionsComb.SHOOT_10RELOAD] + "\t" +
					kv.Value[ActionsComb.SHOOT_10SHIELD] + "\t" +
					kv.Value[ActionsComb.SHOOT_10SHOOT_1] + "\t" +
					kv.Value[ActionsComb.SHOOT_10SHOOT_3] + "\t" +
					kv.Value[ActionsComb.SHOOT_10SHOOT_5] + "\t" +
					kv.Value[ActionsComb.SHOOT_10SHOOT_10]);

			}

			sr.WriteLine(text);
			sr.Close();

		}

		catch (Exception e)
		{
			Debug.Log("Error : " + e.Message);
		}
	}


	public IEnumerator Play() {

		//print ("ENNEMY is playing");

		// Update Alpha / Gamma

		ALPHA = Mathf.Max (0.2f, ALPHA - Score.gameNumber * _stepA);
		GAMMA = Mathf.Max (0.9f, GAMMA + Score.gameNumber * _stepG);

		//
		// This function is used to play a move according
		// to the board, player and time left provided as input.
		//

		// Set the status before we play
		_state = getStatsKey();

		// Get first all the action
		List<Actions> poss_action = getPossibleActions();

		//printDict (PATTERN_FILE);

		Actions action;

		if ( !PATTERN_FILE.ContainsKey( _state) ) {

			print ("Explo");

			action = Exploration(poss_action);
		} else {

			print ("Exploit");
			action = Exploitation(poss_action);
		}

		new_action = true;

		// Play the Action chosen.

		if (SHOOT_POWER.ContainsKey (action)) {

			if (action == Actions.SHOOT_1) 
				Shoot (action, Weapon1, -1);
			else if (action == Actions.SHOOT_3) 
				Shoot (action, Weapon3, -1);
			else if (action == Actions.SHOOT_5) 
				Shoot (action, Weapon5, -1);
			else if (action == Actions.SHOOT_10)
				Shoot (action, Weapon3, -1);

		} else if (action == Actions.SHIELD) {

			Shield ();

		} else {

			Reload ();
		}

		yield break;
			
	}

	public void whatDidYouPlay () {

		if (SHOOT_POWER.ContainsKey (_lastAction)) {
			_ennemy.takeDamage (_lastAction);
		}
	}

	private string getStatsKey() {

		//print ("STATS = " + PlayerShieldHP + "|" + Power + "|" + _ennemy.PlayerShieldHP + "|" + _ennemy.Power);
		return PlayerShieldHP + "|" + Power + "|" + _ennemy.PlayerShieldHP + "|" + _ennemy.Power;
	}

	private void storeLastAction() {
	// This function is used to store the current stats of the moves

		LAST_PATTERN[Players.PLAYER] += ACTIONS_TO_PATTERN[_lastAction];
		LAST_PATTERN[Players.ENNEMY] += ACTIONS_TO_PATTERN[_ennemy.LastAction];
	}

	private void postPlay() {
		storeLastAction();
		string nextState = getStatsKey();
		updateQ(_state, nextState);

	}





	private Actions Exploitation(List<Actions> poss_action) {


		String state = getStatsKey ();
		Actions c    = Actions.SHOOT_10;

		Dictionary<Actions, float> choices = new Dictionary<Actions, float> ();
		Dictionary<Actions, float> prob_choices = new Dictionary<Actions, float> ();

		foreach (Actions action in poss_action) {

			choices.Add (action, O (state, action));
			prob_choices.Add (action, 0);
		}

		print (choices);

		float acc = 0;

		foreach (KeyValuePair<Actions, float> choice in choices) {

			print(choice.Key);

			acc += Mathf.Exp (choice.Value / _T );

			prob_choices [choice.Key] = acc;
		}

		float rng = UnityEngine.Random.Range (0, acc);

		foreach (KeyValuePair<Actions, float> choice in prob_choices) {

			if (choice.Value >= rng) {

				print(choice.Value);
				c = choice.Key;
				break;
			}
		}




		print ("My ACTIONS is " + c);

		_lastAction = c;
		return c;

	}

	private Actions Exploration(List<Actions> action) {

		// See if we need to ponderate the ratio between shoots and others
		// since SHOOT appear more times than other but is less likely too
		_lastAction = action[UnityEngine.Random.Range(0, action.Count - 1)];
		return _lastAction;

	}
		

	//	# O(s, a_i)
	private float O (String state, Actions poss_action ) {
		//	# = \sum_{a_i} n(s,a_-i) / n(s) * Q( s, <a_i, a_-i> )
		Dictionary<Actions,float> n = N(state);

		float sum_n = 0;
		foreach (float x in n.Values) { sum_n += x;}

		// if no stats we return 0
		float sum = 0;
		if (sum_n != 0) {

			foreach (Actions opp_action in _ennemy.getPossibleActions()) {

				sum += n [opp_action] / sum_n * Q (state) [ AA_MATRIX[poss_action][opp_action] ];
			}
			return sum;
		}

		return 0;


	}


	//# V(\sigma)
	private float V(String state) {
		//#  = max_ai O(s, a_i)

		List<float> oo = new List<float> ();

		foreach( Actions action in getPossibleActions() ){

			oo.Add (O (state, action));
		}

		return Mathf.Max(oo.ToArray());
	}

	private void updateQ( String state, String nextState ) {
		//	# s' is nextState
		//	# Q(s, a_i, a_-i) = Q(s, a_i, a_-i) + alpha * (r + ◊ * V(s') - Q(s, a_i, a_-i))

		Actions action    = PATTERNS_TO_ACTIONS[LAST_PATTERN[Players.PLAYER].Substring(1)];
		Actions oppAction = PATTERNS_TO_ACTIONS[LAST_PATTERN[Players.ENNEMY].Substring(1)];

		float reward = payoff(action, oppAction);
		float q		 = Q(state)[ AA_MATRIX[action][oppAction] ];

		q += ALPHA * (reward + GAMMA * V (nextState) - q);

		PATTERN_FILE [state] [ AA_MATRIX[action][oppAction] ] = q;

		float sumProb = 0, maxProb = 50;
		foreach (KeyValuePair<Actions, float> kv in  PROBABILITIES[state]){ sumProb += kv.Value;}
		Dictionary<Actions, float> d = new Dictionary<Actions, float>();

		if ( sumProb > maxProb ) {

			foreach( KeyValuePair<Actions,float> kv in PROBABILITIES[state] ) {

				d.Add(kv.Key, kv.Value * maxProb / sumProb);
			
			}

			PROBABILITIES[state] = d;
		}


		PROBABILITIES[state][oppAction] += 1;

	}




	private Dictionary<Actions, float> N (String state){

		return getActionFromFile( state , PROBABILITIES );
	}

	private Dictionary<ActionsComb, float> Q (string state){

		return getActionFromFile( state , PATTERN_FILE );
	}

	private Dictionary<Actions, float> getActionFromFile (string state, Dictionary<string, Dictionary<Actions,float>> D ) {

		if (!D.ContainsKey (state)) {

			D.Add(state, new Dictionary<Actions,float> () {
				{Actions.RELOAD,  0},
				{Actions.SHIELD,  0},
				{Actions.SHOOT_1, 0},
				{Actions.SHOOT_3, 0},
				{Actions.SHOOT_5, 0},
				{Actions.SHOOT_10,0}} );

		}

		return D [state];
	}

	private Dictionary<ActionsComb, float> getActionFromFile (string state, Dictionary<string, Dictionary<ActionsComb,float>> D ) {

		if (!D.ContainsKey (state)) {

			D.Add(state, new Dictionary<ActionsComb,float> () {
				{ActionsComb.RELOADRELOAD,    0},
				{ActionsComb.RELOADSHIELD,    0}, 
				{ActionsComb.RELOADSHOOT_1,   0}, 
				{ActionsComb.RELOADSHOOT_3,   0}, 
				{ActionsComb.RELOADSHOOT_5,   0},
				{ActionsComb.RELOADSHOOT_10,  0},
				{ActionsComb.SHIELDRELOAD,    0},
				{ActionsComb.SHIELDSHIELD,    0},
				{ActionsComb.SHIELDSHOOT_1,   0},
				{ActionsComb.SHIELDSHOOT_3,   0},
				{ActionsComb.SHIELDSHOOT_5,   0},
				{ActionsComb.SHIELDSHOOT_10,  0},
				{ActionsComb.SHOOT_1RELOAD,   0},
				{ActionsComb.SHOOT_1SHIELD,   0},
				{ActionsComb.SHOOT_1SHOOT_1,  0},
				{ActionsComb.SHOOT_1SHOOT_3,  0},
				{ActionsComb.SHOOT_1SHOOT_5,  0},
				{ActionsComb.SHOOT_1SHOOT_10, 0},
				{ActionsComb.SHOOT_3RELOAD,   0},
				{ActionsComb.SHOOT_3SHIELD,   0},
				{ActionsComb.SHOOT_3SHOOT_1,  0},
				{ActionsComb.SHOOT_3SHOOT_3,  0},
				{ActionsComb.SHOOT_3SHOOT_5,  0},
				{ActionsComb.SHOOT_3SHOOT_10, 0},
				{ActionsComb.SHOOT_5RELOAD,   0},
				{ActionsComb.SHOOT_5SHIELD,   0},
				{ActionsComb.SHOOT_5SHOOT_1,  0},
				{ActionsComb.SHOOT_5SHOOT_3,  0},
				{ActionsComb.SHOOT_5SHOOT_5,  0},
				{ActionsComb.SHOOT_5SHOOT_10, 0},
				{ActionsComb.SHOOT_10RELOAD , 0},
				{ActionsComb.SHOOT_10SHIELD , 0},
				{ActionsComb.SHOOT_10SHOOT_1 ,0},
				{ActionsComb.SHOOT_10SHOOT_3 ,0},
				{ActionsComb.SHOOT_10SHOOT_5 ,0},
				{ActionsComb.SHOOT_10SHOOT_10,0}
			});
			
		}

		return D [state];
	}


	private int payoff(Actions playerNext, Actions enemyNext) {

		return (PlayerHP - _ennemy.PlayerHP) * 10;
	}

	private void printDict( Dictionary<string, Dictionary<Actions,float>> D) {

		print ("Printing Dictionnary");

		foreach (KeyValuePair< string, Dictionary<Actions,float>> dd in D) {

			Dictionary<Actions,float> DD = dd.Value;

			print(dd.Key + " : " +
				  DD[Actions.RELOAD]  + ", " + DD[Actions.SHIELD]  + ", " + DD[Actions.SHOOT_1]  + ", " + 
				  DD[Actions.SHOOT_3] + ", " + DD[Actions.SHOOT_5] + ", " + DD[Actions.SHOOT_10] + ", " );
		}
	}















	public IEnumerator RNGPlay() {

		while (true) {

			float rnd = UnityEngine.Random.value;

			if (rnd >= 0.66f) {

				Shield ();
				yield break;

			} else if (rnd >= 0.33f) {

				Reload ();
				yield break;

			} else {

				rnd = UnityEngine.Random.value;

				foreach (Actions action  in SHOOT_POWER.Keys) {

					if (canShoot (action) && rnd >= 0.5f) {

						Shoot (action, null, -1);
						yield break;
					}
				}

			}
		}


	}
		
}
