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

float4x4 Ret_float4x4() { return float4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16); }

// Parameter types.
float In_float4x4(float4x4 param) { return param[1][3]; }
