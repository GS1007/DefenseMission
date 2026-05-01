using System.Collections;
using UnityEngine;

public interface IStrela2MSeeker
{
    public void Track();

    public void ResetSeeker();

    public Transform GetHitPoint();
}
