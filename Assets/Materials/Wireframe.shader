Shader "Unlit/TexturedWireframe" {
    Properties {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0, 0, 0, 1)
        _WireColor ("Wire Color", Color) = (0, 1, 0, 1)
        _Thickness ("Wire Thickness", Range(0, 5)) = 1
        _WireOpacity ("Wire Opacity", Range(0, 1)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _WireColor;
            fixed4 _BaseColor;
            float _Thickness;
            float _WireOpacity;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Standard UV tiling and offset support
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Sample the base texture
                fixed4 texCol = tex2D(_MainTex, i.uv);
                texCol = texCol * _BaseColor;
                
                // 2. Calculate the wireframe mask using screen-space derivatives
                // fwidth ensures the lines stay a consistent width regardless of distance
                float2 derivative = fwidth(i.uv);
                float2 lineGrid = smoothstep(derivative * _Thickness, 0, abs(frac(i.uv - 0.5) - 0.5));
                
                // Combine U and V lines into a single mask
                float mask = max(lineGrid.x, lineGrid.y);
                
                // 3. Blend the texture color with the wire color
                // We multiply mask by _WireOpacity so you can fade the wires out
                fixed4 finalCol = lerp(texCol, _WireColor, mask * _WireOpacity);
                
                return finalCol;
            }
            ENDCG
        }
    }
}