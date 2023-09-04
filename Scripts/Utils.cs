using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utils : MonoBehaviour
{
	
	public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
	{
		GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
		Transform transform = gameObject.transform;
		transform.SetParent(parent, false);
		transform.localPosition = localPosition;
		TextMesh textMesh = gameObject.GetComponent<TextMesh>();
		textMesh.anchor = textAnchor;
		textMesh.alignment = textAlignment;
		textMesh.characterSize = 0.15f;
		textMesh.text = text;
		textMesh.fontSize = 40;
		textMesh.color = Color.white;
		textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

		return textMesh;
	}
	
	// Must hit collider or doesnt work
	public static Vector3 GetMouseWorldPosition3D()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
		{
			return raycastHit.point;
		}
		return Vector3.zero;
	}

	public static Vector3 GetMouseWorldPosition()
	{
		Vector3 horizontalPlanePosition = GetMouseWorldPositionWithY(Input.mousePosition, Camera.main);
		horizontalPlanePosition.y = 0f;
		return horizontalPlanePosition;
	}
	
	public static Vector3 GetMouseWorldPositionWithZ()
	{
		return GetMouseWorldPositionWithY(Input.mousePosition, Camera.main);
	}
	
	public static Vector3 GetMouseWorldPosition(Camera camera)
	{
		return GetMouseWorldPositionWithY(Input.mousePosition, camera);
	}
	
	public static Vector3 GetMouseWorldPositionWithY(Vector3 screenPosition, Camera camera)
	{
		screenPosition.z = camera.transform.position.y;
		Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
		return worldPosition;
	}
}
