// This is only PC and next gen platforms version    
// AliyerEdon@gmail.com


#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;

public enum AmbientLight
{
	Skybox,
	Color
}
public enum DirectionalMode
{
	Directional,
	NonDirectional
}
public enum LightingMode
{
	Realtime,
	Baked
}
public enum LightSettings
{
	Default,
	Realtime,
	Mixed,
	Baked
}
public enum MyColorSpace
{
	Linear,
	Gamma
}
public enum VolumetricLightType
{
	Off,
	On
}
public enum PointSpotShadow
{
	Off,
	On
}
public enum LightProbeMode
{
	Proxy,
	Blend
}
[ExecuteInEditMode]
public class LightingBox : EditorWindow 
{
	
	public LightingProfile lightingProfile;
	public LightingMode lightingMode;
	public AmbientLight ambientLight;
	public DirectionalMode directionalMode;
	public LightSettings lightSettings;
	public LightProbeMode lightprobeMode;
	public Material skyBox;
	public Light sunLight;
	public Color sunColor = Color.white;
	public bool autoMode;
	public MyColorSpace colorSpace;
	public VolumetricLightType vLight;
	public PointSpotShadow psShadow;
	public float sunIntensity = 2.1f;
	public float bakedResolution = 10f;
	public Color ambientColor;

	public bool helpBox;

	Color redColor,greenColor;
	bool lightError;
	bool lightChecked;
	GUIStyle myFoldoutStyle;
	bool showLogs;

	// Add menu named "My Window" to the Window menu
	[MenuItem("Lighting/Lighting Box")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		LightingBox window = (LightingBox)EditorWindow.GetWindow(typeof(LightingBox));
		window.Show();
		window.autoRepaintOnSceneChange = true;
	}
	void OnDisable(){
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}
	void OnEnable()
	{FindSun ();
		currentScene = EditorApplication.currentScene;
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		if (System.String.IsNullOrEmpty (EditorPrefs.GetString (SceneManager.GetActiveScene ().name)))
			lightingProfile = Resources.Load ("DefaultSettings")as LightingProfile;
		else 
			lightingProfile = (LightingProfile)AssetDatabase.LoadAssetAtPath (EditorPrefs.GetString (SceneManager.GetActiveScene ().name), typeof(LightingProfile));

		OnLoad ();
	}

	// Display window elements (Lighting Box)   
	Vector2 scrollPos;
	void OnGUI()
	{
		if(sunLight)
			sunLight.intensity = sunIntensity;

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,
			false,
			false,
			GUILayout.Width(Screen.width ),
			GUILayout.Height(Screen.height));
		
		EditorGUILayout.Space ();

		GUILayout.Label ("Lighting Box - ALIyerEdon@gmail.com", EditorStyles.helpBox);
		if (!helpBox) {
			if (GUILayout.Button ("Show Help")) {
				helpBox = !helpBox;
			}
		} else {
			if (GUILayout.Button ("Hide Help")) {
				helpBox = !helpBox;
			}
		}
		if (GUILayout.Button ("Refresh")) {
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			if (System.String.IsNullOrEmpty (EditorPrefs.GetString (SceneManager.GetActiveScene ().name)))
				lightingProfile = Resources.Load ("DefaultSettings")as LightingProfile;
			else 
				lightingProfile = (LightingProfile)AssetDatabase.LoadAssetAtPath (EditorPrefs.GetString (SceneManager.GetActiveScene ().name), typeof(LightingProfile));

			OnLoad ();
		}
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		lightingProfile = EditorGUILayout.ObjectField ("Profile", lightingProfile, typeof(LightingProfile)) as LightingProfile;

		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Load")) {
			OnLoad ();
		}
		if (GUILayout.Button ("Save")) {
			OnSave ();
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		skyBox = EditorGUILayout.ObjectField ("SkyBox Material", skyBox, typeof(Material)) as Material;
		sunLight = EditorGUILayout.ObjectField ("Sun Light", sunLight, typeof(Light)) as Light;
		sunColor = EditorGUILayout.ColorField ("Sun Color", sunColor);
		sunIntensity = EditorGUILayout.FloatField ("Sun Intenity", sunIntensity);

		if (sunLight) {
			if (sunColor != sunLight.color)
				sunLight.color = sunColor;
		}

		EditorGUILayout.Space ();

		if (helpBox) {
			EditorGUILayout.HelpBox("Choose between Precomputed Realtime GI or Baked GI mode",MessageType.Info);
		}
		// Choose lighting mode (realtime GI or baked GI)
		lightingMode = (LightingMode)EditorGUILayout.EnumPopup("Lighting Mode",lightingMode,GUILayout.Width(343));
		if (lightingMode == LightingMode.Realtime) {
			Lightmapping.realtimeGI = true;
			Lightmapping.bakedGI = false;
			LightmapEditorSettings.giBakeBackend = LightmapEditorSettings.GIBakeBackend.Radiosity;
		}
		else
		{
			Lightmapping.realtimeGI = false;
			Lightmapping.bakedGI = true;
			LightmapEditorSettings.giBakeBackend = LightmapEditorSettings.GIBakeBackend.PathTracer;
		}

		if (lightingMode == LightingMode.Baked) {
			// Baked lightmapping resolution   
			bakedResolution = EditorGUILayout.FloatField ("Baked Resolution", bakedResolution);
			LightmapEditorSettings.bakeResolution = bakedResolution;
		}

		EditorGUILayout.Space ();
		if (helpBox) {
			EditorGUILayout.HelpBox("Set ambient lighting source as Skybox(IBL) or simple color",MessageType.Info);
		}
		// choose ambient lighting mode   (color or skybox)
		ambientLight = (AmbientLight)EditorGUILayout.EnumPopup("Ambient Source",ambientLight,GUILayout.Width(343));
		if (ambientLight == AmbientLight.Color) {
			ambientColor = EditorGUILayout.ColorField ("Color", ambientColor);
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			RenderSettings.ambientLight = ambientColor;
		}
		else
		{
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
		}


		EditorGUILayout.Space ();
		if (helpBox) {
			EditorGUILayout.HelpBox("Choose between Directional or NonDirectional Mode",MessageType.Info);
		}
		// Choose directional mode (non directional or directional    )
		directionalMode = (DirectionalMode)EditorGUILayout.EnumPopup("Directional Mode",directionalMode,GUILayout.Width(343));
		if(directionalMode == DirectionalMode.Directional)
			LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
		else
			LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;

		if (helpBox) {
			EditorGUILayout.HelpBox("Baked lightmapping resolution. Higher value needs more RAM and longer bake time. Check task manager about RAM usage during bake time",MessageType.Info);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (helpBox) {
			EditorGUILayout.HelpBox("Changing the type of all light sources (Realtime,Baked,Mixed)",MessageType.Info);
		}
		// Change file lightmapping type mixed,realtime baked
		lightSettings = (LightSettings)EditorGUILayout.EnumPopup("All Lights Type",lightSettings,GUILayout.Width(343));
		if (lightSettings == LightSettings.Baked) {

			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				SerializedObject serialLightSource = new SerializedObject(l);
				SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
				SerialProperty.intValue = 2;
				serialLightSource.ApplyModifiedProperties ();
			}
		} 
		if (lightSettings == LightSettings.Realtime) {

			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				SerializedObject serialLightSource = new SerializedObject(l);
				SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
				SerialProperty.intValue = 4;
				serialLightSource.ApplyModifiedProperties ();
			}
		}
		if (lightSettings == LightSettings.Mixed) {

			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				SerializedObject serialLightSource = new SerializedObject(l);
				SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
				SerialProperty.intValue = 1;
				serialLightSource.ApplyModifiedProperties ();
			}

		} 
		EditorGUILayout.Space ();
		if (helpBox) {
			EditorGUILayout.HelpBox("Choose between Linear or Gamma color space , default should be Linear for my settings    ",MessageType.Info);
		}
		// Choose color space (Linear,Gamma) i have used Linear inpost effect setting s
		colorSpace = (MyColorSpace)EditorGUILayout.EnumPopup("Color Space",colorSpace,GUILayout.Width(343));
		if (colorSpace == MyColorSpace.Gamma) 
			PlayerSettings.colorSpace = ColorSpace.Gamma;
		else
			PlayerSettings.colorSpace = ColorSpace.Linear;


		EditorGUILayout.Space ();
		if (helpBox) {
			EditorGUILayout.HelpBox("Activate Volumetric Lights For All Light Sources",MessageType.Info);
		}
		// Activate or deactivate volumetric lighting for all light sources
		vLight = (VolumetricLightType)EditorGUILayout.EnumPopup("Volumetric Light",vLight,GUILayout.Width(343));
		if (vLight == VolumetricLightType.On) {

			Camera[] cams = GameObject.FindObjectsOfType<Camera> ();

			foreach (Camera c in cams) {
				if (!c.gameObject.GetComponent<VolumetricLightRenderer> ()) {
					c.gameObject.AddComponent<VolumetricLightRenderer> ();
					c.gameObject.GetComponent<VolumetricLightRenderer> ().Resolution = VolumetricLightRenderer.VolumtericResolution.Quarter;
					c.gameObject.GetComponent<VolumetricLightRenderer> ().DefaultSpotCookie = Resources.Load ("spot_Cookie_") as Texture;
				}
			}

			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				if (!l.gameObject.GetComponent<VolumetricLight> ()) {
					l.gameObject.AddComponent<VolumetricLight> ();

					l.gameObject.GetComponent<VolumetricLight> ().SampleCount = 8;
					if(l.type == LightType.Directional)
						l.gameObject.GetComponent<VolumetricLight> ().ScatteringCoef = 0.0043f;
					else
						l.gameObject.GetComponent<VolumetricLight> ().ScatteringCoef = 1f;
					
					l.gameObject.GetComponent<VolumetricLight> ().ExtinctionCoef = 0;
					l.gameObject.GetComponent<VolumetricLight> ().SkyboxExtinctionCoef = 0.864f;
					l.gameObject.GetComponent<VolumetricLight> ().MieG = 0.675f;
					l.gameObject.GetComponent<VolumetricLight> ().HeightFog = false;
					l.gameObject.GetComponent<VolumetricLight> ().HeightScale = 0.1f;
					l.gameObject.GetComponent<VolumetricLight> ().GroundLevel = 0;
					if (l.type == LightType.Directional)
						l.gameObject.GetComponent<VolumetricLight> ().Noise = false;
					else {
						l.gameObject.GetComponent<VolumetricLight> ().Noise = true;

						if (l.type == LightType.Spot) {
							if (l.range == 10f)
								l.range = 43f;
							if (l.spotAngle == 30f)
								l.spotAngle = 43f;
							if (psShadow == PointSpotShadow.On)
								l.shadows = LightShadows.Hard;
							else
								l.shadows = LightShadows.None;
						}
						if (l.type == LightType.Point) {

							if(psShadow == PointSpotShadow.On)
								l.shadows = LightShadows.Hard;
							else
								l.shadows = LightShadows.None;
						}
						if (l.type == LightType.Directional)
								l.shadows = LightShadows.Hard;
					}
					
					l.gameObject.GetComponent<VolumetricLight> ().NoiseScale = 0.015f;
					l.gameObject.GetComponent<VolumetricLight> ().NoiseIntensity = 1f;
					l.gameObject.GetComponent<VolumetricLight> ().NoiseIntensityOffset = 0.3f;
					l.gameObject.GetComponent<VolumetricLight> ().NoiseVelocity = new Vector2 (3f, 3f);
					l.gameObject.GetComponent<VolumetricLight> ().MaxRayLength = 400;
				}
			}
		}
		if (vLight == VolumetricLightType.Off) {
			Camera[] cams = GameObject.FindObjectsOfType<Camera> ();

			foreach (Camera c in cams) {
				if (c.gameObject.GetComponent<VolumetricLightRenderer> ())
					DestroyImmediate (c.gameObject.GetComponent<VolumetricLightRenderer> ());
			}

			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				if (l.gameObject.GetComponent<VolumetricLight> ())
					DestroyImmediate(l.gameObject.GetComponent<VolumetricLight> ());
			}
		}
		EditorGUILayout.Space ();
		if (helpBox) {
			EditorGUILayout.HelpBox("Activate shadows for point and spot lights   ",MessageType.Info);
		}
		// Choose hard shadows state on off for spot and point lights
		psShadow = (PointSpotShadow)EditorGUILayout.EnumPopup("Spot/point Light Shadows",psShadow,GUILayout.Width(343));
		if (psShadow == PointSpotShadow.On) {

			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				if (l.type == LightType.Directional)
					l.shadows = LightShadows.Hard;

				if (l.type == LightType.Spot)
					l.shadows = LightShadows.Hard;

				if (l.type == LightType.Point)
				l.shadows = LightShadows.Hard;
			}
		}
		if (psShadow == PointSpotShadow.Off) {
			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) 
			{
				if (l.type == LightType.Spot)
					l.shadows = LightShadows.None;

				if (l.type == LightType.Point)
					l.shadows = LightShadows.None;
			}
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		lightprobeMode = (LightProbeMode)EditorGUILayout.EnumPopup("LightProbes Mode",lightprobeMode,GUILayout.Width(343));
		if (lightprobeMode == LightProbeMode.Blend) {

			MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer> ();

			foreach (MeshRenderer mr in renderers) 
			{
				if (!mr.gameObject.isStatic) {
					if (mr.gameObject.GetComponent<LightProbeProxyVolume> ())
						DestroyImmediate (mr.gameObject.GetComponent<LightProbeProxyVolume> ());
					mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
				}
			}
		}
		if (lightprobeMode == LightProbeMode.Proxy) {

			MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer> ();

			foreach (MeshRenderer mr in renderers) {

				if (!mr.gameObject.isStatic) {
					if(!mr.gameObject.GetComponent<LightProbeProxyVolume> ())
						mr.gameObject.AddComponent<LightProbeProxyVolume> ();
					mr.gameObject.GetComponent<LightProbeProxyVolume> ().resolutionMode = LightProbeProxyVolume.ResolutionMode.Custom;
					mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
				}
			}
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		autoMode = EditorGUILayout.Toggle ("Auto Mode", autoMode);

		if(autoMode)
			Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.Iterative;
		else
			Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
		
		// Start bake
		if (!Lightmapping.isRunning) {
			if (GUILayout.Button ("Bake")) {
				if (!Lightmapping.isRunning) {
					Lightmapping.BakeAsync ();
				}
			}
			if(GUILayout.Button("Clear"))
			{
				Lightmapping.Clear ();
			}
		}else {
			if (GUILayout.Button ("Cancel")) {
				if (Lightmapping.isRunning) {
					Lightmapping.Cancel ();
				}
			}
		}

		if (Input.GetKey (KeyCode.F)) {
			if (Lightmapping.isRunning)
				Lightmapping.Cancel ();
		}
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.E)) {
			if (!Lightmapping.isRunning)
				Lightmapping.BakeAsync ();
		}

		if (helpBox) {
			EditorGUILayout.HelpBox ("Bake : Shift + B", MessageType.Info);
			EditorGUILayout.HelpBox ("Cancel : Shift + C", MessageType.Info);
			EditorGUILayout.HelpBox ("Clear : Shift + E", MessageType.Info);

		}
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		if (showUtility) {
			

			EditorGUILayout.PrefixLabel ("Light Components");
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ((Texture)Resources.Load ("Directional"))) {

				EditorApplication.ExecuteMenuItem ("GameObject/Light/Directional Light");

			}
			if (GUILayout.Button ((Texture)Resources.Load ("Point"))) {

				EditorApplication.ExecuteMenuItem ("GameObject/Light/Point Light");

			}
			if (GUILayout.Button ((Texture)Resources.Load ("Spot"))) {

				EditorApplication.ExecuteMenuItem ("GameObject/Light/Spotlight");

			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.PrefixLabel ("Visual Effects");
			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ((Texture)Resources.Load ("Terrain"))) {

				GameObject TerrainObj = new GameObject ("Terrain" + EditorPrefs.GetInt (PlayerSettings.productName + SceneManager.GetActiveScene ().name + "TerrainNumber"));
				EditorPrefs.SetInt (PlayerSettings.productName + SceneManager.GetActiveScene ().name + "TerrainNumber", EditorPrefs.GetInt (PlayerSettings.productName + SceneManager.GetActiveScene ().name + "TerrainNumber") + 1);
				TerrainData _TerrainData = new TerrainData ();

				_TerrainData.size = new Vector3 (32, 600, 32);
				_TerrainData.heightmapResolution = 513;
				_TerrainData.baseMapResolution = 1024;
				_TerrainData.SetDetailResolution (1024, 8);

				int _heightmapWidth = _TerrainData.heightmapWidth;
				int _heightmapHeight = _TerrainData.heightmapHeight;
				Terrain _Terrain2 = TerrainObj.AddComponent<Terrain> ();
				TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider> ();

				_TerrainCollider.terrainData = _TerrainData;
				_Terrain2.terrainData = _TerrainData;

				AssetDatabase.CreateAsset (_TerrainData, "Assets/" + TerrainObj.name + ".asset");
				TerrainObj.isStatic = true;
			}
			if (GUILayout.Button ((Texture)Resources.Load ("Cube"))) {
			
				EditorApplication.ExecuteMenuItem ("GameObject/3D Object/Cube");
			}
			EditorGUILayout.EndHorizontal ();
		}
		if (GUILayout.Button ("Utility")) {

			showUtility = !showUtility;
		}
		if (GUILayout.Button ("Lighting Window")) {

			EditorApplication.ExecuteMenuItem("Window/Lighting/Settings");
		}
		if (GUILayout.Button ("Move To View")) {

			EditorApplication.ExecuteMenuItem ("GameObject/Move To View");
		}
		if (GUILayout.Button ("Reset Settings")) {
			EditorPrefs.SetInt(PlayerSettings.productName+SceneManager.GetActiveScene().name+"FirstTime",0);
		}

		if (GUILayout.Button ("Show Log / Save")) {
			showLogs = !showLogs;
		}
		// Log window
		//===================================================
		if (showLogs) 
		{
			myFoldoutStyle = new GUIStyle(GUI.skin.button);
			redColor = new Color32 (184, 26, 26, 255);
			greenColor = new Color32 (0, 126, 22, 255);
			myFoldoutStyle.normal.textColor = redColor;
			myFoldoutStyle.fontStyle = FontStyle.Bold;
			myFoldoutStyle.fontStyle = FontStyle.Bold;

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			CheckColorSpace ();

			CheckLightingMode ();

			CheckSettings ();

		}EditorGUILayout.Space ();EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.EndScrollView();
		if(skyBox)
			RenderSettings.skybox = skyBox;



		SceneChangine ();
	}

	bool showUtility;
	public void CheckColorSpace()
	{
		if (PlayerSettings.colorSpace == ColorSpace.Gamma) {
			if (GUILayout.Button ("Prefered color space is Linear, Current is Gamma", myFoldoutStyle))
				PlayerSettings.colorSpace = ColorSpace.Linear;
		}
	}

	public void CheckLightingMode()
	{
		Light[] lights = GameObject.FindObjectsOfType<Light> ();
		foreach (Light l in lights) {
			// Check realtime state light types
			if (Lightmapping.realtimeGI == true) 
			{
				SerializedObject serialLightSource = new SerializedObject(l);
				SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
				if (SerialProperty.intValue == 2) {
					
					if (GUILayout.Button (l.name + " : Change light type to realtime in realtime lighting mode", myFoldoutStyle)) {
						SerialProperty.intValue = 1;
						serialLightSource.ApplyModifiedProperties ();
					}
				}
			}

			// Check baked state light types
			if (Lightmapping.bakedGI == true) 
			{
				SerializedObject serialLightSource = new SerializedObject(l);
				SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
				if (SerialProperty.intValue == 4)
				{
					if (GUILayout.Button (l.name + " : Change light type to Baked/Mixed in Baked lighting mode", myFoldoutStyle)) {
						SerialProperty.intValue = 2;
						serialLightSource.ApplyModifiedProperties ();
					}
				}
			}
			if (vLight == VolumetricLightType.On) {
				if (!l.GetComponent<VolumetricLight> ()) {
					
					if (GUILayout.Button (l.name + " : Don't has VolumetricLight compoennt", myFoldoutStyle))
						l.gameObject.AddComponent<VolumetricLight> ();
				}
			}
		}
		if (!sunLight)
		{
			if (GUILayout.Button ("Sunlight could not be found", myFoldoutStyle))
				EditorApplication.ExecuteMenuItem("GameObject/Light/Directional Light");
		}
	}

	void CheckSettings()
	{
		Camera[] cams = GameObject.FindObjectsOfType<Camera> ();

		foreach (Camera c in cams) {
			if (c.allowMSAA) {
				if (GUILayout.Button ("Disable MSAA in camera component", myFoldoutStyle))
					c.allowMSAA = false;
			}
			if (!c.allowHDR) {
				if (GUILayout.Button ("Enable Allow HDR in camera component", myFoldoutStyle))
					c.allowHDR = true;
			}

			if(vLight == VolumetricLightType.On)
			{
				if (!c.GetComponent<VolumetricLightRenderer> ()) {
					if (GUILayout.Button (c.name + ": VolumetricLightRenderer component is missing on camera", myFoldoutStyle))
						c.gameObject.AddComponent<VolumetricLightRenderer> ();
				}
			}
			if(vLight == VolumetricLightType.Off)
			{
				if (c.GetComponent<VolumetricLightRenderer> ()) {
					if (GUILayout.Button (c.name + ": VolumetricLightRenderer component is not necessary", myFoldoutStyle))
						c.gameObject.AddComponent<VolumetricLightRenderer> ();
				}
			}
		}

		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
		   EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ||
		   EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
			GUILayout.Label ("Current build target is not compatible for next-gen effects");
			if (GUILayout.Button ("Switch to PC", myFoldoutStyle))
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone ,BuildTarget.StandaloneWindows64);
			if (GUILayout.Button ("Switch to PS4", myFoldoutStyle))
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.PS4 ,BuildTarget.PS4);
			if (GUILayout.Button ("Switch to Xbox One", myFoldoutStyle))
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.XboxOne ,BuildTarget.XboxOne);
			if (GUILayout.Button ("Switch to OSX Universal", myFoldoutStyle))
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone ,BuildTarget.StandaloneOSXUniversal);
		}
	}
	// Always find latest directional light as sun light   
	void Update()
	{
		if (!sunLight)
			FindSun ();
	}

	// Find latest directional as sun light source
	void FindSun()
	{
		if (!sunLight) {
			Light[] lights = GameObject.FindObjectsOfType<Light> ();

			foreach (Light l in lights) {
				if (l.type == LightType.Directional) {
					sunLight = l;
					if (EditorPrefsX.GetColor (PlayerSettings.productName + SceneManager.GetActiveScene ().name + "SunColor") == null)
						sunColor = l.color;
					else {
						if(lightingProfile)
							sunColor = lightingProfile.sunColor;
					}

					sunLight.shadowNormalBias = 0;  
					if (sunLight.bounceIntensity == 1f)
						sunLight.bounceIntensity = 1.7f;
				}
			}

		}
	}

	// load saved data based on project and scene name
	void OnLoad()
	{
		if (lightingProfile) {

			lightingMode = lightingProfile.lightingMode;
			skyBox = lightingProfile.skyBox;

			ambientLight = lightingProfile.ambientLight;
			directionalMode =lightingProfile.directionalMode;
			lightSettings = lightingProfile.lightSettings;
			sunColor = lightingProfile.sunColor;
			colorSpace = lightingProfile.colorSpace;
			vLight = lightingProfile.volumetricLight;
			psShadow = lightingProfile.spotPointShadow;

			bakedResolution = lightingProfile.bakedResolution;
			ambientColor = lightingProfile.ambientColor;
			sunIntensity = lightingProfile.sunIntensity;

			autoMode = lightingProfile.automaticLightmap;

			Camera[] cams = GameObject.FindObjectsOfType<Camera> ();

			foreach (Camera c in cams) {
				c.allowHDR = true;
				c.allowMSAA = false;
			}
		}

		FindSun ();
	}

	void OnSave()
	{
		
		if (lightingProfile)
		{
			lightingProfile.lightingMode = lightingMode;  
			lightingProfile.skyBox = skyBox;
			lightingProfile.ambientLight = ambientLight;
			lightingProfile.directionalMode = directionalMode;
			lightingProfile.lightSettings = lightSettings;
			lightingProfile.sunColor = sunColor;
			lightingProfile.colorSpace = colorSpace;
			lightingProfile.volumetricLight = vLight;
			lightingProfile.spotPointShadow = psShadow;

			lightingProfile.bakedResolution = bakedResolution;
			lightingProfile.ambientColor = ambientColor;
			lightingProfile.sunIntensity = sunIntensity;
			lightingProfile.automaticLightmap = autoMode;

			EditorPrefs.SetString (SceneManager.GetActiveScene ().name, AssetDatabase.GetAssetPath (lightingProfile));
			EditorUtility.SetDirty (lightingProfile);
			AssetDatabase.SaveAssets ();
		}
	}

	bool keyState;
	public void OnSceneGUI(SceneView sceneView)
	{
		Event e = Event.current;

		if (e.type == EventType.keyDown)
		{
			if (Event.current.keyCode == (KeyCode.LeftControl))
				keyState = true;
			else
				keyState = false;
		}
		switch (e.type)
		{
		case EventType.keyUp:
			if (Event.current.keyCode == (KeyCode.S)) 
			{
				if(keyState)
					OnSave ();
			}
				break;   
		}
	}
	string currentScene;
	void SceneChangine ()
	{
		if (currentScene != EditorApplication.currentScene)
		{
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			if (System.String.IsNullOrEmpty (EditorPrefs.GetString (SceneManager.GetActiveScene ().name)))
				lightingProfile = Resources.Load ("DefaultSettings")as LightingProfile;
			else 
				lightingProfile = (LightingProfile)AssetDatabase.LoadAssetAtPath (EditorPrefs.GetString (SceneManager.GetActiveScene ().name), typeof(LightingProfile));

			OnLoad ();
			currentScene = EditorApplication.currentScene;
		}
	}

}
	
#endif