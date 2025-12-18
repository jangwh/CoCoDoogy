using System;
using UnityEngine;

[Serializable]
public class OptionalProperty
{
    public bool isOn = false;
    public Vector3Int linkedPos = new(int.MaxValue, int.MaxValue, int.MaxValue);
}
public interface IOptionalProperty
{
    public OptionalProperty property { get; set; }


}
