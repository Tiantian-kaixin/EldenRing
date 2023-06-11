Shader "Custom/URP_BaseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("Base Color", color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0,1)) = 0
        _Metallic("Metallic", Range(0,1)) = 0

        _NoiseTex("noise Map", 2D) = "black" {}
		_FootprintTex("footprint Map", 2D) = "black" {}
        _SnowNormal("snow normal Map", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"            


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _BaseColor;
            float _Smoothness, _Metallic;

            sampler2D _NoiseTex;
			sampler2D _FootprintTex;
            sampler2D _SnowNormal;

            v2f vert (appdata v)
            {
                float4 footprint = tex2Dlod(_FootprintTex, float4(v.uv, 0, 0));
                float4 noise = tex2Dlod(_NoiseTex, float4(v.uv * 0.1, 0, 0));
                float4 snowNormal = tex2Dlod(_SnowNormal, float4(v.uv, 0, 0));
                // float3 snowSurface = float3(0, noise.r * 0.5, 0);
                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex.xyz) - float3(0, footprint.r, 0);// + snowSurface;
                o.normalWS = TransformObjectToWorldNormal(snowNormal.xyz) - float3(0, footprint.r, 0);//TransformObjectToWorldNormal(v.normal.xyz);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = TransformWorldToHClip(o.positionWS);

                OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUV );
    OUTPUT_SH(o.normalWS.xyz, o.vertexSH );

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                InputData inputdata = (InputData)0;
                inputdata.positionWS = i.positionWS;
                inputdata.normalWS = normalize(i.normalWS);
                inputdata.viewDirectionWS = i.viewDir;
                inputdata.bakedGI = SAMPLE_GI( i.lightmapUV, i.vertexSH, inputdata.normalWS );

                SurfaceData surfacedata;
                surfacedata.albedo = _BaseColor;
                surfacedata.specular = 0;
                surfacedata.metallic = _Metallic;
                surfacedata.smoothness = _Smoothness;
                surfacedata.normalTS = 0;
                surfacedata.emission = 0;
                surfacedata.occlusion = 1;
                surfacedata.alpha = 0;
                surfacedata.clearCoatMask = 0;
                surfacedata.clearCoatSmoothness = 0;

                return UniversalFragmentPBR(inputdata, surfacedata);
            }
            ENDHLSL
        }
    }
}
