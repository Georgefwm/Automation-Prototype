using System.Collections;
using System.Collections.Generic;
using ConveyorSystem;
using Unity.VisualScripting;
using UnityEngine;

public class FactoryExecutionManager : MonoBehaviour
{
	public static FactoryExecutionManager Instance { get; private set; }
	private List<ConveyorSegment> _conveyorSegments;

	//private readonly float _tickRate = 0.5f;
	//private float _timer = 0f;
	
	private void Awake()
	{
		Instance = this;
		_conveyorSegments = new List<ConveyorSegment>();
	}

	// Update is called once per frame
    void Update()
    {
	    //_timer -= Time.deltaTime;
	    Tick();
	    //if (_timer <= 0f)
	    //{
		//    _timer = _tickRate;
	    //}
    }

    public void AddSegment(ConveyorSegment segment)
    {
	    if (segment is null) return;
	    
	    _conveyorSegments.Add(segment);
    }

    public void Tick()
    {
	    foreach (ConveyorSegment segment in _conveyorSegments)
	    {
		    segment.Tick();
	    }
    }

    public void UpdateAllSegments()
    {
	    foreach (ConveyorSegment segment in _conveyorSegments)
	    {
		    segment.UpdateSegment();
	    }
    }
}
