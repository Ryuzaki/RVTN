float AlphaPercentaje;
float4x4 World;
float4x4 View;
float4x4 Projection;
Texture TextureOrigin;

sampler TextureSamplerOrigin = sampler_state
{
	texture = <TextureOrigin>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;

};

Texture TextureDestiny;

sampler TextureSamplerDestiny = sampler_state
{
	texture = <TextureDestiny>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;

};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TextureCoordinates = input.TextureCoordinates;

    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 colorOrigen = (1 - AlphaPercentaje) * (tex2D(TextureSamplerOrigin, input.TextureCoordinates) + tex2D(TextureSamplerOrigin, input.TextureCoordinates.xy - (0.001)) + tex2D(TextureSamplerOrigin, input.TextureCoordinates.xy + (0.001)));
	colorOrigen = colorOrigen / 3;
	float4 colorDestino = AlphaPercentaje * (tex2D(TextureSamplerDestiny, input.TextureCoordinates) + tex2D(TextureSamplerDestiny, input.TextureCoordinates.xy - (0.001)) + tex2D(TextureSamplerDestiny, input.TextureCoordinates.xy + (0.001)));
	colorDestino = colorDestino / 3;
    return colorOrigen + colorDestino;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
