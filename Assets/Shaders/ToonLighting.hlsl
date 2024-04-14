#ifdef SHADERGRAPH_PREVIEW
#else
half3 GetAdditionalLightColor(float3 Normal, float3 WPos, out half Intensity)
{
	half3 addLightCol = half3(0,0,0);
	half currentIntensity = 0;
    uint numAdditionalLights = GetAdditionalLightsCount();
    for (uint lightI = 0; lightI < numAdditionalLights; lightI++) {
        Light addLight = GetAdditionalLight(lightI, WPos, 1);
        half lightDot = dot(Normal, addLight.direction);
		currentIntensity = smoothstep(0, 0.03, lightDot* addLight.distanceAttenuation) * addLight.shadowAttenuation * saturate(GetAdditionalLightsCount());
		Intensity+=currentIntensity;
		addLightCol+= lerp(0,addLight.color,currentIntensity);
	} 
	
	return addLightCol;
}
#endif

#define COOKIE_TILING 0.05
void ToonShading_float(in half3 texColor, in float3 Normal, in float3 ClipSpacePos, in float3 WorldPos, in float3 ViewDir,in float Glossiness,in float lightMult, out float3 ToonRampOutput)
{

	// set the shader graph node previews
#ifdef SHADERGRAPH_PREVIEW
	ToonRampOutput = float3(0.5, 0.5, 0);
#else

	// grab the shadow coordinates
#if SHADOWS_SCREEN
	half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
#else
	half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif 

	// grab the main light
#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
	Light light = GetMainLight(shadowCoord);
#else
	Light light = GetMainLight();
#endif

//Additional Lights
	half addLightIntensity = 0;
	float3 addLightCol = float3(0,0,0);
#ifdef _ADDITIONAL_LIGHTS
	addLightCol = GetAdditionalLightColor(Normal, WorldPos, addLightIntensity);
#endif

	//Rework glossiness to better handle small values, add a fixed min glossiness
	Glossiness = saturate(Glossiness * Glossiness *Glossiness);

	//Light
	half NdotL = dot(Normal, light.direction);
	half lightIntensity = smoothstep(0, 0.03, NdotL * light.shadowAttenuation);
	half3 shadowColor = light.color * 0.8;
	shadowColor.b*=1.3;
	half3 lightTint = lerp(shadowColor, light.color, lightIntensity);

	//Specular
	//Half vector is the vetwor in between view dir and light dir
	float3 halfVector = normalize(ViewDir+light.direction);
	float NdotH = dot(Normal, halfVector);
	//Change size based on glossiness
	float specularIntensity = pow(NdotH * lightIntensity, Glossiness *400);
	//Change intensity based on glossiness
	specularIntensity = max(smoothstep(0.005, 0.03, specularIntensity), addLightIntensity) * Glossiness;
	
	//Rim
	//Fix rim size (only intensity changes based on glossiness)
	half rimCutoff = 0.4;
	//Affected by light dir (NdotL) and by shadows (in light intensity)
	float rimDot = (1 - dot(ViewDir, Normal)) * NdotL * lightIntensity;
	//Change intensity based on glossiness
	float rimIntensity = smoothstep(rimCutoff - 0.03, rimCutoff + 0.03, rimDot)*Glossiness;
	
	// add in lights and extra tinting
	ToonRampOutput = lightTint + (specularIntensity +rimIntensity) *light.color + addLightCol;
	ToonRampOutput *= lightMult;
#endif
}