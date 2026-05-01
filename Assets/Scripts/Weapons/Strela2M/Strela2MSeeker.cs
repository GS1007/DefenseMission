using System.Collections;
using UnityEngine;

public class Strela2MSeeker : MonoBehaviour, IStrela2MSeeker
{
    [SerializeField] private LayerMask _aircraftLayer;
    [SerializeField] private LayerMask _flareLayer;
    [SerializeField] private LayerMask _occlusionLayers;

    [SerializeField] private float _seekerFOV = 0f;
    [SerializeField] private float _lockRange = 0f;
    [SerializeField] private float _lockThreshold = 0f;
    [SerializeField] private float _flareHeatMultiplier = 0f;

    private Transform _currentTarget;

    private bool _lockConfirmed = false;
    private float _signalStrength = 0f;
    private float _scanInterval = 0.1f;
    private float _scanTimer = 0f;

    public void ResetSeeker()
    {
    }

    public void Track()
    {
        int combinedMask = _aircraftLayer.value | _flareLayer.value;
        Collider[] contacts = Physics.OverlapSphere(transform.position, _lockRange, combinedMask);

        Transform bestTarget = null;
        float highestSignal = 0f;

        foreach (Collider col in contacts)
        {
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            // Must be strictly inside the FOV cone
            if (angle > _seekerFOV / 2f) continue;

            // Must have line of sight
            if (Physics.Linecast(transform.position, col.transform.position, _occlusionLayers)) continue;

            float targetSignal = CalculateThermalSignature(col.gameObject, dirToTarget, angle);

            if (targetSignal > highestSignal)
            {
                highestSignal = targetSignal;
                bestTarget = col.transform;
            }
        }

        _currentTarget = bestTarget;
        _signalStrength = highestSignal;
        _lockConfirmed = _signalStrength > _lockThreshold;
    }

    public Transform GetHitPoint()
    {
        return _currentTarget;
    }

    private float CalculateThermalSignature(GameObject contact, Vector3 dirToTarget, float angle)
    {
        float reticleFactor = 1f - (angle / (_seekerFOV / 2f));
        float aspectFactor = 1f;
        float heatMultiplier = 1f;

        if (((1 << contact.layer) & _aircraftLayer.value) != 0)
        {
            aspectFactor = Mathf.Clamp01(Vector3.Dot(contact.transform.forward, dirToTarget));
        }
        else if (((1 << contact.layer) & _flareLayer.value) != 0)
        {
            aspectFactor = 1f;
            heatMultiplier = _flareHeatMultiplier;
        }

        return reticleFactor * aspectFactor * heatMultiplier;
    }
}
