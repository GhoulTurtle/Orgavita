/// <summary>
/// Code taken from Unite 2016 Talk "Overthrowing the MonoBehaviour Tyranny in a Glorious Scriptable Object Revolution"
/// Original code belongs to Richard Fine, taken from GitHub - https://github.com/richard-fine/scriptable-object-demo/tree/main
/// </summary>

using System;

[Serializable]
public struct RangedFloat{
	public float minValue;
	public float maxValue;
}