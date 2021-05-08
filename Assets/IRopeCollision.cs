using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRopeCollision
{
    TypeOfConnected GetTypeOfConnected();
    void InitConnect();
    void BreakRope(Vector3 sourceExplosion);
    void SetWithRopeConnected(ConnectedPin connectedPin);
}
