using UnityEngine;
using UnityEngine.Pool;

public abstract class LinkableIdentity : BlockIdentity, IOptionalProperty
{
    public OptionalProperty property { get; set; }
}


    
