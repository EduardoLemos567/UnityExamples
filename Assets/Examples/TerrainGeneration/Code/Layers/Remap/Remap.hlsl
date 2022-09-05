#ifndef TERRAGEN_REMAP_HLSL
#define TERRAGEN_REMAP_HLSL

bool compressOutputForTerrain;
float4 remapValues;

[numthreads(8,8,1)]
void RemapKernel (uint3 id : SV_DispatchThreadID)
{
    const float MAX_ALLOWED_HEIGHT = 32767.0/65536.0;   // Per unity terrain system
    if(CheckBoundary(id))
    {
        float height = resultTexture[id.xy];
        if(compressOutputForTerrain)
        {
            height = Remap(height, remapValues.xy, float2(0, MAX_ALLOWED_HEIGHT));
        }
        else
        {
            height = Remap(height, remapValues.xy, remapValues.zw);
        }
        resultTexture[id.xy] = height;
    }
}

#endif //TERRAGEN_REMAP_HLSL