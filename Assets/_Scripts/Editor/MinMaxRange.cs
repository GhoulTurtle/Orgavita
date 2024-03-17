/// <summary>
/// Code taken from Unite 2016 Talk "Overthrowing the MonoBehaviour Tyranny in a Glorious Scriptable Object Revolution"
/// Original code belongs to Richard Fine, taken from GitHub - https://github.com/richard-fine/scriptable-object-demo/tree/main
/// </summary>

using System;

public class MinMaxRangeAttribute : Attribute{
	public MinMaxRangeAttribute(float min, float max){
		Min = min;
		Max = max;
	}

	public float Min { get; private set; }
	public float Max { get; private set; }
}
