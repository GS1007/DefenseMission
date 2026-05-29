using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helicopter Crash", menuName = "DamageSystem/Aircraft/Crash/Helicopter")]
public class HelicopterCrashSO : CrashBehaviorSO
{
    [SerializeField] private float _crashForce = 0f;
    [SerializeField] private float _crashTorque = 0f;

    public override IEnumerator CrashRoutine(Rigidbody rb, Func<bool> checkHitGround)
    {
        rb.AddForce(rb.transform.forward * _crashForce, ForceMode.VelocityChange);

        while (checkHitGround() == false)
        {
            rb.AddTorque(Vector3.up * _crashTorque, ForceMode.Acceleration);

            yield return new WaitForFixedUpdate();
        }
    }
}
