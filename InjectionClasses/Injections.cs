using UnityEngine;
using InternalModBot;

public class FixSpidertrons : Character
{
    /// <summary>
    /// used by the injector to copy the msil from and paste it into the real function
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetPositionForAIToAimAt()
    {
        return CalledFromInjections.FromGetPositionForAIToAimAt(this);
    }
}