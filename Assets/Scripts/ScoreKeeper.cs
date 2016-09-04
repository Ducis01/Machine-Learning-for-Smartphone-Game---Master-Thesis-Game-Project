using UnityEngine;
using System.Collections;
using System.IO;
using System; // TODO -> for exceptions, find the real class sould be better


public class ScoreKeeper : MonoBehaviour {

	[HideInInspector]
	public float gameNumber;
	[HideInInspector]
	public float winAITotal;
	[HideInInspector]
	public float tieTotal;

	private string PATH_score = "score";
	private string STOR_score = "";

	// Unity things
	protected void Start ()
	{
		// Create the object
		STOR_score = Application.persistentDataPath + "/" + PATH_score;
		Load ();
	}
	
	// Setters and getters 
	public float winRate {
		get { return (gameNumber - tieTotal != 0) ? Mathf.Round(100 * (winAITotal / (gameNumber - tieTotal))) : 50; }
		// no outside set !
	}

	private void Load()
	{
		// Handle any problems that might arise when reading the text
		try
		{

			//StreamReader theReader = new StreamReader(fileName, Encoding.Default);

			string content;

			if (File.Exists(STOR_score)) {
				StreamReader sr = File.OpenText(STOR_score);
				content = sr.ReadLine();
				sr.Close ();
				
			} else
				content = ((TextAsset)Resources.Load(PATH_score, typeof(TextAsset))).text;
			

			print("The content" + content);

			string[] lineArray = content.Split("\n"[0]);

			/* loop over each line in the file */
			foreach ( string thisLine in lineArray ) {
				print(thisLine);
				/* split each line by commas */
				string[] s = thisLine.Split("\t"[0]);

				gameNumber =  float.Parse(s[0]);
				winAITotal =  float.Parse(s[1]);
				tieTotal   =  float.Parse(s[2]);
			
			}
		}

		catch (Exception e)
		{
			Debug.Log("Error : " + e.Message);
		}
	}

	public void OnDisable() {

		print (Application.persistentDataPath);
		print ("SAVE ME" + gameNumber + "\t" + winAITotal + "\t" + tieTotal);
		File.WriteAllText( Application.persistentDataPath +  "/" +  PATH_score, gameNumber + "\t" + winAITotal + "\t" + tieTotal);
	}
}
