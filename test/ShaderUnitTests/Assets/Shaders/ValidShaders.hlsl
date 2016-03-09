
[numthreads(64, 1, 1)]
void CS()
{
}

float4 PS(float2 uv : TEXCOORD0) : SV_Target
{
	return float4(1, 2, 3, 4);
}

float4 VS(float4 pos : SV_Position) : SV_Position
{
	return pos;
}
