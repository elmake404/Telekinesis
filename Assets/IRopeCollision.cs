using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRopeCollision
{
    TypeOfConnected GetTypeOfConnected();


    int GetUniqueID();
    void InitConnect();
    void BreakRope();
    void SetWithRopeConnected(ConnectedPin connectedPin);
}
