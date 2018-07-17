using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public class readData : MonoBehaviour
{
	private string jsonString;
	public static String sensorName { set; get; }
	private JsonData itemData;
	public static Boolean token { set; get; }
	public static Boolean firstCreate { set; get; }
	private GameObject sensorNameObject;

	// Use this for initialization
	void Start()
	{

	}

	IEnumerator GetCountries()
	{
		
	


		string getCountriesUrl = "http://uoweb1.ncl.ac.uk/api/v1/sensor/live.json?sensor_name=" + sensorName + "&api_key=a1vvpmt4zk0o46v7veh29p8f3kqkuz611edwez2usenlnm8u018w28ft3cc6kughg5i7fj2xirxu63ap9mz6ghk0j7";
		//string getCountriesUrl = "http://uoweb1.ncl.ac.uk/api/v1/sensors/live.json?api_key=a1vvpmt4zk0o46v7veh29p8f3kqkuz611edwez2usenlnm8u018w28ft3cc6kughg5i7fj2xirxu63ap9mz6ghk0j7";
		//string getCountriesUrl = "http://uoweb1.ncl.ac.uk/api/v1/sensor/data/raw.json?start_time=20180529105700&end_time=20180529110000&sensor_name=new_new_emote_1904&api_key=a1vvpmt4zk0o46v7veh29p8f3kqkuz611edwez2usenlnm8u018w28ft3cc6kughg5i7fj2xirxu63ap9mz6ghk0j7";
		if (token == true)
		{

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
						//Debug.Log(jsonString);
						itemData = JsonMapper.ToObject(jsonString);
						var tValue = GameObject.FindGameObjectWithTag("TValue").GetComponent<Text>();
                        var noTwoValue = GameObject.FindGameObjectWithTag("NO2").GetComponent<Text>();
                        var noValue = GameObject.FindGameObjectWithTag("NO").GetComponent<Text>();
                        var hValue = GameObject.FindGameObjectWithTag("Humidity").GetComponent<Text>();
                        var sValue = GameObject.FindGameObjectWithTag("Sound").GetComponent<Text>();
                        var coValue = GameObject.FindGameObjectWithTag("CO").GetComponent<Text>();

						//Debug.Log((itemData[0]["data"]["Temperature"]["data"][0]).ToString());
						GameObject.FindGameObjectWithTag("sensorName").GetComponent<Text>().text = sensorName;
						tValue.text = (itemData[0]["data"]["Temperature"]["data"][0]).ToString() + " Celsius";
						noTwoValue.text = (itemData[0]["data"]["NO2"]["data"][0]).ToString().Substring(0, 5) + " ugm -3";
						noValue.text = (itemData[0]["data"]["NO"]["data"][0]).ToString().Substring(0, 5) + " ugm -3";
						hValue.text = (itemData[0]["data"]["Humidity"]["data"][0]).ToString() + " %";
						sValue.text = (itemData[0]["data"]["Sound"]["data"][0]).ToString() + " db";
						coValue.text = (itemData[0]["data"]["CO"]["data"][0]).ToString().Substring(0, 6) + " ugm -3";



						StartCoroutine(GetCountries());

					}
				}
			}
		}


	}


	// Update is called once per frame
	void Update()
	{
		StartCoroutine(GetCountries());

	}
}
