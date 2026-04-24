using System.Collections;
using UnityEngine;

public abstract class CrashBehaviorSO : ScriptableObject
{
    public abstract IEnumerator CrashRoutine(Rigidbody rb, System.Func<bool> checkHitGround);
}
