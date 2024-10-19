using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject on screen.
/// Checks whether a GameObject is on screen and can force it to stay on screen.
/// Note that this ONLY works for an orthographic Main Camera.
/// </summary>
public class BoundsCheck : MonoBehaviour {
    [System.Flags]
    public enum eScreenLocs {
        onScreen = 0,
        offRight = 1,
        offLeft = 2,
        offUp = 4,
        offDown = 8
    }
    public enum eType { center, inset, outset };  // a
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;  // a

    [Header("Dynamic")]
    public eScreenLocs screenLocs = eScreenLocs.onScreen;
    public float camWidth;
    public float camHeight;

    void Awake() {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate() {
        // Find the checkRadius that will enable center, inset, or outset
        float checkRadius = 0;  // b
        if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;
        screenLocs = eScreenLocs.onScreen;

        if (pos.x > camWidth + checkRadius) {
            pos.x = camWidth + checkRadius;
            screenLocs |= eScreenLocs.offRight;  // Update screenLocs
        }
        if (pos.x < -camWidth - checkRadius) {
            pos.x = -camWidth - checkRadius;
            screenLocs |= eScreenLocs.offLeft;  // Update screenLocs
        }

        if (pos.y > camHeight + checkRadius) {
            pos.y = camHeight + checkRadius;
            screenLocs |= eScreenLocs.offUp;  // Update screenLocs
        }
        if (pos.y < -camHeight - checkRadius) {
            pos.y = -camHeight - checkRadius;
            screenLocs |= eScreenLocs.offDown;  // Update screenLocs
        }


        if (keepOnScreen && !isOnScreen) {  // f
            transform.position = pos;
            screenLocs = eScreenLocs.onScreen;  // g
        }

        transform.position = pos;
    }

    public bool isOnScreen {
        get { return (screenLocs == eScreenLocs.onScreen); }  // e
    }
    public bool LocIs(eScreenLocs checkLoc){
        if(checkLoc == eScreenLocs.onScreen )return isOnScreen;
        return ( (screenLocs & checkLoc) == checkLoc);
    }
}
