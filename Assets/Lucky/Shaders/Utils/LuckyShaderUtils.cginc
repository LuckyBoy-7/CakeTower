#ifndef LuckyShaderUtils
#define LuckyShaderUtils


/// 根据法线方向和上, 下, 周围一圈对应的贴图获取该像素颜色
float4 getUpAroundBottomColorWithTex(sampler2D upTex, sampler2D aroundTex, sampler2D bottomTex, float2 upUV, float2 aroundUV, float2 bottomUV, float3 normal)
{
    float4 colorUp = tex2D(upTex, upUV);
    float4 colorAround = tex2D(aroundTex, aroundUV);
    float4 colorBottom = tex2D(bottomTex, bottomUV);

    float4 color = lerp(colorAround, colorUp, step(0.5, normal.y));
    color = lerp(color, colorBottom, step(0.5, -normal.y));

    return color;
}

#endif
