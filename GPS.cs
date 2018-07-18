using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using LitJson;
using UnityEngine.Networking;

public class GPS : MonoBehaviour
{
	List<string> places = new List<string>() { "new_new_emote_1903", "new_new_emote_1902", "new_new_emote_1904", "new_new_emote_1908", "new_new_emote_1705", "new_new_emote_1907" };
	List<float> Latitudes = new List<float>() { 54.97304295188f, 54.9734549487624f, 54.972397607978f, 54.9721388049708f, 54.9718949244835f, 54.97457f };
	List<float> Longitudes = new List<float>() { -1.62198476063258f, -1.62154347697917f, -1.6225219524328f, -1.62310237397882f, -1.62269849919361f, -1.62482f };
	List<int> flag = new List<int>();
	List<GameObject> models = new List<GameObject>();
	List<String> existSensors = new List<String>();
	public static GPS Instance { set; get; }
	public float currentLatitude;
	public float currentLongitude;
	private string jsonString;
	private JsonData itemData;
	public GameObject canvas;
	public GameObject camera;
	private GameObject frameRateTextObject;
	private GameObject distanceTextObject;
	private GameObject TargetTextObject;
	private GameObject coordinatesObject;
	float distancee = 1;


	private float radius;

	// Use this for initialization
	private void Start()
	{

		distanceTextObject = GameObject.FindGameObjectWithTag("distance");
		TargetTextObject = GameObject.FindGameObjectWithTag("TargetPosition");
		coordinatesObject = GameObject.FindGameObjectWithTag("coordinates");

		Instance = this;

		for (int i = 0; i < places.Count; i++)
		{
			models.Add(Instantiate(canvas));
			flag.Add(0);
		}
  
		radius = 15f;

		StartCoroutine(StartLocationService());
	}


	private void Update()
	{
		frameRateTextObject = GameObject.FindGameObjectWithTag("FrameRate");
		frameRateTextObject.GetComponent<Text>().text = "FPS:" + (Time.frameCount / Time.time).ToString();

	}
	public IEnumerator getData(int index, String sensorName)
	{

		string getCountriesUrl = "http://uoweb1.ncl.ac.uk/api/v1/sensor/live.json?sensor_name=" + sensorName + "&api_key=a1vvpmt4zk0o46v7veh29p8f3kqkuz611edwez2usenlnm8u018w28ft3cc6kughg5i7fj2xirxu63ap9mz6ghk0j7";


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
					itemData = JsonMapper.ToObject(jsonString);
					GameObject.FindGameObjectWithTag("sensorName").GetComponent<Text>().text = sensorName;
					models[index].transform.Find("TemperaturePanel/T Value").GetComponent<Text>().text = (itemData[0]["data"]["Temperature"]["data"][0]).ToString() + " Celsius";
					models[index].transform.Find("NO2Panel/NO2 Value").GetComponent<Text>().text = (itemData[0]["data"]["NO2"]["data"][0]).ToString().Substring(0, 5) + " ugm -3";
					models[index].transform.Find("HumidityPanel/H Value").GetComponent<Text>().text = (itemData[0]["data"]["Humidity"]["data"][0]).ToString() + " %";
					models[index].transform.Find("COPanel/CO Value").GetComponent<Text>().text = (itemData[0]["data"]["CO"]["data"][0]).ToString().Substring(0, 6) + " ugm -3";
					models[index].transform.Find("SoundPanel/S Value").GetComponent<Text>().text = (itemData[0]["data"]["Sound"]["data"][0]).ToString() + " db";
					models[index].transform.Find("NOPanel/NO Value").GetComponent<Text>().text = (itemData[0]["data"]["NO"]["data"][0]).ToString().Substring(0, 5) + " ugm -3";

				}
			}
		}


	}


	private float Calc(float referenceLatitude, float referenceLongitude, float currentLatitude, float currentLongitude)
	{
		float distance = 0f, x = 0f, y = 0f, r1, r2, r3, c;
		float R = 6378.137f;
		r1 = referenceLatitude * Mathf.Deg2Rad;
		r2 = currentLatitude * Mathf.Deg2Rad;
		x = (currentLatitude - referenceLatitude) * Mathf.Deg2Rad;
		y = (currentLongitude - referenceLongitude) * Mathf.Deg2Rad;
		r3 = Mathf.Sin(x / 2) * Mathf.Sin(x / 2) + Mathf.Cos(r1) * Mathf.Cos(r2) * Mathf.Sin(y / 2) * Mathf.Sin(y / 2);
		c = 2 * Mathf.Atan2(Mathf.Sqrt(r3), Mathf.Sqrt(1 - r3));
		distance = Mathf.RoundToInt(R * c * 1000f);
		return distance;

	}





	private IEnumerator StartLocationService()
	{

		while (true)
		{

			List<float> distances = new List<float>();
			float nearby;
			int index;
			if (!Input.location.isEnabledByUser)
			{
				Debug.Log("User has not enabled GPS");
				yield break;
			}
			Input.location.Start();

			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
			{
				yield return new WaitForSeconds(1);
				maxWait--;
			}

			if (maxWait <= 0)
			{
				Debug.Log("Timed out");
				yield break;
			}

			if (Input.location.status == LocationServiceStatus.Failed)
			{
				Debug.Log("Unable to determin device location");
				yield break;
			}

			currentLatitude = Input.location.lastData.latitude;
			currentLongitude = Input.location.lastData.longitude;
			coordinatesObject.GetComponent<Text>().text = "Current Latitude:  " + GPS.Instance.currentLatitude.ToString() + " ,Current Longitude: " + GPS.Instance.currentLongitude.ToString();

			for (int i = 0; i < places.Count; i++)
			{
				distances.Add(Calc(Latitudes[i], Longitudes[i], currentLatitude, currentLongitude));
			}
			nearby = distances.Min();
			index = distances.IndexOf(nearby);
			distanceTextObject.GetComponent<Text>().text = "The closet sensor in: " + nearby.ToString() + " meters";
			TargetTextObject.GetComponent<Text>().text = "The nearst sensor: " + places[index];

			if (nearby <= radius)
			{
				if (flag[index] != 1)
				{
					GameObject.FindGameObjectWithTag("sensorName").GetComponent<Text>().text = places[index];


					models[index].transform.position = camera.transform.position + camera.transform.forward * distancee;
					flag[index] = 1;
					models[index].SetActive(true);
					StartCoroutine(getData(index, places[index]));

				}
				else
				{
					models[index].SetActive(true);
                	Debug.Log("you are here");

				}





			}

			else
			{
				Debug.Log("Not In range");
                models[index].SetActive(false);
    		}
   
			Input.location.Stop();
		}


	}



}
