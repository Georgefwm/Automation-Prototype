using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Vector3 _offset;
	private float _smoothTime = 0.1f;
	private Vector3 _velocity = Vector3.zero;
	private Vector3 _targetPosition;
	private float _sensitivity = 0.1f;

	private void Start()
	{
		//_offset = new Vector3(0, 0, 0);
		_offset = transform.position;
		_targetPosition = _offset;
	}

	private void Update()
	{

		if (Input.GetKey(KeyCode.A)) _targetPosition.x += -1 * _sensitivity;
		if (Input.GetKey(KeyCode.D)) _targetPosition.x += 1 * _sensitivity;
		if (Input.GetKey(KeyCode.W)) _targetPosition.z += 1 * _sensitivity;
		if (Input.GetKey(KeyCode.S)) _targetPosition.z += -1 * _sensitivity;
		if (Input.GetKey(KeyCode.E)) _targetPosition.y += 1 * (_sensitivity * 0.5f);
		if (Input.GetKey(KeyCode.Q)) _targetPosition.y += -1 * (_sensitivity * 0.5f);
		
		
		Vector3 newTargetPosition = _targetPosition;
		
		Vector3 newTransformPosition = Vector3.SmoothDamp(transform.position, newTargetPosition, ref _velocity, _smoothTime);
		transform.position = newTransformPosition;
	}
}
