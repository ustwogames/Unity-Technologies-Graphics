//
// This file was automatically generated. Please don't edit by hand.
//

#ifndef PROBEVOLUMELIGHTING_CS_HLSL
#define PROBEVOLUMELIGHTING_CS_HLSL
//
// UnityEngine.Rendering.HighDefinition.LeakMitigationMode:  static fields
//
#define LEAKMITIGATIONMODE_NORMAL_BIAS (0)
#define LEAKMITIGATIONMODE_GEOMETRIC_FILTER (1)
#define LEAKMITIGATIONMODE_PROBE_VALIDITY_FILTER (2)
#define LEAKMITIGATIONMODE_OCTAHEDRAL_DEPTH_OCCLUSION_FILTER (3)

// Generated from UnityEngine.Rendering.HighDefinition.ProbeVolumeEngineData
// PackingRules = Exact
struct ProbeVolumeEngineData
{
    float weight;
    float3 debugColor;
    int payloadIndex;
    float3 rcpPosFaceFade;
    float3 rcpNegFaceFade;
    float rcpDistFadeLen;
    float endTimesRcpDistFadeLen;
    float3 scale;
    float3 bias;
    float4 octahedralDepthScaleBias;
    float3 resolution;
    float3 resolutionInverse;
    int volumeBlendMode;
};

// Generated from UnityEngine.Rendering.HighDefinition.SphericalHarmonicsL1
// PackingRules = Exact
struct SphericalHarmonicsL1
{
    float4 shAr;
    float4 shAg;
    float4 shAb;
};

//
// Accessors for UnityEngine.Rendering.HighDefinition.ProbeVolumeEngineData
//
float GetWeight(ProbeVolumeEngineData value)
{
    return value.weight;
}
float3 GetDebugColor(ProbeVolumeEngineData value)
{
    return value.debugColor;
}
int GetPayloadIndex(ProbeVolumeEngineData value)
{
    return value.payloadIndex;
}
float3 GetRcpPosFaceFade(ProbeVolumeEngineData value)
{
    return value.rcpPosFaceFade;
}
float3 GetRcpNegFaceFade(ProbeVolumeEngineData value)
{
    return value.rcpNegFaceFade;
}
float GetRcpDistFadeLen(ProbeVolumeEngineData value)
{
    return value.rcpDistFadeLen;
}
float GetEndTimesRcpDistFadeLen(ProbeVolumeEngineData value)
{
    return value.endTimesRcpDistFadeLen;
}
float3 GetScale(ProbeVolumeEngineData value)
{
    return value.scale;
}
float3 GetBias(ProbeVolumeEngineData value)
{
    return value.bias;
}
float4 GetOctahedralDepthScaleBias(ProbeVolumeEngineData value)
{
    return value.octahedralDepthScaleBias;
}
float3 GetResolution(ProbeVolumeEngineData value)
{
    return value.resolution;
}
float3 GetResolutionInverse(ProbeVolumeEngineData value)
{
    return value.resolutionInverse;
}
int GetVolumeBlendMode(ProbeVolumeEngineData value)
{
    return value.volumeBlendMode;
}
//
// Accessors for UnityEngine.Rendering.HighDefinition.SphericalHarmonicsL1
//
float4 GetShAr(SphericalHarmonicsL1 value)
{
    return value.shAr;
}
float4 GetShAg(SphericalHarmonicsL1 value)
{
    return value.shAg;
}
float4 GetShAb(SphericalHarmonicsL1 value)
{
    return value.shAb;
}

#endif
