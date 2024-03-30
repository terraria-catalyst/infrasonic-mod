sampler inputImage : register(s0);

float threshold;

float4 shade;

float4 PixelShaderFunction(float2 uv: TEXCOORD0) : COLOR0
{
    float4 color = tex2D(inputImage, uv);

    float brightness = color.x + color.y + color.z + color.w;
    brightness /= 4;

    if (any(color) && brightness > threshold)
    {
        return shade;
    }

    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass HeadShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};