	
	#include "UnityCG.cginc"
	
	uniform sampler2D _MainTex;
	uniform sampler2D _EALumSrc;
	uniform sampler2D _EAHist;
	
	uniform half4 _MainTex_TexelSize;
	uniform half4 _MainTex_ST;
	uniform half4 _EyeAdaptation;
	
	
    struct appdata {
    	float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    half2 uv: TEXCOORD0;
	};

	struct v2fCross {
	    float4 pos : SV_POSITION;
	    half2 uv1: TEXCOORD0;
	    half2 uv2: TEXCOORD1;
	    half2 uv3: TEXCOORD2;
	    half2 uv4: TEXCOORD3;
	};
	
	v2f vert(appdata v) {
    	v2f o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	return o;
	}
	
   	v2fCross vertCross(appdata v) {
    	v2fCross o;
    	o.pos = UnityObjectToClipPos(v.vertex);
		half3 offsets = _MainTex_TexelSize.xyy * half3(0.5, 0.5, 0);
		o.uv1 = v.texcoord - offsets.xz;
		o.uv2 = v.texcoord - offsets.zy;
		o.uv3 = v.texcoord + offsets.xz;
		o.uv4 = v.texcoord + offsets.zy;
		return o;
	}	
	
	inline half getLuma(half3 rgb) {
		const half3 lum = float3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}
	
	half4 fragScreenLum (v2f i) : SV_Target {
		half4 c = tex2D(_MainTex, i.uv);
		#if UNITY_COLORSPACE_GAMMA
		c.rgb = GammaToLinearSpace(c.rgb);
		#endif
		c.r = log(1.0 + getLuma(c.rgb));
   		return c.rrrr;
   	}  
   	
   	half4 fragReduceScreenLum (v2fCross i) : SV_Target {
		half4 c1 = tex2D(_MainTex, i.uv1);
		half4 c2 = tex2D(_MainTex, i.uv2);
		half4 c3 = tex2D(_MainTex, i.uv3);
		half4 c4 = tex2D(_MainTex, i.uv4);
	  	c1.g = max( c1.g, max( c2.g, max( c3.g, c4.g )));
	 	c1.r = (c1.r + c2.r + c3.r + c4.r) * 0.25;
   		return c1;
   	}     	

   	half4 fragBlendScreenLum (v2f i) : SV_Target {
		half4 c     = tex2D(_MainTex, 0.5.xx);
   		half4 p     = tex2D(_EAHist , 0.5.xx);
		float speed  = c.r < p.r ? _EyeAdaptation.z: _EyeAdaptation.w;
		c.a = speed * unity_DeltaTime.x;
   		return c;
   	}  
   	
   	half4 fragBlend (v2f i) : SV_Target {
		half4 c = tex2D(_MainTex, 0.5.xx);
		c.a = 1.0;
   		return c;
   	}  

