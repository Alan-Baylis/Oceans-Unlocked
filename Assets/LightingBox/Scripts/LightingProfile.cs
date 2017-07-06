#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Lighting Profile", order = 1)]
public class LightingProfile : ScriptableObject {
	// Global settings
    public string objectName = "LightingProfile";
	public Material skyBox;
	public  Color sunColor = Color.white;
	public float sunIntensity = 2.1f;
	public  float bakedResolution = 10f;
	public  LightingMode lightingMode;
	public  AmbientLight ambientLight;
	public  Color ambientColor = Color.white;
	public  DirectionalMode directionalMode;
	public  LightSettings lightSettings;
	public MyColorSpace colorSpace;
	public VolumetricLightType volumetricLight;
	public PointSpotShadow spotPointShadow;
	public bool automaticLightmap;
}
#endif