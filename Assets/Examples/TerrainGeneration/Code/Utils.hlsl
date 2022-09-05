#ifndef TERRAGEN_UTILS_HLSL
#define TERRAGEN_UTILS_HLSL

inline float Remap(float value, float in_min, float in_max, float out_min, float out_max)
{
    return out_min + (value - in_min) * (out_max - out_min) / (in_max - in_min);
}

inline bool CheckBoundary(in uint3 id) { return (float)id.x < textureSize.x && (float)id.y < textureSize.y; }

inline float2 GetUV(in uint3 id) { return id.xy / textureSize; }

inline float Remap(float value, float2 in_values, float2 out_values)
{
    return out_values.x + (value - in_values.x) * (out_values.y - out_values.x) / (in_values.y - in_values.x);
}

#endif //TERRAGEN_UTILS_HLSL