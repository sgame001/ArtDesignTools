	// Copyright 2016-2018 Kronnect - All Rights Reserved.
	
	#include "UnityCG.cginc"
	#include "BeautifyAdvancedParams.cginc"
	#include "BeautifyOrtho.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _BloomTex;
	uniform half4 	  _BloomTex_TexelSize;
	uniform sampler2D _BloomTex1;
	uniform sampler2D _BloomTex2;
	uniform sampler2D _BloomTex3;
	uniform sampler2D _BloomTex4;
	uniform half4     _MainTex_TexelSize;
	uniform half4     _MainTex_ST;
    uniform half4 	  _Bloom;
	uniform half4 	  _BloomWeights;
	uniform half4 	  _BloomWeights2;
    uniform half4 	  _AFTint;
	uniform half      _BlurScale;

	#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
	uniform sampler2D_float _CameraDepthTexture;
	uniform half      _BloomDepthTreshold;
	#endif

	#if BEAUTIFY_BLOOM_USE_LAYER
	uniform sampler2D_float _BloomSourceDepth;
	uniform half      _BloomLayerZBias;
	#endif


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
	    half2 uv: TEXCOORD0;
	    half2 uv1: TEXCOORD1;
	    half2 uv2: TEXCOORD2;
	    half2 uv3: TEXCOORD3;
	    half2 uv4: TEXCOORD4;
	};

	struct v2fLum {
		float4 pos : SV_POSITION;
		half2 uv: TEXCOORD0;
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
		half2 depthUV: TEXCOORD1;
		#endif
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
		half2 depthUVNonStereo: TEXCOORD2;
		#endif
	};

	struct v2fCrossLum {
		float4 pos : SV_POSITION;
		half2 uv: TEXCOORD0;
		half2 uv1: TEXCOORD1;
		half2 uv2: TEXCOORD2;
		half2 uv3: TEXCOORD3;
		half2 uv4: TEXCOORD4;
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
		half2 depthUV: TEXCOORD5;
		#endif
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
		half2 depthUVNonStereo: TEXCOORD6;
		#endif
	};

	v2f vert(appdata v) {
    	v2f o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.uv.y = 1.0 - o.uv.y;
    	}
    	#endif    	
    	return o;
	}

	v2fLum vertLum(appdata v) {
		v2fLum o;
		o.pos = UnityObjectToClipPos(v.vertex);

		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
			o.uv = v.texcoord;
			o.depthUVNonStereo = v.texcoord;
			o.depthUV = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		#else
			o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
			#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
			o.depthUV = o.uv;
			#endif
		#endif
		#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0) {
				// Depth texture is inverted WRT the main texture
				o.uv.y = 1.0 - o.uv.y;
			}
		#endif
		return o;
	}

	inline half Brightness(half3 c) {
		return max(c.r, max(c.g, c.b));
	}

	half4 fragLum (v2fLum i) : SV_Target {
		half4 c = tex2D(_MainTex, i.uv);
		c = clamp(c, 0.0.xxxx, _BloomWeights2.zzzz);
   		#if UNITY_COLORSPACE_GAMMA
		c.rgb = GammaToLinearSpace(c.rgb);
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
		half depth01 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV)));
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH
		c.rgb *= (1.0 - depth01 * _BloomDepthTreshold);
		#endif
		#if BEAUTIFY_BLOOM_USE_LAYER
			#if UNITY_SINGLE_PASS_STEREO
				half depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUVNonStereo)));
			#else
				half depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUV)));
			#endif
			half isTransparent = (depth02 >= 1) && any(c.rgb>0);
			half nonEclipsed = isTransparent || (depth01 > depth02 - _BloomLayerZBias);
			c.rgb *= nonEclipsed;
		#endif
		c.a = Brightness(c.rgb);
		c.rgb = max(c.rgb - _Bloom.www, 0);
   		return c;
   	}

   	v2fCross vertCross(appdata v) {
    	v2fCross o;
    	o.pos = UnityObjectToClipPos(v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Texture is inverted WRT the main texture
    	    v.texcoord.y = 1.0 - v.texcoord.y;
    	}
    	#endif   
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		half3 offsets = _MainTex_TexelSize.xyx * half3(1,1,-1);
		#if UNITY_SINGLE_PASS_STEREO
		offsets.xz *= 2.0;
		#endif
		o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - offsets.xy, _MainTex_ST);
		o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord - offsets.zy, _MainTex_ST);
		o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offsets.zy, _MainTex_ST);
		o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offsets.xy, _MainTex_ST);
		return o;
	}

	v2fCrossLum vertCrossLum(appdata v) {
		v2fCrossLum o;
		o.pos = UnityObjectToClipPos(v.vertex);
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
			o.depthUVNonStereo = v.texcoord;
			o.depthUV = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
			#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0) {
					// Texture is inverted WRT the main texture
					v.texcoord.y = 1.0 - v.texcoord.y;
				}
			#endif   
			o.uv = v.texcoord;
			half3 offsets = _MainTex_TexelSize.xyx * half3(1, 1, -1);
			o.uv1 = v.texcoord - offsets.xy;
			o.uv2 = v.texcoord - offsets.zy;
			o.uv3 = v.texcoord + offsets.zy;
			o.uv4 = v.texcoord + offsets.xy;
		#else
			#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
			o.depthUV = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
			#endif
			#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0) {
					// Texture is inverted WRT the main texture
					v.texcoord.y = 1.0 - v.texcoord.y;
				}
			#endif   
			o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
			half3 offsets = _MainTex_TexelSize.xyx * half3(1, 1, -1);
			#if UNITY_SINGLE_PASS_STEREO
			offsets.xz *= 2.0;
			#endif
			o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - offsets.xy, _MainTex_ST);
			o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord - offsets.zy, _MainTex_ST);
			o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offsets.zy, _MainTex_ST);
			o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offsets.xy, _MainTex_ST);
		#endif
		return o;

	}

   	half4 fragLumAntiflicker(v2fCrossLum i) : SV_Target {
		half4 c1 = tex2D(_MainTex, i.uv1);
		c1 = clamp(c1, 0.0.xxxx, _BloomWeights2.zzzz);
		half4 c2 = tex2D(_MainTex, i.uv2);
		c2 = clamp(c2, 0.0.xxxx, _BloomWeights2.zzzz);
		half4 c3 = tex2D(_MainTex, i.uv3);
		c3 = clamp(c3, 0.0.xxxx, _BloomWeights2.zzzz);
		half4 c4 = tex2D(_MainTex, i.uv4);
		c4 = clamp(c4, 0.0.xxxx, _BloomWeights2.zzzz);

		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER
		half depth01 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.depthUV)));
		half depthAtten = 1.0 - depth01 * _BloomDepthTreshold;
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH
		c1.rgb *= depthAtten;
		c2.rgb *= depthAtten;
		c3.rgb *= depthAtten;
		c4.rgb *= depthAtten;
		#endif

		#if BEAUTIFY_BLOOM_USE_LAYER
		#if UNITY_SINGLE_PASS_STEREO
		half depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUVNonStereo)));
		#else
		half depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUV)));
		#endif
		half isTransparent = (depth02 >= 1) && any(c1.rgb>0);
		half nonEclipsed = isTransparent || (depth01 > depth02 - _BloomLayerZBias );
		c1.rgb *= nonEclipsed;
		c2.rgb *= nonEclipsed;
		c3.rgb *= nonEclipsed;
		c4.rgb *= nonEclipsed;
		#endif
		
		c1.a = Brightness(c1.rgb);
		c2.a = Brightness(c2.rgb);
		c3.a = Brightness(c3.rgb);
		c4.a = Brightness(c4.rgb);
	    
	    half w1 = 1.0 / (c1.a + 1.0);
	    half w2 = 1.0 / (c2.a + 1.0);
	    half w3 = 1.0 / (c3.a + 1.0);
	    half w4 = 1.0 / (c4.a + 1.0);

	    half dd  = 1.0 / (w1 + w2 + w3 + w4);
	    c1 = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
	    
   		#if UNITY_COLORSPACE_GAMMA
		c1.rgb = GammaToLinearSpace(c1.rgb);
		#endif

		c1.rgb = max(c1.rgb - _Bloom.www, 0);
   		return c1;
	}
	

	half4 fragBloomCompose (v2f i) : SV_Target {
		half4 b0 = tex2D( _BloomTex  , i.uv );
		half4 b1 = tex2D( _BloomTex1 , i.uv );
		half4 b2 = tex2D( _BloomTex2 , i.uv );
		half4 b3 = tex2D( _BloomTex3 , i.uv );
		half4 b4 = tex2D( _BloomTex4 , i.uv );
		half4 b5 = tex2D( _MainTex   , i.uv );
		half4 pixel = b0 * _BloomWeights.x + b1 * _BloomWeights.y + b2 * _BloomWeights.z + b3 * _BloomWeights.w + b4 * _BloomWeights2.x + b5 * _BloomWeights2.y;
		return pixel;
	}


	half4 fragResample(v2fCross i) : SV_Target {
		half4 c1 = tex2D(_MainTex, i.uv1);
		half4 c2 = tex2D(_MainTex, i.uv2);
		half4 c3 = tex2D(_MainTex, i.uv3);
		half4 c4 = tex2D(_MainTex, i.uv4);
			    
	    half w1 = 1.0 / (c1.a + 1.0);
	    half w2 = 1.0 / (c2.a + 1.0);
	    half w3 = 1.0 / (c3.a + 1.0);
	    half w4 = 1.0 / (c4.a + 1.0);
	    
	    half dd  = 1.0 / (w1 + w2 + w3 + w4);
	    return (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
	}


	half4 fragResampleAF(v2fCross i) : SV_Target {
		half4 c1 = tex2D(_MainTex, i.uv1);
		half4 c2 = tex2D(_MainTex, i.uv2);
		half4 c3 = tex2D(_MainTex, i.uv3);
		half4 c4 = tex2D(_MainTex, i.uv4);
			    
	    half w1 = 1.0 / (c1.a + 1.0);
	    half w2 = 1.0 / (c2.a + 1.0);
	    half w3 = 1.0 / (c3.a + 1.0);
	    half w4 = 1.0 / (c4.a + 1.0);
	    
	    half dd  = 1.0 / (w1 + w2 + w3 + w4);
	    c1 = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
	    c1.rgb = lerp(c1.rgb, Brightness(c1.rgb) * _AFTint.rgb, _AFTint.a);
	    c1.rgb *= _Bloom.xxx;
	    return c1;
	}

	half4 fragCopy(v2f i) : SV_Target {
		return tex2D(_MainTex, i.uv);
	}

	half4 fragDebugBloom (v2f i) : SV_Target {
		return tex2D(_BloomTex, i.uv) * _Bloom.xxxx;
	}
	
	half4 fragResampleFastAF(v2f i) : SV_Target {
		half4 c = tex2D(_MainTex, i.uv);
	    c.rgb = lerp(c.rgb, Brightness(c.rgb) * _AFTint.rgb, _AFTint.a);
	    c.rgb *= _Bloom.xxx;
	    return c;
	}	
	
	v2fCross vertBlurH(appdata v) {
    	v2fCross o;
    	o.pos = UnityObjectToClipPos(v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Texture is inverted WRT the main texture
    	    v.texcoord.y = 1.0 - v.texcoord.y;
    	}
    	#endif   
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		half2 inc = half2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0);	
#if UNITY_SINGLE_PASS_STEREO
		inc.x *= 2.0;
#endif
    	o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);	
    	o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);	
		half2 inc2 = half2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0);	
#if UNITY_SINGLE_PASS_STEREO
		inc2.x *= 2.0;
#endif
		o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);
    	o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST);	
		return o;
	}	
	
	v2fCross vertBlurV(appdata v) {
    	v2fCross o;
    	o.pos = UnityObjectToClipPos(v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Texture is inverted WRT the main texture
    	    v.texcoord.y = 1.0 - v.texcoord.y;
    	}
    	#endif   
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	half2 inc = half2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale);	
    	o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);	
    	o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);	
    	half2 inc2 = half2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale);	
    	o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);	
    	o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST);	
    	return o;
	}
	
	half4 fragBlur (v2fCross i): SV_Target {
		half4 pixel = tex2D(_MainTex, i.uv) * 0.2270270270
					+ (tex2D(_MainTex, i.uv1) + tex2D(_MainTex, i.uv2)) * 0.3162162162
					+ (tex2D(_MainTex, i.uv3) + tex2D(_MainTex, i.uv4)) * 0.0702702703;
   		return pixel;
	}	
