float4x4 World;
float4x4 View;
float4x4 Projection;
float2 ViewportScale;

texture Texture;
texture MaskTexture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler MaskSampler = sampler_state
{
    Texture = (MaskTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
	float2 Corner : POSITION0;
	float4 SpriteCenter : POSITION1;
	float4 Color : COLOR;
	float Size : TEXCOORD0;
	float Rotation : TEXCOORD1;
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

	// Compute a 2x2 rotation matrix.
    float c = cos(input.Rotation);
    float s = sin(input.Rotation);
    
    float2x2 rotationMatrix = float2x2(c, -s, s, c);

    output.Position = mul(mul(input.SpriteCenter, View), Projection);
	output.Position.xy +=  mul(rotationMatrix, input.Corner) * ViewportScale * input.Size * Projection._m11;

	output.TexCoord = input.Corner + 0.5;
	output.Color = input.Color;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(Sampler, input.TexCoord) * input.Color;
	float4 mask = tex2D(MaskSampler, input.TexCoord);
	float alpha = mask.r;
	result.rgb *= alpha;
	result.a = alpha;
	return result;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();

		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
	}
}
