using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
	public string nameString;
	public Transform prefab;
	public Transform visual;
	public Sprite icon;
	
	public int maxStacks;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
