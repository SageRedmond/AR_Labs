using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject PlaneObjectPrefab;
    [SerializeField] private GameObject WallAndRoofObjectPrefab;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake(){
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable(){
        EnhancedTouch.EnhancedTouchSupport.Enable();
        //Subscribe to onFingerDown Event
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable() {
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger){
        //Don't do anything if there are multiple fingers on the screen
        if(finger.index != 0) return;

        if (aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon)){
            foreach(ARRaycastHit hit in hits){
                Pose pose = hit.pose;

                if (aRPlaneManager.GetPlane(trackableId: hit.trackableId).alignment == PlaneAlignment.HorizontalUp){
                    GameObject obj = Instantiate(original: PlaneObjectPrefab, position: pose.position, rotation: pose.rotation);

                    Vector3 objectPosition = obj.transform.position;
                    Vector3 cameraPosition = Camera.main.transform.position;
                    Vector3 direction = cameraPosition - objectPosition;
                    Vector3 targetRotationEuler = Quaternion.LookRotation(forward: direction).eulerAngles;
                    Vector3 scaledEular = Vector3.Scale(targetRotationEuler, obj.transform.up.normalized); // (0, 1, 0)
                    Quaternion targetRotation = Quaternion.Euler(scaledEular);
                    obj.transform.rotation = obj.transform.rotation * targetRotation;
                }
                else{
                    GameObject obj = Instantiate(original: WallAndRoofObjectPrefab, position: pose.position, rotation: pose.rotation);
                }
            }
        }
    }
}
