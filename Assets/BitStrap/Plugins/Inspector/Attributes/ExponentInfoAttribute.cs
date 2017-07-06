using UnityEngine;

namespace BitStrap
{
	/// <summary>
	/// Put this attribute above a number field (int, long, float, double)
	/// It adds a small label to the right of the property field.
	/// This small label shows the number in scientific notation
	/// 
	/// <code>
	/// <para>// This results in 1E+09</para> 
	/// <para>[ExponentInfo]</para>
	/// <para>public float largeNumber = 1000000000f;</para>
	/// </code>
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
	public class ExponentInfoAttribute : PropertyAttribute
	{
		public GUIStyle labelStyle;
		public float width;

		public ExponentInfoAttribute()
		{
			labelStyle = GUI.skin.GetStyle("miniLabel");
			width = labelStyle.CalcSize(new GUIContent("0.0E+00")).x;
		}
	}
}