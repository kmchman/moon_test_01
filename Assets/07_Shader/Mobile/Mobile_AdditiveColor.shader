
Shader "Mobile/Particles/AdditiveColor" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	 _TintColor ("Tint Color", Color) = (0.500000,0.500000,0.500000,0.500000)
}

Category {
	Tags { "Queue"="Transparent -1 " "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
		
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				
				combine constant * primary
				
			}
			SetTexture [_MainTex] {
                 combine texture * previous DOUBLE
             }
		}
	}
}
}