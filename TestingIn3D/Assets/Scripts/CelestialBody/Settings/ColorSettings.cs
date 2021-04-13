using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public enum PlanetType { FirstPlanet, SecondPlanet, Moon, Sun };
    public PlanetType planetType;

    public FirstPlanet firstPlanet;
    public SecondPlanet secondPlanet;
    public Moon moon;
    public Sun sun;

    [System.Serializable]
    public class FirstPlanet
    {
        [HideInInspector]
        public Material celestialBodyMaterial;
        public Ocean ocean;
        public Shore shore;
        public Biomes biomes;
        public Mountains mountains;

        [System.Serializable]
        public class Ocean
        {
            [HideInInspector]
            public Material oceanMaterial;

            public Color OceanDeep = new Color(1f, 1f, 1f, 1f);
            public Color OceanShallow = new Color(1f, 1f, 1f, 1f);

            public float OceanRadius;

            public float DepthMultiplier;
            public float AlphaMultiplier;

            [Range(0,1)]
            public float Smoothness;
        }

        [System.Serializable]
        public class Shore
        {
            public Color DryShoreColor = new Color(1f, 1f, 1f, 1f);
            public Color DampShoreColor = new Color(1f, 1f, 1f, 1f);

            [Range(0, 1)]
            public float DryShoreHeightAboveWater = 0.1f;
            [Range(0, 1)]
            public float DampShoreHeightAboveWater = 0.1f;
            [Range(0, 1)]
            public float OceanBlend = 0.5f;
            [Range(0, 1)]
            public float ShoreBlend = 0.5f;
        }

        [System.Serializable]
        public class Biomes
        {
            public Color BiomeALow = new Color(1f, 1f, 1f, 1f);
            public Color BiomeAHigh = new Color(1f, 1f, 1f, 1f);
            /*
            public Color BiomeBLow;
            public Color BiomeBHigh;

            public Color BiomeCLow;
            public Color BiomeCHigh;

            public Color BiomeDLow;
            public Color BiomeDHigh;
            */
            [Range(0, 1)]
            public float BiomeHeightAboveShore;
            [Range(0, 1)]
            public float FlatColBlend;
            /*
            public float FlatColBlendNoise;
            public float MaxFlatHeight;

            public Texture2D NoiseTex;
            public float NoiseScale;
            public float NoiseScale2;
            */
        }

        [System.Serializable]
        public class Mountains
        {
            public Color MountainLow = new Color(1f, 1f, 1f, 1f);
            public Color MountainHigh = new Color(1f, 1f, 1f, 1f);

            public float MaxFlatHeight;
            public float MountainTopBlend;
            public float SteepnessThresholdLow;
            public float SteepnessThresholdHigh;
            public float MountainBlend;
            public float HighUpMountainSteepnessDistribution;
            //public float SteepBands;
            //public float SteepBandsStrength;
            //public float FlatToSteepNoise;
        }
    }

    [System.Serializable]
    public class SecondPlanet
    {
        [HideInInspector]
        public Material celestialBodyMaterial;
        public Ocean ocean;

        [System.Serializable]
        public class Ocean
        {
            [HideInInspector]
            public Material oceanMaterial;

            public Color OceanDeep = new Color(1f, 1f, 1f, 1f);
            public Color OceanShallow = new Color(1f, 1f, 1f, 1f);

            public float OceanRadius;

            public float DepthMultiplier;
            public float AlphaMultiplier;

            [Range(0, 1)]
            public float Smoothness;
        }
    }

    [System.Serializable]
    public class Moon
    {
        [HideInInspector]
        public Material celestialBodyMaterial;
        public NormalMap normalMap;

        [System.Serializable]
        public class NormalMap
        {
            public Texture2D NormalMapTexture;

            //public float ScaleTexture;
            public float BlendSharpness;
            public float ScaleNormalMap;
        }
    }

    [System.Serializable]
    public class Sun
    {
        [HideInInspector]
        public Material celestialBodyMaterial;

        public Color SunColor1 = new Color(1f, 1f, 1f, 1f);
        public Color SunColor2 = new Color(1f, 1f, 1f, 1f);

        public float Frequency = 2;
        public float SunNoiseSpeed = 10;
    }
}
