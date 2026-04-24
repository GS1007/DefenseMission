using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helicopter Crash", menuName = "DamageSystem/Aircraft/Crash/Jet")]
public class JetCrashSO : CrashBehaviorSO
{
    [SerializeField] private float _crashForce = 0f;
    [SerializeField] private float _crashVerticalTorque = 0f;
    [SerializeField] private float _crashHorizontalTorque = 0f;

    public override IEnumerator CrashRoutine(Rigidbody rb, Func<bool> checkHitGround)
    {
        rb.AddForce(rb.transform.forward * _crashForce, ForceMode.VelocityChange);

        while (!checkHitGround())
        {
            rb.AddTorque(rb.transform.forward * _crashVerticalTorque +
                         rb.transform.right * _crashHorizontalTorque, ForceMode.Acceleration);

            yield return new WaitForFixedUpdate();
        }
    }
}
