using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 offsetPosition;
    [SerializeField]
    private Space offsetPositionSpace = Space.Self;
    [SerializeField]
    private bool lookAt = true;
    [SerializeField]
    private float damping;

    private void LateUpdate()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing target ref !", this);
            return;
        }

        // compute position
        if (offsetPositionSpace == Space.Self) transform.position = Vector3.Lerp(transform.position, target.TransformPoint(offsetPosition), Time.deltaTime * damping);
        else transform.position = Vector3.Lerp(transform.position, target.position + offsetPosition, Time.deltaTime * damping);

        // compute rotation
        if (lookAt)
        {
            transform.LookAt(target);
            transform.rotation = target.rotation;
        }
        else transform.rotation = target.rotation;
    }
}
