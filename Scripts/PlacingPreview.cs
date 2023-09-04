using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingPreview : MonoBehaviour
{
	private float _transitionSpeed = 40f;
	private Transform _visual;
	private PlaceableObjectSO _placeableObjectSO;
	
    // Start is called before the first frame update
    void Start()
    {
        RefreshVisual();

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
	    RefreshVisual();
    }

    private void LateUpdate() {
	    Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
	    targetPosition.y = 0f;

	    Transform currentTransform = transform;
	    transform.position = Vector3.Lerp(currentTransform.position, targetPosition, Time.deltaTime * _transitionSpeed);
	    transform.rotation = Quaternion.Lerp(currentTransform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * _transitionSpeed);
    }

    private void RefreshVisual() {
	    if (_visual != null) {
		    Destroy(_visual.gameObject);
		    _visual = null;
	    }

	    PlaceableObjectSO placeableObjectSO = GridBuildingSystem.Instance.GetSelectedPlaceableObject();
	    if (placeableObjectSO == null) return;
	    
	    _visual = Instantiate(placeableObjectSO.visual, Vector3.zero, Quaternion.identity);
	    _visual.parent = transform;
	    _visual.localPosition = Vector3.zero;
	    _visual.localEulerAngles = Vector3.zero;
	    SetLayerRecursive(_visual.gameObject, 11);
	    
    }
    
    private void SetLayerRecursive(GameObject targetGameObject, int layer) {
	    targetGameObject.layer = layer;
	    foreach (Transform child in targetGameObject.transform) {
		    SetLayerRecursive(child.gameObject, layer);
	    }
    }
}
