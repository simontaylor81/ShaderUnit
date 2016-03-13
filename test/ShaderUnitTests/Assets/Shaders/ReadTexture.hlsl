// Simple compute shader that reads the contents of a texture.
Texture2D<float4> Texture;
RWStructuredBuffer<float4> OutBuffer;

// Not efficient workgroup size, but we're running on WARP so who cares, this is simpler.
[numthreads(1, 1, 1)]
void Main(uint3 id : SV_DispatchThreadID)
{
	uint width, height;
	Texture.GetDimensions(width, height);

	OutBuffer[id.y * width + id.x] = Texture.Load(uint3(id.xy, 0));
}
