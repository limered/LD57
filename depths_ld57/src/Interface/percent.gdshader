shader_type canvas_item;

uniform float progress : hint_range(0, 1) = 0.2;
uniform float radius : hint_range(0, 1) = 0.38;
uniform float edge_smoothness : hint_range(0, 0.05) = 0.01;

const vec4 strong_pink = vec4(0.992, 0.569, 0.733, 1.0);
const vec4 soft_pink = vec4(0.984, 0.875, 0.91, 1.0);
const vec4 gray = vec4(0.263, 0.263, 0.263, 1.0);
const vec4 light_blue = vec4(0.592, 0.886, 0.965, 1.0);
const vec4 white = vec4(1.0);
const vec4 black = vec4(0.0);

const float outer_radius = 0.45;
const float background_radius = 0.40;
const float inner_radius = 0.33;

void fragment() {
    vec2 center = vec2(0.5);
    vec2 dir = UV - center;
    float dist = length(dir);

    float angle = atan(dir.y, dir.x) + 1.57;
    angle = angle < 0.0 ? angle + 6.28318530718 : angle;

    float progress_angle = progress * 6.28318530718;

    float in_circle = smoothstep(radius + edge_smoothness, radius - edge_smoothness, dist);
    float in_angle = step(angle, progress_angle);

	vec4 col = gray;
	if (dist > outer_radius){
		discard;
	}
	if (dist < background_radius){
		col = white;
	}
	if (dist > inner_radius)
    	COLOR = mix(col, strong_pink, in_angle * in_circle);
	if(dist < inner_radius)
		COLOR = soft_pink;
}