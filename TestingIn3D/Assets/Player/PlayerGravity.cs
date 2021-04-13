using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    // Bry dig ej om detta
    Transform CelestialBodies;
    CelestialBodiesInOrbit celestialBodiesInOrbit;
    public Vector3 startVelocity;
    private SunLight sunLight;

    // Bry dig om detta
    Rigidbody rb;
    [SerializeField]
    private float gravityConstForPlayer = -12f;
    [SerializeField]
    private float standingUprightSlerpSpeed = 50f;
    [SerializeField]
    private float lookSlerpSpeed = 10f;

    private Transform camMainTransform;
    private Transform camPosTransform;

    void Start()
    {
        // Skit i detta
        CelestialBodies = GameObject.Find("Celestial Bodies").transform;
        celestialBodiesInOrbit = CelestialBodies.GetComponent<CelestialBodiesInOrbit>();
        sunLight = GameObject.Find("Directional Light").GetComponent<SunLight>();
        //rb.AddForce(startVelocity, ForceMode.Acceleration);

        // Tänk på detta
        rb = GetComponent<Rigidbody>();
        camMainTransform = GameObject.Find("Main Camera").transform;
        camPosTransform = GameObject.Find("PlayerCamPos").transform;
    }

    public void UpdatePlayerVelocity(float timeStep)
    {
        // Behöver ej ändra eller fucka runt med
        float accelerationSqrMagnitudeNow = 0f;
        Vector3 forceDirNow = Vector3.zero;
        Transform nearestChild = CelestialBodies.transform;
        foreach (Transform child in CelestialBodies) // gå igenom transforms och celestialBodyManagers istället
        {
            float sqrDist = (child.position - transform.position).sqrMagnitude;
            Vector3 forceDir = (child.position - transform.position).normalized;
            Vector3 acceleration = forceDir * celestialBodiesInOrbit.gravitationalConstant * child.GetComponent<CelestialBodyManager>().sphereSettings.mass / sqrDist;

            if (acceleration.sqrMagnitude > accelerationSqrMagnitudeNow)
            {
                accelerationSqrMagnitudeNow = acceleration.sqrMagnitude;
                forceDirNow = -forceDir;
                nearestChild = child;
            }
            //rb.AddForce(acceleration * timeStep * System.Convert.ToSingle(!isGrounded), ForceMode.Acceleration);
            //gameObject.GetComponent<Rigidbody>().AddForce(acceleration * timeStep);
            //Debug.Log(acceleration);
        }
        //sunLight.ChangePlanetFollow(nearestChild); // DONT DELETE

        // Allt nedan kan du ändra/fucka runt med
        camPosTransform.position = transform.position;

        Vector3 gravityUp = (camPosTransform.position - nearestChild.position).normalized;
        Vector3 localUp = camPosTransform.up;

        rb.AddForce(gravityUp * gravityConstForPlayer);

        Quaternion standingUpright = Quaternion.FromToRotation(localUp, gravityUp) * camPosTransform.rotation;
        camPosTransform.rotation = Quaternion.Slerp(camPosTransform.rotation, standingUpright, standingUprightSlerpSpeed * Time.deltaTime);

        Quaternion lookDirectionLocalUp = Quaternion.FromToRotation(camMainTransform.up, camPosTransform.up) * camMainTransform.rotation;
        Quaternion targetLookDirection = Quaternion.FromToRotation(camPosTransform.forward, lookDirectionLocalUp * Vector3.forward) * camPosTransform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, lookSlerpSpeed * Time.deltaTime);
    }
}
