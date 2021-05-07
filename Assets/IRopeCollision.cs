using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRopeCollision
{

    void InitConnect();
    void BreakRope(Vector3 sourceExplosion);
    void SetWithRopeConnected(ConnectedPin connectedPin);
}
