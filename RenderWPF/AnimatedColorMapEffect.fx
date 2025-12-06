// ============================================================
// 1. AnimatedColorMapEffect.fx (Shader HLSL avec animation)
// ============================================================
// Compilez avec: fxc /T ps_3_0 /Fo AnimatedColorMapEffect.ps AnimatedColorMapEffect.fx
// C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64
sampler2D Input : register(s0);
float ColorMapType : register(c0);
float Time : register(c1);
float AnimationType : register(c2);
float AnimationSpeed : register(c3);

// Colormaps (même code que précédemment)
float4 ApplyJet(float value)
{
    float4 color;
    if (value < 0.125)
    {
        color = float4(0, 0, 0.5 + value * 4.0, 1);
    }
    else if (value < 0.375)
    {
        float t = (value - 0.125) * 4.0;
        color = float4(0, t, 1, 1);
    }
    else if (value < 0.625)
    {
        float t = (value - 0.375) * 4.0;
        color = float4(t, 1, 1 - t, 1);
    }
    else if (value < 0.875)
    {
        float t = (value - 0.625) * 4.0;
        color = float4(1, 1 - t, 0, 1);
    }
    else
    {
        color = float4(1 - (value - 0.875) * 2.0, 0, 0, 1);
    }
    return color;
}

float4 ApplyHot(float value)
{
    float4 color;
    if (value < 0.375)
    {
        color = float4(value / 0.375, 0, 0, 1);
    }
    else if (value < 0.75)
    {
        float t = (value - 0.375) / 0.375;
        color = float4(1, t, 0, 1);
    }
    else
    {
        float t = (value - 0.75) / 0.25;
        color = float4(1, 1, t, 1);
    }
    return color;
}

float4 ApplyViridis(float value)
{
    float4 c0 = float4(0.267, 0.005, 0.329, 1);
    float4 c1 = float4(0.283, 0.141, 0.458, 1);
    float4 c2 = float4(0.253, 0.265, 0.530, 1);
    float4 c3 = float4(0.164, 0.471, 0.558, 1);
    float4 c4 = float4(0.134, 0.659, 0.518, 1);
    float4 c5 = float4(0.477, 0.821, 0.318, 1);
    float4 c6 = float4(0.993, 0.906, 0.144, 1);
    
    if (value < 0.166) return lerp(c0, c1, value * 6.0);
    else if (value < 0.333) return lerp(c1, c2, (value - 0.166) * 6.0);
    else if (value < 0.5) return lerp(c2, c3, (value - 0.333) * 6.0);
    else if (value < 0.666) return lerp(c3, c4, (value - 0.5) * 6.0);
    else if (value < 0.833) return lerp(c4, c5, (value - 0.666) * 6.0);
    else return lerp(c5, c6, (value - 0.833) * 6.0);
}

// ============================================================
// ANIMATIONS DANS LE SHADER
// ============================================================

// Animation Type 0: Vagues sinusoïdales
float AnimateWaves(float2 uv, float baseValue)
{
    float wave1 = sin(uv.x * 20.0 + Time * AnimationSpeed);
    float wave2 = cos(uv.y * 15.0 + Time * AnimationSpeed * 0.7);
    float wave3 = sin((uv.x + uv.y) * 10.0 - Time * AnimationSpeed * 0.5);
    
    float waves = (wave1 + wave2 + wave3) / 3.0;
    return baseValue * (waves * 0.3 + 0.7);
}

// Animation Type 1: Rotation et pulsation
float AnimateRotatingPulse(float2 uv, float baseValue)
{
    float2 center = float2(0.5, 0.5);
    float2 delta = uv - center;
    
    // Rotation
    float angle = Time * AnimationSpeed * 0.5;
    float2 rotated;
    rotated.x = delta.x * cos(angle) - delta.y * sin(angle);
    rotated.y = delta.x * sin(angle) + delta.y * cos(angle);
    
    float dist = length(rotated);
    float pulse = sin(dist * 10.0 - Time * AnimationSpeed * 2.0) * 0.5 + 0.5;
    
    return baseValue * pulse;
}

// Animation Type 2: Turbulence (bruit simplifié)
float AnimateTurbulence(float2 uv, float baseValue)
{
    float2 p = uv * 10.0;
    
    float turb = 0.0;
    turb += sin(p.x + Time * AnimationSpeed) * cos(p.y);
    turb += sin(p.y * 1.3 + Time * AnimationSpeed * 0.8) * cos(p.x * 0.8);
    turb += sin((p.x + p.y) * 0.7 - Time * AnimationSpeed * 0.6) * 0.5;
    
    turb = turb * 0.3 + 0.5;
    
    return baseValue * turb;
}

// Animation Type 3: Effet de ripple (ondulations)
float AnimateRipple(float2 uv, float baseValue)
{
    float2 center = float2(0.5, 0.5);
    float dist = distance(uv, center);
    
    float ripple = sin(dist * 30.0 - Time * AnimationSpeed * 3.0) * 0.5 + 0.5;
    float fade = 1.0 - smoothstep(0.0, 0.7, dist);
    
    return baseValue * (ripple * fade + (1.0 - fade));
}

// Animation Type 4: Déplacement horizontal (scrolling)
float AnimateScroll(float2 uv, float baseValue)
{
    float2 scrolledUV = uv;
    scrolledUV.x += Time * AnimationSpeed * 0.1;
    scrolledUV.x = frac(scrolledUV.x); // Wrap
    
    float pattern = sin(scrolledUV.x * 30.0) * cos(scrolledUV.y * 25.0);
    return baseValue * (pattern * 0.3 + 0.7);
}

// Animation Type 5: Zoom pulsant
float AnimateZoom(float2 uv, float baseValue)
{
    float2 center = float2(0.5, 0.5);
    float zoom = 1.0 + sin(Time * AnimationSpeed) * 0.3;
    
    float2 zoomedUV = (uv - center) * zoom + center;
    
    // Si hors limites, on atténue
    if (zoomedUV.x < 0.0 || zoomedUV.x > 1.0 || 
        zoomedUV.y < 0.0 || zoomedUV.y > 1.0)
    {
        return baseValue * 0.5;
    }
    
    return baseValue;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    // Lire la valeur de base de la texture
    float4 texColor = tex2D(Input, uv);
    float baseValue = texColor.r;
    
    // Appliquer l'animation
    float animatedValue = baseValue;
    
    if (AnimationType < 0.5)
    {
        animatedValue = AnimateWaves(uv, baseValue);
    }
    else if (AnimationType < 1.5)
    {
        animatedValue = AnimateRotatingPulse(uv, baseValue);
    }
    else if (AnimationType < 2.5)
    {
        animatedValue = AnimateTurbulence(uv, baseValue);
    }
    else if (AnimationType < 3.5)
    {
        animatedValue = AnimateRipple(uv, baseValue);
    }
    else if (AnimationType < 4.5)
    {
        animatedValue = AnimateScroll(uv, baseValue);
    }
    else
    {
        animatedValue = AnimateZoom(uv, baseValue);
    }
    
    // Clamper la valeur
    animatedValue = saturate(animatedValue);
    
    // Appliquer la colormap
    float4 color;
    if (ColorMapType < 0.5)
    {
        color = ApplyJet(animatedValue);
    }
    else if (ColorMapType < 1.5)
    {
        color = ApplyHot(animatedValue);
    }
    else
    {
        color = ApplyViridis(animatedValue);
    }
    
    return color;
}
