Shader "Anzu/Video Playback Shader" {
	Properties {
		[MaterialToggle] _ShouldSwitchRB ("Should switch R/B", Float) = 0
		[MaterialToggle] _ShouldFlipX ("Should Flip X", Float) = 0
		[MaterialToggle] _ShouldFlipY ("Should Flip Y", Float) = 0
		_Transparency ("Transparency", Range(0, 1)) = 1
		_BaseColor ("Base Color (RGB)", Vector) = (1,1,1,1)
		_Brightness ("Brightness", Float) = 0
		_Contrast ("Contrast", Float) = 1
		_VisibleNormWidth ("Visible Width", Float) = 1
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}