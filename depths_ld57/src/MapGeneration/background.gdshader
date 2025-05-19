shader_type canvas_item;

void vertex() {
}

uniform float displacement_strength = 0.05;

const vec3 strong_pink = vec3(0.992, 0.569, 0.733);
const vec3 soft_pink = vec3(0.984, 0.875, 0.91);
const vec3 gray = vec3(0.263, 0.263, 0.263);
const vec3 light_blue = vec3(0.592, 0.886, 0.965);
const vec3 white = vec3(1.0);
const vec3 black = vec3(0.0);

void fragment() {
	float tex = 1.0-texture(TEXTURE, UV).r; // TODO invert again
	
	float size = 0.005;
	float acc = 0.0;
	int iter = 0;
	for (float i = -size; i <= size; i += 0.001) {
		for (float j = -size; j <= size; j += 0.001) {
			vec2 uv = UV + vec2(i, j);
			acc += 1.0-texture(TEXTURE, uv).r;
			iter++;
		}
	}
	acc /= float(iter);
	
	vec3 col;
	if(tex > 0.5){
		col = mix(gray, strong_pink, acc);
	}else{
		col = mix(gray, strong_pink, 0.2*acc);
	}
	
	COLOR = vec4(col, 1.0);
}
