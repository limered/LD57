shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE:hint_screen_texture, filter_linear_mipmap;
uniform float distortion_strength = 0.0002;
uniform vec4 water_color : source_color = vec4(0.2, 0.6, 0.8, 0.5);
uniform float edge_vignette = 2.0;

void vertex() {
}

void fragment() {
    // Screen distortion using noise and time
    vec2 distorted_uv = SCREEN_UV;
    distorted_uv.x += cos(TIME * 2.0 + SCREEN_UV.y * 20.0) * distortion_strength;
    distorted_uv.y += sin(TIME * 1.5 + SCREEN_UV.x * 15.0) * distortion_strength;
    
    // Base color with depth-based attenuation
    vec3 screen_color = textureLod(SCREEN_TEXTURE, distorted_uv, 0.0).rgb;
    float depth_factor = 1.0 - smoothstep(0.3, 1.0, length(SCREEN_UV - 0.5));
    vec3 final_color = mix(screen_color, water_color.rgb, water_color.a * depth_factor);
    
    // Edge vignette effect
    vec2 center = SCREEN_UV - 0.5;
    float vignette = 1.0 - dot(center, center) * edge_vignette;
    final_color *= vignette;
    
    COLOR = vec4(final_color, 1.0);
}

