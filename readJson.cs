using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public class readJson : MonoBehaviour {
	private string jsonString;
	private JsonData itemData;
	GameObject target;
	public Button aButton;
	// Use this for initialization
	void Start () 
	{
		//jsonString = File.ReadAllText(Application.dataPath+"/Resources/data.json");
		 
		//Debug.Log(jsonString);
		//itemData = JsonMapper.ToObject(jsonString);
		//Debug.Log(itemData[0]["data"]["Sound"]["data"]["2018-05-29 10:59:29"]);
		StartCoroutine(GetCountries());

	
	}

	IEnumerator GetCountries()
	{

		string getCountriesUrl = "http://uoweb1.ncl.ac.uk/api/v1/sensor/data/raw.json?start_time=20180529105700&end_time=20180529110000&sensor_name=new_new_emote_1904&api_key=a1vvpmt4zk0o46v7veh29p8f3kqkuz611edwez2usenlnm8u018w28ft3cc6kughg5i7fj2xirxu63ap9mz6ghk0j7";

		using (UnityWebRequest www = UnityWebRequest.Get(getCountriesUrl))
		{
			//www.chunkedTransfer = false;
			yield return www.Send();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				if (www.isDone)
				{
					jsonString = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
					Debug.Log(jsonString);
					itemData = JsonMapper.ToObject(jsonString);
					var button = GameObject.Find("Temperature").GetComponent<Button>();
					button.GetComponentsInChildren<Text>()[1].text = (itemData[0]["data"]["Sound"]["data"]["2018-05-29 10:59:29"]).ToString();



                    Debug.Log(itemData[0]["data"]["Sound"]["data"]["2018-05-29 10:59:29"]);
                    StartCoroutine(GetCountries());

				}
			}
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
