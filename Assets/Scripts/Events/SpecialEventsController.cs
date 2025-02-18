using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpecialEventsController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _eventsGameObjects;

    private List<IEvent> _events = new List<IEvent>();
    private IEvent _currnetEvent;
    private const float EVENT_LENGHT = 20;

    private void Start()
    {
        foreach (var eventObject in _eventsGameObjects)
        {
            var spEvent = eventObject.GetComponent<IEvent>();
            if (spEvent != null)
            {
                _events.Add(spEvent);
            }
        }
    }

    public void StartRandomEvent(List<CatController> cats)
    {
        var r = Random.Range(0, _events.Count);
        _currnetEvent = _events[r];
        _currnetEvent.StartEvent(cats);
        StartTimer();
    }

    private void StartTimer()
    {
        DOVirtual.DelayedCall(EVENT_LENGHT, () =>
        {
            _currnetEvent.StopEvent();
        });
    }
}
