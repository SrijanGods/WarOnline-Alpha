//----------------------------------------------
//            Realistic Tank Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[AddComponentMenu("BoneCracker Games/Realistic Tank Controller/UI/Dashboard")]
public class RTC_UIDashboard : MonoBehaviour {

	// Getting an Instance of Main Shared RTC Settings.
	#region RTC Settings Instance

	private RTC_Settings RTCSettingsInstance;
	private RTC_Settings RTCSettings {
		get {
			if (RTCSettingsInstance == null) {
				RTCSettingsInstance = RTC_Settings.Instance;
			}
			return RTCSettingsInstance;
		}
	}

	#endregion

	private RectTransform rectTransform;
	private RTC_MainCamera mainCamera;

	public GameObject UIRoot;
	public GameObject mobileControllers;

	private RTC_TankController currentTankController;

	[Header("UI Texts")]
	[Space()]
	public Text projectileName;
	public Text currentAmmo;
	public Text totalAmmo;

	[Header("UI Images")]
	[Space()]
	public Image crosshair;
	public Image tankRotation;


	[Space()]
	public bool disableUIWhenNoVehicle = true;

	void Awake () {

		if (RTCSettings.uiType == RTC_Settings.UIType.None) {
			gameObject.SetActive (false);
			return;
		}

		currentTankController = GetComponentInParent<RTC_TankController>();
		rectTransform = GetComponent<RectTransform> ();

		if (RTCSettings.controllerType != RTC_Settings.ControllerType.Mobile)
			mobileControllers.SetActive (false);
		else
			mobileControllers.SetActive (true);
	
	}

	void OnEnable(){

		RTC_TankController.OnRTCSpawned += OnRTCSpawned;

	}

	void OnRTCSpawned (RTC_TankController tankController) {

		currentTankController = tankController;
	
	}

	void OnDisable(){

		RTC_TankController.OnRTCSpawned -= OnRTCSpawned;

	}

	void LateUpdate(){

		if (!currentTankController) {
			UIRoot.SetActive (false);
			return;
		}

		if (!currentTankController.canControl || !currentTankController.gameObject.activeInHierarchy || !currentTankController.enabled) {
			if (disableUIWhenNoVehicle)
				UIRoot.SetActive (false);
			return;
		}

		if(!UIRoot.activeInHierarchy)
			UIRoot.SetActive (true);



		/*
		if(crosshair){

			if (!mainCamera) {
				mainCamera = GameObject.FindObjectOfType<RTC_MainCamera> ();
				return;
			}


			Vector3 pos;
			pos.x = rectTransform.rect.width  * vpPosition.x - rectTransform.rect.width / 2f;
			pos.y = rectTransform.rect.height * vpPosition.y - rectTransform.rect.height / 2f;
			pos.z = 0f;

			crosshair.transform.localPosition = pos;

		}
		*/

	}

}
