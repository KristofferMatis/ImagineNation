﻿// TO USE
//
// 1. Make sure you have set the ambient lighting. Edit -> Render Settings -> Ambient Lighting
// 2. Create a material
// 3. Place the material on the object
// 4. Drag a texture to the "_Texture" box
// 5. You are done.
//
// Created by Jason Hein


Shader "Production/Diffuse"
{
	//Properties that can be set by designers
	Properties
    {
    	_MainTex ("Texture", 2D) = "white" {} 
    	_PointLightIllumination("Point Light Illumination", Float) = 10.0
    	_PointLightMaximumIllumination("Point Light Max Illumination", Float) = 0.35
    }
    
    //Shader
	SubShader
	{
		//Pass for directional and ambient lighting
		Pass 
		{
			Tags { "LightMode" = "ForwardBase" } 
			
			//This is a CG shader
			CGPROGRAM
			
			//Allows us to get ambient lighting
			#include "UnityCG.cginc"
 			
 			//Define the shaders
         	#pragma vertex vertShader
         	#pragma fragment fragShader
         	
         	//World Light Colour (from "UnityCG.cginc")
         	float4 _LightColor0;
         	
         	//Public Uniforms
         	sampler2D _MainTex;
         	float4 _MainTex_ST;
         	
         	//What the vertex shader will recieve
         	struct vertInput
         	{
            	float4 pos : POSITION0;
            	float3 normal : NORMAL;
            	half2 uv : TEXCOORD0;
       		};
       		
       		//What the fragment shader will recieve
         	struct vertOutput
         	{
            	float4 pos : POSITION0;
            	float4 posWorld : POSITION1;
            	float3 normalDir : TEXCOORD0;
            	half2 uv : TEXCOORD1;
        	};
         	
         	//Vertex Shader
         	vertOutput vertShader(vertInput input)
         	{
         		//A container for the vertexOutput
         		vertOutput output;
         		
         		//Calculate the vertex's position according to the camera
         		output.pos = mul(UNITY_MATRIX_MVP, input.pos);
         		
         		//Calculate the real world position of the vertex, for later calculations
         		output.posWorld = mul(_Object2World, input.pos);
         		
         		//Calculate the direction of our surface normal
         		output.normalDir = normalize(mul(float4(input.normal, 0.0), _World2Object).xyz);
         		
         		//Give output the texture colour
         		output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
         		
         		//Return our output
         		return output;
         	}
         	
         	//Fragment Shader
         	float4 fragShader(vertOutput output) : COLOR
         	{
         		//All our values are interpolated, so now we can do per-pixel calculations
         		
         		//Re-normalize direction values so they are correct
         		float3 normalDirection = normalize(output.normalDir);
            	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - output.posWorld.xyz);
            	
            	//Direction of our light, for dot product calculations
            	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
 				
 				//Base colour of this fragment
            	float4 textureColor = tex2D(_MainTex, output.uv);
            	
            	//Calculate ambient light
            	float3 ambientLight = textureColor.xyz * UNITY_LIGHTMODEL_AMBIENT.xyz;
            	
            	//Calculate the base colour of the fragment with lighting
            	float3 diffuseLighting = textureColor.xyz * _LightColor0.xyz * max(0.0, dot(normalDirection, lightDirection));

         		//Return the final colour of the fragment
         		return float4(ambientLight + diffuseLighting, 1.0);
         	}
         	
 			//End the cg shader
 			ENDCG
		}
		
		//Pass for additional lighting
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }
			
			//Add the the colour we had already
			Blend One One
			
			//This is a CG shader
			CGPROGRAM
 			
 			//Define the shaders
         	#pragma vertex vertShader
         	#pragma fragment fragShader
         	
         	//World Light Colour (from "UnityCG.cginc")
         	float4 _LightColor0;
         	
         	//Public Uniforms
         	sampler2D _MainTex;
         	float4 _MainTex_ST;
         	float _PointLightIllumination;
         	float _PointLightMaximumIllumination;
         	
         	//What the vertex shader will recieve
         	struct vertInput
         	{
            	float4 pos : POSITION0;
            	float3 normal : NORMAL;
            	half2 uv : TEXCOORD0;
       		};
       		
       		//What the fragment shader willl recieve
         	struct vertOutput
         	{
            	float4 pos : POSITION0;
            	float4 posWorld : POSITION1;
            	float3 normalDir : TEXCOORD0;
            	half2 uv : TEXCOORD1;
            	float3 vertexLighting : TEXCOORD2;
        	};
        	
        	//Vertex Shader
         	vertOutput vertShader(vertInput input)
         	{
         		//A container for the vertexOutput
         		vertOutput output;
         		
         		//Calculate the vertex's position according to the camera
         		output.pos = mul(UNITY_MATRIX_MVP, input.pos);
         		
         		//Calculate the real world position of the vertex, for later calculations
         		output.posWorld = mul(_Object2World, input.pos);
         		
         		//Calculate the direction of our surface normal
         		output.normalDir = normalize(mul(float4(input.normal, 0.0), _World2Object).xyz);
         		
         		//Give output the texture colour
         		output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
         		
         		//Additional Lighting (vertex lights)
         		output.vertexLighting = float3 (0.0, 0.0, 0.0);
         		#ifdef VERTEXLIGHT_ON
            	for (int index = 0; index < 3; index++)
            	{    
               		float3 vertexToLightSource = unity_LightPosition[index].xyz - output.posWorld.xyz; 
              	 	float distShading = 1.0 / pow(vertexToLightSource, 2) * _PointLightIllumination;
              	 	if (distShading > _PointLightMaximumIllumination)
            		{
            			distShading = _PointLightMaximumIllumination;
            		}
              	 	
               		float3 vertexLightIllumination = distShading * unity_LightColor[index].rgb *
               		tex2D(_MainTex, output.uv).rgb * max(0.0, dot(output.normalDir, normalize(vertexToLightSource)));         
 
               		output.vertexLighting += vertexLightIllumination;
            	}
           	 	#endif
         		
         		//Return our output
         		return output;
         	}
         	
         	//Fragment Shader
         	float4 fragShader(vertOutput output) : COLOR
         	{
         		//All our values are interpolated, so now we can do per-pixel calculations
         		
         		//Re-normalize direction values so they are correct
         		float3 normalDirection = normalize(output.normalDir);
            	float3 viewDirection = normalize(_WorldSpaceCameraPos - output.posWorld.xyz);
            	
            	//Direction of our light, for dot product calculations
            	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz - output.posWorld.xyz);
 				
 				//Base colour of this fragment
            	float4 textureColor = tex2D(_MainTex, output.uv);
            	
            	//Calculate shading based off our distance from the lights
            	float distShading = 1.0 / pow(length(_WorldSpaceLightPos0.xyz - output.posWorld.xyz), 2.0) * _PointLightIllumination;
            	if (distShading > _PointLightMaximumIllumination)
            	{
            		distShading = _PointLightMaximumIllumination;
            	}
            	
            	//Calculate the base colour of the fragment with lighting
            	float3 fragmentColour = textureColor.xyz * _LightColor0.xyz * distShading * max(0.0, dot(normalDirection, lightDirection));

         		//Return the final colour of the fragment
         		return float4(fragmentColour + output.vertexLighting, 1.0);
         	}
         	
         	//End the cg shader
 			ENDCG
		}
	}
}