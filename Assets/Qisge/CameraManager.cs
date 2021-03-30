using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public Camera Camera;

    CameraChanges lastValues = new CameraChanges();
    
    private void Awake() {
        lastValues.x = 0;
        lastValues.y = 0;
        lastValues.angle = 0;
        lastValues.size = 8;
        SetValues(lastValues);
    }

    public void UpdateCamera(CameraChanges changes) {
        if (changes==null) {
            return;
        }
        CheckDefaultValues(changes);
        SetValues(changes);
        lastValues = changes;
    }

    public void SetValues(CameraChanges changes) {
        //Debug.Log("Set Values" + changes.size + " " + changes.x + " " + changes.y + " " + changes.angle);
        Camera.orthographicSize = changes.size;
        Camera.transform.localPosition = new Vector3(changes.x, changes.y, 0);
        Camera.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, changes.angle));
    }


    public void CheckDefaultValues(CameraChanges changes) {
        if (changes.x<= CameraChanges.minValue) {
            changes.x = lastValues.x;
        }
        if (changes.y <= CameraChanges.minValue) {
            changes.y = lastValues.y;
        }
        if (changes.angle <= CameraChanges.minValue) {
            changes.angle = lastValues.angle;
        }
        if (changes.size <= CameraChanges.minValue) {
            changes.size = lastValues.size;
        }
    }

}
