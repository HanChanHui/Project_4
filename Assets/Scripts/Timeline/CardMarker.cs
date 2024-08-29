using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel;
using System.Collections.Generic;

namespace HornSpirit {
    [Serializable, DisplayName("Card Marker")]
    public class CardMarker : Marker, INotification {
        public CardData card;
        public Vector3 position;
        public List<GridPosition> placeableGridPosition;
        public int placeableCost;


        public PropertyName id { get { return new PropertyName(); } }
    }
}
