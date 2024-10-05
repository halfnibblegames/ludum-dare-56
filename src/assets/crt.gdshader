shader_type canvas_item;

uniform float vignetteWidth;
uniform float curvature;
uniform vec2 screenSize;
uniform vec4 offScreenColor;

void fragment() { 
    vec2 centeredUv = SCREEN_UV * 2.0 - 1.0;
    vec2 offset = centeredUv.yx / curvature;
    vec2 uvToSample = (centeredUv + centeredUv * offset * offset) * 0.5 + 0.5;

    if (uvToSample.x <= 0.0f || 1.0f <= uvToSample.x || uvToSample.y <= 0.0f || 1.0f <= uvToSample.y) {
        COLOR = offScreenColor
    } else {
        vec4 color = texture(SCREEN_TEXTURE, uvToSample);
        vec2 centeredSampledUv = uvToSample * 2.0f - 1.0f;
        vec2 vignette = clamp(smoothstep(vec2(0.0f), vignetteWidth / screenSize.xy, 1.0f - abs(centeredSampledUv)), 0.0f, 1.0f);

        color.g *= (sin(SCREEN_UV.y * screenSize.y) + 1.0f) * 0.15f + 1.0f;
        color.rb *= (cos(SCREEN_UV.y * screenSize.y) + 1.0f) * 0.135f + 1.0f;

        COLOR = clamp(color, 0.0f, 1.0f) * vignette.x * vignette.y;
    }
}