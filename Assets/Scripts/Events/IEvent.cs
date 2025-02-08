using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    public void StartEvent(List<CatController> cats);
    public void StopEvent();
}
