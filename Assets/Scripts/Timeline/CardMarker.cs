using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel;


[Serializable, DisplayName("Card Marker")]
public class CardMarker : Marker, INotification
{
    public CardData card;
    public Vector3 position;
    

    public PropertyName id {get {return new PropertyName();}}
}
