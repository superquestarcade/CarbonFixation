using Data;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PoI Data",  fileName = "PoI Data",  order = 0)]
public class PoiGenDataSo : ScriptableObject
{
	public PointOfInterestGenData[] pointOfInterestGenData;
	
}