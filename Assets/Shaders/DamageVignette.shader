Shader "Custom/UI/DamageVignette"
{
    Properties
    {
        _Color ("Vignette Color", Color) = (1, 0, 0, 1)
        _Radius ("Inner Radius", Range(0.0, 1.5)) = 0.6
        _Softness ("Softness", Range(0.01, 1.0)) = 0.4
        // Required by Unity UI system
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float4 worldPos : TEXCOORD1;
            };

            float4 _Color;
            float  _Radius;
            float  _Softness;
            float4 _ClipRect;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex;
                o.uv       = v.uv;
                o.color    = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Distance from center, corrected for aspect ratio via the uv space
                float2 uv = i.uv - 0.5;
                float dist = length(uv * 2.0);

                // Vignette: 0 at center, 1 at edges
                float vignette = smoothstep(_Radius - _Softness, _Radius + _Softness, dist);

                // Overall alpha comes from the Image's color alpha (set by C# script)
                float alpha = vignette * i.color.a;

                // Clip to UI rect (required for Unity UI masking support)
                alpha *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);

                fixed4 col = _Color;
                col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
}
