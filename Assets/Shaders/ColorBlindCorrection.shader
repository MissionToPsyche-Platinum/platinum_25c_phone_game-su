Shader "Custom/ColorBlindCorrection"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Blend Off Cull Off

        Pass
        {
            Name "ColorBlindCorrection"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Set globally via Shader.SetGlobalFloat("_ColorBlindMode", ...)
            // 0 = None, 1 = Protanopia, 2 = Deuteranopia, 3 = Tritanopia
            // float used instead of int — SetGlobalInt is unreliable in URP fragment shaders
            float _ColorBlindMode;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 tex = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);

                int mode = (int)_ColorBlindMode;

                // Passthrough when no mode is active
                if (mode == 0)
                    return tex;

                // --- RGB to LMS (cone response) ---
                // Source: daltonize.org (used widely in game dev)
                float L = (17.8824   * tex.r) + (43.5161  * tex.g) + (4.11935 * tex.b);
                float M = (3.45565   * tex.r) + (27.1554  * tex.g) + (3.86714 * tex.b);
                float S = (0.0299566 * tex.r) + (0.184309 * tex.g) + (1.46709 * tex.b);

                float l, m, s;

                // --- Simulate colorblind perception in LMS space ---
                UNITY_BRANCH
                if (mode == 1) // Protanopia (L-cone missing)
                {
                    l =  0.0      * L + 2.02344  * M + -2.52581 * S;
                    m =  0.0      * L + 1.0      * M +  0.0     * S;
                    s =  0.0      * L + 0.0      * M +  1.0     * S;
                }
                else if (mode == 2) // Deuteranopia (M-cone missing)
                {
                    l =  1.0      * L + 0.0 * M + 0.0     * S;
                    m =  0.494207 * L + 0.0 * M + 1.24827 * S;
                    s =  0.0      * L + 0.0 * M + 1.0     * S;
                }
                else // Tritanopia (S-cone missing)
                {
                    l =  1.0      * L +  0.0      * M + 0.0 * S;
                    m =  0.0      * L +  1.0      * M + 0.0 * S;
                    s = -0.395913 * L +  0.801109 * M + 0.0 * S;
                }

                // --- Simulated LMS back to RGB (what colorblind person sees) ---
                half4 sim;
                sim.r = ( 0.0809444479   * l) + (-0.130504409   * m) + ( 0.116721066 * s);
                sim.g = (-0.0102485335   * l) + ( 0.0540193266  * m) + (-0.113614708 * s);
                sim.b = (-0.000365296938 * l) + (-0.00412161469 * m) + ( 0.693511405 * s);
                sim.a = 1.0;

                // --- Daltonization: shift lost info into channels they can perceive ---
                half4 diff = tex - sim;
                half4 result = tex;
                result.g += (diff.r * 0.7) + diff.g;
                result.b += (diff.r * 0.7) + diff.b;

                return half4(saturate(result.rgb), tex.a);
            }
            ENDHLSL
        }
    }
}
