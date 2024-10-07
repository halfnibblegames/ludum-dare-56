shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.5;

void fragment() {
  vec4 color = texture(TEXTURE, UV).rgba;
  float a = (floor(progress * 4.0) + 1.0) * 0.25;
  a = clamp(a, 0.0, 1.0);

  COLOR.rgb = color.rgb;
  COLOR.a = min(color.a, a);
}
