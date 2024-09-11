using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Transform followTarget;

    private Vector2 startingPosition;
    private float startingZ;

    private Vector2 CamMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    private float ZDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    private float ClippingPlane => cam.transform.position.z + (ZDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane);

    private float ParallaxFactor => Mathf.Abs(ZDistanceFromTarget) / ClippingPlane;

    private void Start()
    {
        if (cam == null || followTarget == null)
        {
            Debug.LogError("Camera or followTarget is not assigned in the inspector.");
            enabled = false; // Disable the script if required components are not assigned
            return;
        }

        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    private void Update()
    {
        Vector2 newPosition = startingPosition + CamMoveSinceStart * ParallaxFactor;
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
