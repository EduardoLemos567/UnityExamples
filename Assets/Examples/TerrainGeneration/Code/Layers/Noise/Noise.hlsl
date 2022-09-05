#ifndef TERRAGEN_NOISE_HLSL
#define TERRAGEN_NOISE_HLSL

// All functions in this file(except the kernel void Noise (uint3 id : SV_DispatchThreadID))
// are licensed under MIT from following authors:

// Copyright (c) 2011~2016 Stefan Gustavson. All rights reserved.
// Distributed under the MIT license. See LICENSE file.
// https://github.com/stegu/webgl-noise

// Copyright (C) 2011 Ashima Arts. All rights reserved.
// Distributed under the MIT License. See LICENSE file.
// https://github.com/ashima/webgl-noise

inline float NoiseRandom(float2 co){
    return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
}
// float2
inline float2 NoiseMod289(float2 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}
// float3
inline float3 NoiseMod289(float3 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}
// float4
inline float4 NoiseMod289(float4 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}
// Modulo 7 without a division
inline float3 NoiseMod7(float3 x) {
  return x - floor(x * (1.0 / 7.0)) * 7.0;
}
// Permutation polynomial: (34x^2 + 6x) mod 289
inline float3 NoisePermute(float3 x) {
  return NoiseMod289((34.0 * x + 10.0) * x);
}
// float4
inline float4 NoisePermute(float4 x)
{
  return NoiseMod289(((x*34.0)+10.0)*x);
}
// Cellular noise, returning F1 and F2 in a float2.
// Standard 3x3 search window for good F1 and F2 values
float2 NoiseCellular(float2 P) {
    const float K = 1.0/7.0;
    const float Ko = 3.0/7.0;
    const float jitter = 1.0;
	float2 Pi = NoiseMod289(floor(P));
 	float2 Pf = frac(P);
	float3 oi = float3(-1.0, 0.0, 1.0);
	float3 of = float3(-0.5, 0.5, 1.5);
	float3 px = NoisePermute(Pi.x + oi);
	float3 p = NoisePermute(px.x + Pi.y + oi); // p11, p12, p13
	float3 ox = frac(p*K) - Ko;
	float3 oy = NoiseMod7(floor(p*K))*K - Ko;
	float3 dx = Pf.x + 0.5 + jitter*ox;
	float3 dy = Pf.y - of + jitter*oy;
	float3 d1 = dx * dx + dy * dy; // d11, d12 and d13, squared
	p = NoisePermute(px.y + Pi.y + oi); // p21, p22, p23
	ox = frac(p*K) - Ko;
	oy = NoiseMod7(floor(p*K))*K - Ko;
	dx = Pf.x - 0.5 + jitter*ox;
	dy = Pf.y - of + jitter*oy;
	float3 d2 = dx * dx + dy * dy; // d21, d22 and d23, squared
	p = NoisePermute(px.z + Pi.y + oi); // p31, p32, p33
	ox = frac(p*K) - Ko;
	oy = NoiseMod7(floor(p*K))*K - Ko;
	dx = Pf.x - 1.5 + jitter*ox;
	dy = Pf.y - of + jitter*oy;
	float3 d3 = dx * dx + dy * dy; // d31, d32 and d33, squared
	// Sort out the two smallest distances (F1, F2)
	float3 d1a = min(d1, d2);
	d2 = max(d1, d2); // Swap to keep candidates for F2
	d2 = min(d2, d3); // neither F1 nor F2 are now in d3
	d1 = min(d1a, d2); // F1 is now in d1
	d2 = max(d1a, d2); // Swap to keep candidates for F2
	d1.xy = (d1.x < d1.y) ? d1.xy : d1.yx; // Swap if smaller
	d1.xz = (d1.x < d1.z) ? d1.xz : d1.zx; // F1 is in d1.x
	d1.yz = min(d1.yz, d2.yz); // F2 is now not in d2.yz
	d1.y = min(d1.y, d1.z); // nor in  d1.z
	d1.y = min(d1.y, d2.x); // F2 is in d1.y, we're done.
	return sqrt(d1.xy);
}
inline float4 NoiseTaylorInvSqrt(float4 r)
{
  return 1.79284291400159 - 0.85373472095314 * r;
}
inline float2 NoiseFade(float2 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}
float NoiseSimplex(float2 v)
  {
  const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                     -0.577350269189626,  // -1.0 + 2.0 * C.x
                      0.024390243902439); // 1.0 / 41.0
// First corner
  float2 i  = floor(v + dot(v, C.yy) );
  float2 x0 = v -   i + dot(i, C.xx);

// Other corners
  float2 i1;
  //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
  //i1.y = 1.0 - i1.x;
  i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
  // x0 = x0 - 0.0 + 0.0 * C.xx ;
  // x1 = x0 - i1 + 1.0 * C.xx ;
  // x2 = x0 - 1.0 + 2.0 * C.xx ;
  float4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;

// Permutations
  i = NoiseMod289(i); // Avoid truncation effects in permutation
  float3 p = NoisePermute( NoisePermute( i.y + float3(0.0, i1.y, 1.0 ))
		+ i.x + float3(0.0, i1.x, 1.0 ));

  float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;

// Gradients: 41 points uniformly over a line, mapped onto a diamond.
// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

  float3 x = 2.0 * frac(p * C.www) - 1.0;
  float3 h = abs(x) - 0.5;
  float3 ox = floor(x + 0.5);
  float3 a0 = x - ox;

// Normalise gradients implicitly by scaling m
// Approximation of: m *= inversesqrt( a0*a0 + h*h );

  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

// Compute final noise value at P
  float3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}
// Classic Perlin noise
float NoisePerlin(float2 P)
{
  float4 Pi = floor(P.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
  float4 Pf = frac(P.xyxy) - float4(0.0, 0.0, 1.0, 1.0);
  Pi = NoiseMod289(Pi); // To avoid truncation effects in permutation
  float4 ix = Pi.xzxz;
  float4 iy = Pi.yyww;
  float4 fx = Pf.xzxz;
  float4 fy = Pf.yyww;

  float4 i = NoisePermute(NoisePermute(ix) + iy);

  float4 gx = frac(i * (1.0 / 41.0)) * 2.0 - 1.0 ;
  float4 gy = abs(gx) - 0.5 ;
  float4 tx = floor(gx + 0.5);
  gx = gx - tx;

  float2 g00 = float2(gx.x,gy.x);
  float2 g10 = float2(gx.y,gy.y);
  float2 g01 = float2(gx.z,gy.z);
  float2 g11 = float2(gx.w,gy.w);

  float4 norm = NoiseTaylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
  g00 *= norm.x;  
  g01 *= norm.y;  
  g10 *= norm.z;  
  g11 *= norm.w;  

  float n00 = dot(g00, float2(fx.x, fy.x));
  float n10 = dot(g10, float2(fx.y, fy.y));
  float n01 = dot(g01, float2(fx.z, fy.z));
  float n11 = dot(g11, float2(fx.w, fy.w));

  float2 fade_xy = NoiseFade(Pf.xy);
  float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
  float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
  return 2.3 * n_xy;
}
// Classic Perlin noise, periodic variant
float NoisePeriodicPerlin(float2 P, float2 rep)
{
  float4 Pi = floor(P.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
  float4 Pf = frac(P.xyxy) - float4(0.0, 0.0, 1.0, 1.0);
  Pi = fmod(Pi, rep.xyxy); // To create noise with explicit period, TODO: check if GLSL.mod == HLSL.fmod in this case
  Pi = NoiseMod289(Pi);        // To avoid truncation effects in permutation
  float4 ix = Pi.xzxz;
  float4 iy = Pi.yyww;
  float4 fx = Pf.xzxz;
  float4 fy = Pf.yyww;

  float4 i = NoisePermute(NoisePermute(ix) + iy);

  float4 gx = frac(i * (1.0 / 41.0)) * 2.0 - 1.0 ;
  float4 gy = abs(gx) - 0.5 ;
  float4 tx = floor(gx + 0.5);
  gx = gx - tx;

  float2 g00 = float2(gx.x,gy.x);
  float2 g10 = float2(gx.y,gy.y);
  float2 g01 = float2(gx.z,gy.z);
  float2 g11 = float2(gx.w,gy.w);

  float4 norm = NoiseTaylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
  g00 *= norm.x;  
  g01 *= norm.y;  
  g10 *= norm.z;  
  g11 *= norm.w;  

  float n00 = dot(g00, float2(fx.x, fy.x));
  float n10 = dot(g10, float2(fx.y, fy.y));
  float n01 = dot(g01, float2(fx.z, fy.z));
  float n11 = dot(g11, float2(fx.w, fy.w));

  float2 fade_xy = NoiseFade(Pf.xy);
  float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
  float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
  return 2.3 * n_xy;
}

// Variables
int noiseType;
float2 noiseRange;
float4 noiseOffsetScale;

[numthreads(8,8,1)]
void NoiseKernel (uint3 id : SV_DispatchThreadID)
{
    if(CheckBoundary(id))
    {
        float height = 0;
        float2 uv = (GetUV(id) + noiseOffsetScale.xy) * noiseOffsetScale.zw;
        switch(noiseType)
        {
            case 0: 
                height = NoiseRandom(uv);
                break;
            case 1: 
                height = NoiseSimplex(uv);
                break;
            case 2: 
                height = NoisePerlin(uv);
                break;
            case 3: 
                height = NoiseCellular(uv).x;
                break;
        }
        resultTexture[id.xy] = resultTexture[id.xy] + Remap(height, float2(-1, 1), noiseRange);
    }
}

#endif //TERRAGEN_NOISE_HLSL