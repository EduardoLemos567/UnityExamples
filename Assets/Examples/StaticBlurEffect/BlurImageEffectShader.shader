// Based/Copied from: https://github.com/amilajack/gaussian-blur

// The MIT License (MIT) Copyright (c) 2018-present Amila
// Permission is hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies 
// or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

Shader "ImageEffect/BlurImageEffectShader"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Direction ("Direction", Vector) = (0,0,0,0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            //#pragma target 2.0

            //#include "UnityCG.cginc"
            #include "HLSLSupport.cginc"
            #include "UnityShaderVariables.cginc"
            #include "UnityCG.cginc"


            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float2 _Direction;

            struct FromUnity
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct ToFragment
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            ToFragment VertexProgram (FromUnity from_unity)
            {
                ToFragment to_fragment;
                to_fragment.vertex = UnityObjectToClipPos(from_unity.vertex);
                to_fragment.uv = from_unity.uv;
                return to_fragment;
            }

            void BlurEffect(in sampler2D tex, in float2 texel_size, in float2 uv, in float2 direction, out fixed3 color)
            {
                const float voff1 = 1.3846153846;
                const float voff2 = 3.2307692308;
                const float vuv0 = 0.2270270270;
                const float vuv1 = 0.3162162162;
                const float vuv2 = 0.0702702703;
                float2 off1 = voff1 * direction * texel_size;
                float2 off2 = voff2 * direction * texel_size;
                color = tex2D(tex, uv) * vuv0;
                color += tex2D(tex, uv + off1) * vuv1;
                color += tex2D(tex, uv - off1) * vuv1;
                color += tex2D(tex, uv + off2) * vuv2;
                color += tex2D(tex, uv - off2) * vuv2;
            }

            fixed4 FragmentProgram (ToFragment from_vertex) : SV_Target
            {
                fixed3 color;
                BlurEffect(_MainTex, _MainTex_TexelSize, from_vertex.uv, _Direction, color);
                return fixed4(color, 1);
            }
            ENDHLSL
        }
    }
}
