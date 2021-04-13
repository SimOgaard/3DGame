using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLight : MonoBehaviour
{
    // skapa ett nytt layer
    // sätt planeten i den nya layern
    // minns den layern
    // skapa directional light
    // lägg till layern som en mask på directional light
    // orientera directional light från vector3.zero till transform.pos
    // pass in directional light worldpos i shader som du gör med ocean.center



    // kom på ett sätt att lägga in array med värden till en ocean shader, itterera

        
    //public Transform planetFollow;
    //private OceanEffects oceanEffects;
    
    private Transform[] dirLight;
    private Transform[] planetsPos;
    public int layerStart = 10;

    private void Awake()
    {
        //oceanEffects = GameObject.Find("Main Camera").GetComponent<OceanEffects>();

        dirLight = new Transform[GameObject.Find("Celestial Bodies").transform.childCount - 1];
        planetsPos = new Transform[GameObject.Find("Celestial Bodies").transform.childCount - 1];
        foreach (Transform child in GameObject.Find("Celestial Bodies").transform)
        {
            if (child != transform.parent)
            {
                child.GetComponent<CelestialBodyManager>().layer = layerStart;
                //SetLayerRecursively(child.gameObject, layerStart);

                GameObject directionalLightComponent = new GameObject();
                directionalLightComponent.AddComponent(typeof(Light));
                directionalLightComponent.GetComponent<Light>().type = LightType.Directional;
                directionalLightComponent.GetComponent<Light>().cullingMask =  1 << layerStart; // AAAAAAAAAH IM GOING INSANE
                directionalLightComponent.name = "Directional_Light_" + (layerStart - 10);
                directionalLightComponent.transform.parent = transform;
                directionalLightComponent.gameObject.layer = layerStart;

                dirLight[layerStart - 10] = directionalLightComponent.transform;
                planetsPos[layerStart - 10] = child;

                layerStart++;
            }
        }
    }

    /*
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    */

    private void Update()
    {
        for (int i = 0; i < dirLight.Length; i++)
        {
            dirLight[i].rotation = Quaternion.FromToRotation(dirLight[i].forward, planetsPos[i].position) * dirLight[i].rotation;
        }

        //transform.rotation = Quaternion.FromToRotation(transform.forward, planetFollow.position) * transform.rotation;
    }
    /*
    public void ChangePlanetFollow(Transform transform)
    {
        //planetFollow = transform;
        oceanEffects.ChangeMaterialToRender(transform);
    }*/
}
