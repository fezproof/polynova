Shader "Custom/ScrollingWater" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_ScrollXSpeed("X Scroll Speed", Range(0,1)) = 0.1
		_ScrollYSpeed("Y Scroll Speed", Range(0,1)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		fixed _ScrollXSpeed;
		fixed _ScrollYSpeed;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed2 scrolledUV = IN.uv_MainTex;
			fixed2 scrolledUV2 = IN.uv_MainTex;

			fixed xScrollValue = frac(_ScrollXSpeed * _SinTime.y);
			fixed yScrollValue = frac(_ScrollYSpeed * _SinTime.y);
			scrolledUV += fixed2(xScrollValue, yScrollValue);
			scrolledUV2 +=  3.0 * fixed2(xScrollValue, -yScrollValue);

			// Albedo comes from a texture tinted by color
			fixed4 c = 0.6 * tex2D (_MainTex, scrolledUV) * _Color + 0.4 * tex2D(_MainTex, scrolledUV2) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
