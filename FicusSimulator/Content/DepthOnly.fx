// DepthOnly.fx

// Simple vertex shader
float4x4 WorldViewProjection;

struct VertexInput
{
    float4 Position : POSITION;
};

struct VertexOutput
{
    float4 Position : POSITION;
};

VertexOutput VS(VertexInput input)
{
    VertexOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    return output;
}

// Simple pixel shader, no color output, just writing to depth
float PS(VertexOutput input) : SV_DEPTH
{
    return input.Position.z; // Output depth value
}

// Define the technique and pass
technique DepthOnly
{
    pass P0
    {
        // Define the shaders for this pass
        VertexShader = compile vs_5_0 VS();
        PixelShader = compile ps_5_0 PS();
    }
}
