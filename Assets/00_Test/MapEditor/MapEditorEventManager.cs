using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NN;
namespace MapEditor
{
    public class MapEditorEventManager : MonoBehaviour
    {
        public class Event : EventElement
        {
            public Handle_EventData Event_CreateBuilding;
            public void Sene_Event_CreateBuilding(EventData data)
            {
                if (Event_CreateBuilding != null)
                    Event_CreateBuilding(data);
            }

        }
    }
}