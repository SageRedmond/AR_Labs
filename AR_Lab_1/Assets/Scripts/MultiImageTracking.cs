using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]

public class MultiImageTracking : MonoBehaviour
{   
    [SerializeField] private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private ARTrackedImageManager trackedImageManager;

    private void Awake() {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        //Pre-loading all the prefabs
        foreach(GameObject prefab in placeablePrefabs) {
            try {
                GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                newPrefab.name = prefab.name;
                spawnedPrefabs.Add(prefab.name, newPrefab);
            }
            catch (System.Exception e) {
                Debug.LogError("Instantiation failed: " + e.Message);
            }
            
            
        }
    }
    // Subscribe to Image Changed Event
    private void OnEnable() {
        trackedImageManager.trackedImagesChanged += OnImageChanged;
    }
    // Unsubscribe to Image Changed Event
    private void OnDisable() {
        trackedImageManager.trackedImagesChanged -= OnImageChanged;
    }
    // Respond to the Image Changed Event
    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs) {
        foreach(ARTrackedImage trackedImage in eventArgs.added) { //New Image Found
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated) { //Existing image changed
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed) { //Image tracking lost
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage) {
        string name = trackedImage.referenceImage.name;
        Debug.Log("IMAGE: " + trackedImage.referenceImage.name);

        //Get the transform of the detected image
        Vector3 position = trackedImage.transform.position;
        Quaternion rotation = trackedImage.transform.rotation;

        GameObject prefab = spawnedPrefabs[name]; //Refer to the prefab in our dictionary with the same name as the image
        prefab.transform.position = position;
        prefab.transform.rotation = rotation;
        prefab.SetActive(true);

        /* 
        This foreach loop makes sure only 1 model is active at a time.
        You can disable it if you want multiple objects acctive simultaneously
        */
        foreach(GameObject go in spawnedPrefabs.Values){
            if(go.name != name) {
                go.SetActive(false);
            }
        }
    }
}
