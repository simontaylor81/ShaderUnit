// Tests for various different parameter and return types.

// Return types
float Ret_float() { return 1.0f; }
int Ret_int() { return -1; }
uint Ret_uint() { return 1; }

float2 Ret_float2() { return float2(1.0f, 2.0f); }
float3 Ret_float3() { return float3(1.0f, 2.0f, 3.0f); }
float4 Ret_float4() { return float4(1.0f, 2.0f, 3.0f, 4.0f); }

int2 Ret_int2() { return int2(1, 2); }
int3 Ret_int3() { return int3(1, 2, 3); }
int4 Ret_int4() { return int4(1, 2, 3, 4); }

int2 Ret_uint2() { return uint2(1, 2); }
int3 Ret_uint3() { return uint3(1, 2, 3); }
int4 Ret_uint4() { return uint4(1, 2, 3, 4); }

float3x3 Ret_float3x3() { return float3x3(1, 2, 3, 4, 5, 6, 7, 8, 9); }
float3x3 Ret_int3x3() { return int3x3(1, 2, 3, 4, 5, 6, 7, 8, 9); }
float3x3 Ret_uint3x3() { return uint3x3(1, 2, 3, 4, 5, 6, 7, 8, 9); }

float4x4 Ret_float4x4() { return float4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16); }
float4x4 Ret_int4x4() { return int4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16); }
float4x4 Ret_uint4x4() { return uint4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16); }

// Parameter types.
float In_float(float param) { return param; }
float In_int(int param) { return (float)param; }
float In_uint(uint param) { return (float)param; }

float In_float2(float2 param) { return param.y; }
float In_float3(float3 param) { return param.z; }
float In_float4(float4 param) { return param.w; }

float In_int2(int2 param) { return (float)param.y; }
float In_int3(int3 param) { return (float)param.z; }
float In_int4(int4 param) { return (float)param.w; }

float In_uint2(uint2 param) { return (float)param.y; }
float In_uint3(uint3 param) { return (float)param.z; }
float In_uint4(uint4 param) { return (float)param.w; }

float In_float3x3(float3x3 param) { return param._21; }
float In_int3x3(int3x3 param) { return (float)param._21; }
float In_uint3x3(uint3x3 param) { return (float)param._21; }

float In_float4x4(float4x4 param) { return param._24; }
float In_int4x4(int4x4 param) { return (float)param._24; }
float In_uint4x4(uint4x4 param) { return (float)param._24; }
