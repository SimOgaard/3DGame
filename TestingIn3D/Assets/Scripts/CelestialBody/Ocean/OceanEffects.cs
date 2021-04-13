using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// set this material out of script, not on update.

public class OceanEffects : MonoBehaviour
{
    //public Material[] oceanMaterials;
    //public int oceanToRender;
    //private CelestialBodyManager[] celestialBodyManagers;
    private Material materialToRender;
    //public Transform transformToRender;

    private void Start()
    {
        //materialToRender = GameObject.Find("Directional Light").GetComponent<SunLight>().planetFollow.GetComponent<CelestialBodyManager>().oceanMaterial;
        //celestialBodyManagers = GameObject.Find("Celestial Bodies").GetComponent<CelestialBodiesInOrbit>().celestialBodyManagers;

        /*
        int matAmount = 0;
        for (int i = 0; i < celestialBodyManagers.Length; i++)
        {
            Debug.Log(celestialBodyManagers[i].oceanRadius);
            if(celestialBodyManagers[i].oceanRadius != 0)
            {
                matAmount++;
            }
        }
        oceanMaterials = new Material[matAmount];

        matAmount = 0;
        for (int i = 0; i < celestialBodyManagers.Length; i++)
        {
            if (celestialBodyManagers[i].oceanRadius != 0)
            {
                oceanMaterials[matAmount++] = celestialBodyManagers[i].oceanMaterial;
            }
        }
        */
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {        
        //materialToRender = transformToRender.GetComponent<CelestialBodyManager>().oceanMaterial;
        Graphics.Blit(source, destination, materialToRender);
        /*for (int i = 0; i < celestialBodyManagers.Length; i++)
        {
            if (celestialBodyManagers[i].oceanRadius != 0 && i == oceanToRender)
            {
//                Debug.Log(celestialBodyManagers[i].oceanRadius);
                Graphics.Blit(source, destination, celestialBodyManagers[i].oceanMaterial);
//                Graphics.CopyTexture(source, destination);
            }
        }*/
    }

    public void ChangeMaterialToRender(Transform transform)
    {
        Material mat = transform.GetComponent<CelestialBodyManager>().oceanMaterial;
        if (mat != materialToRender)
        {
            materialToRender = mat;
        }
    }
}
