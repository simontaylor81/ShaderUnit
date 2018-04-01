Texture2D<float4> tex1;
Texture2D<float4> tex2;
Texture2D<float4> texMask;
SamplerState samp;

void vsMain(
	in float4 pos : POSITION,
	out float2 outUV : TEXCOORD,
	out float4 outPos : SV_Position)
{
	outPos = pos;

	outUV = pos.xy * 0.5 + 0.5;
	outUV.y = 1 - outUV.y;
}

float4 psMain(float2 uv : TEXCOORD) : SV_Target
{
	float4 t1 = tex1.SampleLevel(samp, uv, 0);
	float4 t2 = tex2.SampleLevel(samp, uv, 0);
	float4 mask = texMask.SampleLevel(samp, uv, 0);
	float4 result = lerp(t1, t2, mask);
	//result.a = 1;
	return result;
}
