using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GPS : MonoBehaviour
{
	List<string> places = new List<string>() { "new_new_emote_1903", "new_new_emote_1902", "new_new_emote_1904","new_new_emote_1908","new_new_emote_1705","new_new_emote_1907" };
	List<float> Latitudes = new List<float>() { 54.97304295188f, 54.9734549487624f, 54.972397607978f, 54.9721388049708f, 54.9718949244835f, 54.97457f };
	List<float> Longitudes = new List<float>() { -1.62198476063258f, -1.62154347697917f, -1.6225219524328f,-1.62310237397882f, -1.62269849919361f,-1.62482f};
	List<int> flag = new List<int>();
	List<GameObject> models = new List<GameObject>();
	List<String> existSensors = new List<String>();
	public static GPS Instance { set; get; }
	public float currentLatitude;
	public float currentLongitude;
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
        
		//readData.sensorName = places[0];
        //readData.token = true;
       

		radius = 15f;

		StartCoroutine(StartLocationService());
	}


	private void Update()
	{
		frameRateTextObject = GameObject.FindGameObjectWithTag("FrameRate");
		frameRateTextObject.GetComponent<Text>().text = "FPS:" + (Time.frameCount / Time.time).ToString();

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
					models[index].transform.position = camera.transform.position + camera.transform.forward * distancee;
					flag[index] = 1;
					models[index].SetActive(true);


                    readData.sensorName = places[index];
                    var tValue = GameObject.FindGameObjectWithTag("TValue").GetComponent<Text>();
                    var noTwoValue = GameObject.FindGameObjectWithTag("NO2").GetComponent<Text>();
                    var noValue = GameObject.FindGameObjectWithTag("NO").GetComponent<Text>();
                    var hValue = GameObject.FindGameObjectWithTag("Humidity").GetComponent<Text>();
                    var sValue = GameObject.FindGameObjectWithTag("Sound").GetComponent<Text>();
                    var coValue = GameObject.FindGameObjectWithTag("CO").GetComponent<Text>();
                  
					tValue.text = "reading...";
                    noTwoValue.text = "reading...";
                    noValue.text = "reading...";
                    hValue.text = "reading...";
                    sValue.text = "reading...";
                    coValue.text = "reading...";

				}
				else
				{
					models[index].SetActive(true);


                    readData.sensorName = places[index];
                    readData.token = true;

                    Debug.Log("you are here");
				}





			}

			else
			{
				Debug.Log("Not In range");
				readData.token = false;
				models[index].SetActive(false);


			}



			Input.location.Stop();
		}


	}



}
