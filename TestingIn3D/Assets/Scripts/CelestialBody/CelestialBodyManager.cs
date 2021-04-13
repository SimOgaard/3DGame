using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyManager : MonoBehaviour
{
    [TextArea]
    public string Notes = "";

    //    public ComputeShader computeShader;
    private Mesh[] mesh = new Mesh[20];

    private CreateDetailedMesh createDetailedMesh;
    [HideInInspector]
    public static Vector3[][] vectors = new Vector3[20][];
    private static Vector3[][] vectors_saved = new Vector3[20][];
    [HideInInspector]
    public static int[][] triangles = new int[20][];

    private SimpleSphereProjection simpleSphereProjection;
    public TerrainProjection terrainProjection;
    public TerrainProjectionGPU terrainProjectionGPU;

    public FaunaProjection faunaProjection;

    public ColorSettings colorSettings;
    public SphereSettings sphereSettings;
    public ShapeSettings shapeSettings;

    private FaunaTerrainSettings faunaTerrainSettings;
    public FaunaSettings faunaSettings;

    [HideInInspector]
    public bool colorSettingsFoldout;
    [HideInInspector]
    public bool sphereSettingsFoldout;
    [HideInInspector]
    public bool terrainSettingsFoldout;
    [HideInInspector]
    public bool faunaSettingsFoldout;

    public CelestialBodyOrbit celestialBodyOrbit;

    private GameObject childComponentForMeshes;

    public IColor planetColor;

    [HideInInspector]
    public Material planetMaterial;

    [HideInInspector]
    public float oceanRadius;

    [HideInInspector]
    public Material oceanMaterial;

    [HideInInspector]
    public Texture2D[] noiseTextures;

    public int layer;

    private void Start()
    {
        /*
        noiseTextures = new Texture2D[4];
        for (int i = 0; i < 4; i++)
        {
            noiseTextures[i] = new Texture2D(128, 128);
        }
        */

        transform.position = sphereSettings.worldPos;

        childComponentForMeshes = new GameObject();
        childComponentForMeshes.transform.parent = gameObject.transform;
        childComponentForMeshes.name = "AllMeshes";
/*
        childComponentForMeshes.AddComponent(typeof(Rigidbody));
        childComponentForMeshes.GetComponent<Rigidbody>().useGravity = false;
        //childComponentForMeshes.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        //childComponentForMeshes.GetComponent<Rigidbody>().isKinematic = true;
        childComponentForMeshes.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
*/
        childComponentForMeshes.transform.position = transform.gameObject.transform.position;

        for (int i = 0; i < 20; i++)
        {
            GameObject childMeshComponent = new GameObject();
            childMeshComponent.layer = layer;
            childMeshComponent.transform.position = childComponentForMeshes.gameObject.transform.position;

            childMeshComponent.AddComponent(typeof(MeshFilter));
            childMeshComponent.AddComponent(typeof(MeshRenderer));

            /* FOR MOVING MESH
            childMeshComponent.AddComponent(typeof(Rigidbody));
            childMeshComponent.GetComponent<Rigidbody>().useGravity = false;
            childMeshComponent.GetComponent<Rigidbody>().angularDrag = 0;
            //childMeshComponent.GetComponent<Rigidbody>().isKinematic = false;
            childMeshComponent.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            childMeshComponent.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            childMeshComponent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //childMeshComponent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            childMeshComponent.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
            childMeshComponent.GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
            */

            childMeshComponent.AddComponent<MeshCollider>();
            //childMeshComponent.GetComponent<MeshCollider>().convex = true; FOR MOVING MESH
            childMeshComponent.name = "Mesh_" + i;
            childMeshComponent.transform.parent = childComponentForMeshes.transform;

            Mesh childMesh = new Mesh()
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };
            childMesh.MarkDynamic();
            childMeshComponent.gameObject.GetComponent<MeshFilter>().mesh = childMesh;
            childMeshComponent.GetComponent<MeshCollider>().sharedMesh = childMeshComponent.gameObject.GetComponent<MeshFilter>().mesh;

            mesh[i] = childMesh;
        }

        celestialBodyOrbit = new CelestialBodyOrbit(sphereSettings);

        createDetailedMesh = new CreateDetailedMesh(sphereSettings);
        GenerateCelestialBody();
        OnColorSettingsUpdated();
    }

    public void OnSphereSettingsUpdated()
    {
        GenerateCelestialBody();
    }

    public void GenerateCelestialBody()
    {
        createDetailedMesh.CreateDetailedTriangles();
        vectors = createDetailedMesh.GetVectors();
        triangles = createDetailedMesh.GetTriangles();
        simpleSphereProjection = new SimpleSphereProjection();

        for (int i = 0; i < 20; i++)
        {
            vectors_saved[i] = (Vector3[])vectors[i].Clone();
        }

        terrainProjectionGPU = new TerrainProjectionGPU(shapeSettings, sphereSettings, ref noiseTextures);
        ProjectFauna();
        //        terrainProjection = new TerrainProjection(shapeSettings, sphereSettings);
        RenderMyMesh();
    }

    public void OnTerrainSettingsUpdated()
    {
        for (int i = 0; i < 20; i++)
        {
            vectors[i] = (Vector3[])vectors_saved[i].Clone();
        }

        terrainProjectionGPU = new TerrainProjectionGPU(shapeSettings, sphereSettings, ref noiseTextures);
        ProjectFauna();
        //        terrainProjection = new TerrainProjection(shapeSettings, sphereSettings);
        RenderMyMesh();
    }

    public void OnFaunaSettingsUpdated()
    {
        ProjectFauna();
    }

    public void ProjectFauna()
    {
        faunaProjection = new FaunaProjection(faunaTerrainSettings, faunaSettings, gameObject, oceanRadius * oceanRadius, layer);
    }

    void RenderMyMesh()
    {
        for (int i = 0; i < 20; i++)
        {
            mesh[i].Clear();

            mesh[i].vertices = vectors[i];
            mesh[i].triangles = triangles[i];

            mesh[i].RecalculateNormals();
            mesh[i].RecalculateTangents();
            mesh[i].RecalculateBounds();
        }
    }

    public void OnColorSettingsUpdated()
    {
        planetColor = ColorFactory.CreateColor(colorSettings);
        oceanRadius = planetColor.GetOceanRadius();
        oceanMaterial = planetColor.GetOceanMaterial();
        planetMaterial = planetColor.GetColors();

        foreach (Transform child in childComponentForMeshes.transform)
        {
            child.gameObject.GetComponent<MeshRenderer>().sharedMaterial = planetMaterial;
        }

        ProjectFauna();
    }

    public interface IColor
    {
        Material GetColors();
        float GetOceanRadius();
        Material GetOceanMaterial();
    }

    public static class ColorFactory
    {
        public static IColor CreateColor(ColorSettings colorSettings)
        {
            switch (colorSettings.planetType)
            {
                case ColorSettings.PlanetType.FirstPlanet:
                    return new FirstPlanetColor(colorSettings.firstPlanet);
                case ColorSettings.PlanetType.SecondPlanet:
                    return new SecondPlanetColor(colorSettings.secondPlanet);
                case ColorSettings.PlanetType.Moon:
                    return new MoonColor(colorSettings.moon);
                case ColorSettings.PlanetType.Sun:
                    return new SunColor(colorSettings.sun);
            }
            return null;
        }
    }

    public class FirstPlanetColor : IColor
    {
        ColorSettings.FirstPlanet colorSettings;

        public FirstPlanetColor(ColorSettings.FirstPlanet colorSettings)
        {
            this.colorSettings = colorSettings;
            //colorSettings.celestialBodyMaterial = Resources.Load("PlanetMaterials/First Planet Material", typeof(Material)) as Material;
            //colorSettings.ocean.oceanMaterial = Resources.Load("PlanetMaterials/Ocean Material First", typeof(Material)) as Material;

            colorSettings.celestialBodyMaterial = new Material(Shader.Find("Planet/First"));
            colorSettings.ocean.oceanMaterial = new Material(Shader.Find("Planet/Ocean"));

            // Ocean
            colorSettings.ocean.oceanMaterial.SetColor("_OceanDeep", colorSettings.ocean.OceanDeep);
            colorSettings.ocean.oceanMaterial.SetColor("_OceanShallow", colorSettings.ocean.OceanShallow);
            colorSettings.ocean.oceanMaterial.SetFloat("_OceanRadius", colorSettings.ocean.OceanRadius);
            colorSettings.ocean.oceanMaterial.SetFloat("_DepthMultiplier", colorSettings.ocean.DepthMultiplier);
            colorSettings.ocean.oceanMaterial.SetFloat("_AlphaMultiplier", colorSettings.ocean.AlphaMultiplier);
            colorSettings.ocean.oceanMaterial.SetFloat("_Smoothness", colorSettings.ocean.Smoothness);

            // Shore
            colorSettings.celestialBodyMaterial.SetColor("_DampShoreColor", colorSettings.shore.DampShoreColor);
            colorSettings.celestialBodyMaterial.SetColor("_DryShoreColor", colorSettings.shore.DryShoreColor);

            colorSettings.celestialBodyMaterial.SetFloat("_OceanRadius", colorSettings.ocean.OceanRadius);
            colorSettings.celestialBodyMaterial.SetFloat("_DampShoreHeightAboveWater", colorSettings.shore.DampShoreHeightAboveWater);
            colorSettings.celestialBodyMaterial.SetFloat("_DryShoreHeightAboveWater", colorSettings.shore.DryShoreHeightAboveWater);
            colorSettings.celestialBodyMaterial.SetFloat("_ShoreBlend", colorSettings.shore.ShoreBlend);
            colorSettings.celestialBodyMaterial.SetFloat("_OceanBlend", colorSettings.shore.OceanBlend);

            // Biome
            colorSettings.celestialBodyMaterial.SetFloat("_FlatColBlend", colorSettings.biomes.FlatColBlend);
            colorSettings.celestialBodyMaterial.SetFloat("_BiomeHeightAboveShore", colorSettings.biomes.BiomeHeightAboveShore);
            colorSettings.celestialBodyMaterial.SetColor("_BiomeALow", colorSettings.biomes.BiomeALow);
            colorSettings.celestialBodyMaterial.SetColor("_BiomeAHigh", colorSettings.biomes.BiomeAHigh);

            // Mountain
            colorSettings.celestialBodyMaterial.SetColor("_MountainLow", colorSettings.mountains.MountainLow);
            colorSettings.celestialBodyMaterial.SetColor("_MountainHigh", colorSettings.mountains.MountainHigh);
            colorSettings.celestialBodyMaterial.SetFloat("_MountainTopBlend", colorSettings.mountains.MountainTopBlend);
            colorSettings.celestialBodyMaterial.SetFloat("_MaxFlatHeight", colorSettings.mountains.MaxFlatHeight);
            colorSettings.celestialBodyMaterial.SetFloat("_SteepnessThresholdLow", colorSettings.mountains.SteepnessThresholdLow);
            colorSettings.celestialBodyMaterial.SetFloat("_SteepnessThresholdHigh", colorSettings.mountains.SteepnessThresholdHigh);
            colorSettings.celestialBodyMaterial.SetFloat("_MountainBlend", colorSettings.mountains.MountainBlend);
            colorSettings.celestialBodyMaterial.SetFloat("_HighUpMountainSteepnessDistribution", colorSettings.mountains.HighUpMountainSteepnessDistribution);
        }

        public Material GetColors()
        {
            return colorSettings.celestialBodyMaterial;
        }

        public float GetOceanRadius()
        {
            return colorSettings.ocean.OceanRadius;
        }

        public Material GetOceanMaterial()
        {
            return colorSettings.ocean.oceanMaterial;
        }
    }

    public class SecondPlanetColor : IColor
    {
        ColorSettings.SecondPlanet colorSettings;

        public SecondPlanetColor(ColorSettings.SecondPlanet colorSettings)
        {
            this.colorSettings = colorSettings;
            //colorSettings.celestialBodyMaterial = Resources.Load("PlanetMaterials/Second Planet Material", typeof(Material)) as Material;
            //colorSettings.ocean.oceanMaterial = Resources.Load("PlanetMaterials/Ocean Material Second", typeof(Material)) as Material;
            colorSettings.celestialBodyMaterial = new Material(Shader.Find("Planet/First"));
            colorSettings.ocean.oceanMaterial = new Material(Shader.Find("Planet/Ocean"));

            colorSettings.ocean.oceanMaterial.SetColor("_OceanDeep", colorSettings.ocean.OceanDeep);
            colorSettings.ocean.oceanMaterial.SetColor("_OceanShallow", colorSettings.ocean.OceanShallow);
            colorSettings.ocean.oceanMaterial.SetFloat("_OceanRadius", colorSettings.ocean.OceanRadius);
            colorSettings.ocean.oceanMaterial.SetFloat("_DepthMultiplier", colorSettings.ocean.DepthMultiplier);
            colorSettings.ocean.oceanMaterial.SetFloat("_AlphaMultiplier", colorSettings.ocean.AlphaMultiplier);
            colorSettings.ocean.oceanMaterial.SetFloat("_Smoothness", colorSettings.ocean.Smoothness);
        }

        public Material GetColors()
        {
            return colorSettings.celestialBodyMaterial;
        }

        public float GetOceanRadius()
        {
            return colorSettings.ocean.OceanRadius;
        }

        public Material GetOceanMaterial()
        {
            return colorSettings.ocean.oceanMaterial;
        }
    }

    public class MoonColor : IColor
    {
        ColorSettings.Moon colorSettings;

        public MoonColor(ColorSettings.Moon colorSettings)
        {
            this.colorSettings = colorSettings;
            colorSettings.celestialBodyMaterial = new Material(Shader.Find("Planet/Moon"));
            //colorSettings.celestialBodyMaterial = Resources.Load("PlanetMaterials/Moon Material", typeof(Material)) as Material;

            colorSettings.celestialBodyMaterial.SetTexture("_NormalMapA", colorSettings.normalMap.NormalMapTexture);
            //colorSettings.celestialBodyMaterial.SetFloat("_ScaleTexture", colorSettings.normalMap.ScaleTexture);
            colorSettings.celestialBodyMaterial.SetFloat("_BlendSharpness", colorSettings.normalMap.BlendSharpness);
            colorSettings.celestialBodyMaterial.SetFloat("_ScaleNormalMapA", colorSettings.normalMap.ScaleNormalMap);
        }

        public Material GetColors()
        {
            return colorSettings.celestialBodyMaterial;
        }

        public float GetOceanRadius()
        {
            return 0;
        }

        public Material GetOceanMaterial()
        {
            return null;
        }
    }

    public class SunColor : IColor
    {
        ColorSettings.Sun colorSettings;

        public SunColor(ColorSettings.Sun colorSettings)
        {
            this.colorSettings = colorSettings;
            colorSettings.celestialBodyMaterial = new Material(Shader.Find("Planet/Sun With Noise"));
            //colorSettings.celestialBodyMaterial = Resources.Load("PlanetMaterials/Sun With Noise Material", typeof(Material)) as Material;

            colorSettings.celestialBodyMaterial.SetColor("_SunColor1", colorSettings.SunColor1);
            colorSettings.celestialBodyMaterial.SetColor("_SunColor2", colorSettings.SunColor2);

            colorSettings.celestialBodyMaterial.SetFloat("_Frequency", colorSettings.Frequency);
            colorSettings.celestialBodyMaterial.SetFloat("_SunNoiseSpeed", colorSettings.SunNoiseSpeed);
        }

        public Material GetColors()
        {
            return colorSettings.celestialBodyMaterial;
        }

        public float GetOceanRadius()
        {
            return 0;
        }

        public Material GetOceanMaterial()
        {
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (vectors.Length == 0)
        {
            return;
        }

/*        for (int i = 0; i < vectors.Length; i++)
        {
            Gizmos.DrawSphere(vectors[i], 0.05f);
        }*/
/*
        Gizmos.color = Color.red;
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            int z = i * 3;
            Gizmos.DrawLine(vectors[triangles[z]], vectors[triangles[z + 1]]);
            Gizmos.DrawLine(vectors[triangles[z]], vectors[triangles[z + 2]]);
            Gizmos.DrawLine(vectors[triangles[z + 1]], vectors[triangles[z + 2]]);
        }
*/
    }
}
