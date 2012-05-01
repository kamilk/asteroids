float4x4 World;
float4x4 View;
float4x4 Projection;

texture Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
	float2 Corner : POSITION0;
	float4 ParticleCenter : POSITION1;
	float4 Color : COLOR;
	float Size : POINTSIZE0;
	float Rotation : POINTSIZE1;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4 position = input.ParticleCenter + float4(input.Corner, 0, 0);

    float4 worldPosition = mul(position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.Corner + 0.5;
	output.Color = input.Color;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(Sampler, input.TexCoord) * input.Color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
