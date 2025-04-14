shader_type canvas_item;

uniform sampler2D map;
uniform vec2 position = vec2(0.2, 0.2);

const vec3 strong_pink = vec3(0.992, 0.569, 0.733);
const vec3 soft_pink = vec3(0.984, 0.875, 0.91);
const vec3 gray = vec3(0.263, 0.263, 0.263);
const vec3 light_blue = vec3(0.592, 0.886, 0.965);
const vec3 white = vec3(1.0);
const vec3 black = vec3(0.0);

void fragment() {
	float c = texture(map, UV).r;
	vec3 col;

	if(c < 0.5){
		col = strong_pink;
	}else{
		col = gray;
	}

	vec2 dir = UV - position;
	float len = length(dir);

	float dot_strength = (cos(TIME) + 3.0) * 0.2;
	float red_dot = smoothstep(0.01, 0.05 * dot_strength, len);

	COLOR = mix(vec4(soft_pink, 1.0), vec4(col, 1.0), red_dot);
}