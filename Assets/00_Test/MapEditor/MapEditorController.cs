//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MapEditorController : MonoBehaviour
//{
//    public InputDown EventInputDown;
//    public delegate void InputDown(Vector3 pos);
//    public InputDown EventInputPress;
//    public delegate void InputPress(Vector3 pos);
//    public InputDown EventInputDrag;
//    public delegate void InputDrag(Vector3 pos);
//    public InputDown EventInputClick;
//    public delegate void InputClick(Vector3 pos);

//    public Vector3 firstTouchPos;
//    // Update is called once per frame
//    void Update()
//    {
//        if(Input.GetMouseButtonDown(0))
//        {
//            firstTouchPos = Input.mousePosition;
//            if (EventInputDown != null)
//            {
//                EventInputDown(firstTouchPos);
//            }
//        }
//        else if(Input.GetMouseButton(0))
//        {
//            if(firstTouchPos == Input.mousePosition)
//            {
//                //if (EventInputPress != null)
//                //    EventInputPress(Input.mousePosition);
//            }
//            else
//            {
//                if (EventInputDrag != null)
//                    EventInputDrag(Input.mousePosition);
//            }

//        }
//        else if(Input.GetMouseButtonUp(0))
//        {
//            if (EventInputClick != null)
//                EventInputClick(Input.mousePosition);
//        }

//    }
//}
