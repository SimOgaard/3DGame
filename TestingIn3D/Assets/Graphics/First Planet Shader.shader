﻿Shader "Planet/First"
{
    Properties
    {
        _MainTex ("texture", 2D) = "white" {}

		// Shore
		_DampShoreColor ("Damp Shore Color", Color) = (1,1,1,1)
		_DryShoreColor ("Dry Shore Color", Color) = (1,1,1,1)
		_OceanRadius ("Ocean Radius", Float) = 5.0
		_DampShoreHeightAboveWater ("Damp Shore Height Above Water", Float) = 0.5
		_DryShoreHeightAboveWater ("Dry Shore Height Above Water", Float) = 0.5
		_ShoreBlend ("Blending Of Shore", Float) = 0.5
		_OceanBlend ("Blending Underneath Ocean", Float) = 0.5

		// Biome
		_FlatColBlend ("Blending Of Biomes", Float) = 0.5
		_BiomeHeightAboveShore ("Biome Height Above Shore", Float) = 0.5
		_BiomeALow ("Biome A Color", Color) = (1,1,1,1)
		_BiomeAHigh ("Biome B Color", Color) = (1,1,1,1)

		// Mountain
		_MaxFlatHeight ("Maximum Height For Biomes", Float) = 0.5
		_SteepnessThresholdLow ("Steepness Threshold Low", Float) = 0.5
		_SteepnessThresholdHigh ("Steepness Threshold High", Float) = 0.5
		_MountainBlend ("Mountain Blend", Float) = 0.5
		_MountainTopBlend ("Mountain Top Blend", Float) = 0.5
		_HighUpMountainSteepnessDistribution ("High Up Mountain Steepness Distribution", Float) = 0.1
		_MountainLow ("Mountain Low Color", Color) = (1,1,1,1)
		_MountainHigh ("Mountain High Color", Color) = (1,1,1,1)
    }
    SubShader
    {
		Tags { "RenderType"="Opaque" }
		LOD 200

        CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.5

        #include "UnityCG.cginc"

        struct Input
        {
			float2 uv_MainTex;
			float3 worldPos;
			float4 terrainData;
			float3 vertPos;
			float3 normal;
			float4 tangent;
        };

        sampler2D _MainTex;

		// Shore
		fixed4 _DampShoreColor;
		fixed4 _DryShoreColor;
		fixed _OceanRadius;
		fixed _DampShoreHeightAboveWater;
		fixed _DryShoreHeightAboveWater;
		fixed _ShoreBlend;
		fixed _OceanBlend;

		// Biome
		float _BiomeHeightAboveShore;
		float _FlatColBlend;
		fixed4 _BiomeALow;
		fixed4 _BiomeAHigh;

		// Mountain
		float _MaxFlatHeight;
		float _SteepnessThresholdLow;
		float _SteepnessThresholdHigh;

		float _MountainBlend;
		float _MountainTopBlend;
		float _HighUpMountainSteepnessDistribution;
		
		fixed4 _MountainLow;
		fixed4 _MountainHigh;
        
		// börja blenda, hur långt den ska blenda, vart den är just nu
		float Blend(float startHeight, float blendDst, float height) {
			 return smoothstep(startHeight - blendDst / 2, startHeight + blendDst / 2, height);
		}

		float remap01(float v, float minOld, float maxOld)
		{
			return saturate((v-minOld) / (maxOld-minOld));
		}

		void vert (inout appdata_full v, out Input o)
        {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertPos = v.vertex;
			o.normal = v.normal;
			o.terrainData = v.texcoord;
			o.tangent = v.tangent;
        }

		void surf (Input IN, inout SurfaceOutputStandard o)
        {
			// Beräkna steepness: 0 = flat, 1 = steep
			float3 sphereNormal = normalize(IN.vertPos);
			float steepness = 1 - dot(sphereNormal, IN.normal);

			// Beräkna heights
			float terrainHeight = length(IN.vertPos);
			float dampShoreHeight = _OceanRadius + _DampShoreHeightAboveWater;
			float dryShoreHeight = dampShoreHeight + _DryShoreHeightAboveWater;
			float biomeThreshold = dryShoreHeight + _BiomeHeightAboveShore;
			float mountainThreshold = biomeThreshold + _MaxFlatHeight;

			// colours
			fixed4 shoreColour = lerp(_DampShoreColor, _DryShoreColor, Blend(dampShoreHeight, _OceanBlend, terrainHeight));
			fixed4 biomeColour = lerp(_BiomeALow, _BiomeAHigh, Blend(biomeThreshold, _FlatColBlend, terrainHeight));

			float4 mountainColour = lerp(biomeColour, _MountainLow, Blend(_SteepnessThresholdLow, _MountainBlend, steepness));
			mountainColour = lerp(mountainColour, _MountainHigh, Blend(_SteepnessThresholdHigh, _MountainBlend, steepness));

			fixed4 biomeShoreBlend = lerp(shoreColour, mountainColour, Blend(dryShoreHeight, _ShoreBlend, terrainHeight));
			fixed4 mountainTop = lerp(biomeShoreBlend, lerp(_MountainLow, _MountainHigh, Blend(_HighUpMountainSteepnessDistribution, _MountainBlend, steepness)), Blend(mountainThreshold, _MountainTopBlend, terrainHeight));
			//fixed4 mountainBlend = lerp(mountainColour, biomeColour, Blend(biomeThreshold, _MountainBlend, terrainHeight));

			float4 terrainColour = mountainTop;

			// ändra materialets färger
			o.Albedo = terrainColour;
//			o.Smoothness = dot(o.Albedo, 1) / 3 * _Smoothness;
//			o.Metallic = _Metallic;
        }
        ENDCG
    }
	FallBack "Diffuse"
}
