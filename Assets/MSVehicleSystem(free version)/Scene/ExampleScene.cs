using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene : MonoBehaviour {

	GameObject DirectionalLight;
	bool instructions = true;

	MSSceneController controls;

	string enterAndE;
	string startCar;
	string reload;
	string handBrak;
	string switchc;
	string gearsAuto;

	void Awake(){
		controls = GameObject.FindObjectOfType (typeof(MSSceneController))as MSSceneController;
		DirectionalLight = transform.GetComponentInChildren <Light>().gameObject;
		instructions = true;
		if (!controls) {
			instructions = false;
			return;
		}
		enterAndE = controls.controls.enterEndExit.ToString ();
		startCar = controls.controls.startTheVehicle.ToString ();
		reload = controls.controls.reloadScene.ToString ();
		handBrak = controls.controls.handBrakeInput.ToString ();
		switchc = controls.controls.switchingCameras.ToString ();
	}

	void Start(){
		DirectionalLight.GetComponent<Light> ().intensity = 1;
		RenderSettings.reflectionIntensity = 1;
		RenderSettings.ambientSkyColor = new Color(1,1,1);
	}

	void OnGUI(){
		if (instructions) {
			GUI.skin.label.fontSize = 14;
			if(GUI.Button(new Rect(115,5,400,40),"Click here for the full version of the asset 'MS Vehicle System'")){
				Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/88035");
			}
			GUI.Box (new Rect (5, 160, 180, 135), "");
			GUI.color = Color.green;
			GUI.Label (new Rect (10, 165, 180, 35), "CONTROLS");
			GUI.color = Color.white;
			GUI.Label (new Rect (10, 190, 300, 35), "Enter/Exit Vehicle: " + enterAndE);
			GUI.Label (new Rect (10, 210, 300, 35), "Start the vehicle: " + startCar);
			GUI.Label (new Rect (10, 230, 300, 35), "Reload Scene: " + reload);
			GUI.Label (new Rect (10, 250, 300, 35), "Hand Brake: " + handBrak);
			GUI.Label (new Rect (10, 270, 300, 35), "Switch Cameras: " + switchc);
		}
	}
}
