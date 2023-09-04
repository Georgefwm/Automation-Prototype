using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "ScriptableObjects/Recipe")]
public class RecipeSO : ScriptableObject
{
	public string displayName;
	
	// The machine that can craft this recipe
	public PlaceableObjectSO machineUsed;
	
	// Input items and amount required to start crafting
	public List<RecipeItem> inputItems;
	
	// Output items and how much is produced from 1 craft
	public List<RecipeItem> outputItems;
	
	// Time a single craft takes in seconds
	public float craftTime;
	
	[System.Serializable]
	public struct RecipeItem
	{
		public ItemSO itemType;
		public int amount;
	}

	public static float GetItemsPerMinute(RecipeItem recipeItem, float recipeCraftTime)
	{
		return recipeItem.amount * 60 / recipeCraftTime;
	}
}
