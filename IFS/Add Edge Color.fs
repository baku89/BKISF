/*
{
	"NAME": "Add Edge Color",
	"DESCRIPTION": "Adds a gradient to the four corners of the input image that changes to the specified color.",
	"CREDIT": "Baku Hashimoto <baku89.com>",
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "edgeColor",
			"LABEL": "Edge Color",
			"TYPE": "color",
			"DEFAULT": [0.5, 0.5, 0.5, 1.0]
		},
		{
			"NAME": "width",
			"LABEL": "Width",
			"TYPE": "point2D",
			"DEFAULT": [0.1, 0.1]
		}
	]
}
*/

float blend(float e, float a, float b) {
    return pow(pow(a, e) + pow(b, e), 1.0 / e);
}

void main() {
	vec2 uv = isf_FragNormCoord.xy;

	vec4 inputColor = IMG_NORM_PIXEL(inputImage, uv);

	float e = 2.0; // roundness (2.0 == circle)

	// Represents the weight of edge color (1.0 == edge).
	float w = 0.0;
	w = blend(e, w, smoothstep(width.x, 0.0, uv.x));
	w = blend(e, w, smoothstep(width.y, 0.0, uv.y));
	w = blend(e, w, smoothstep(1.0 - width.x, 1.0, uv.x));
	w = blend(e, w, smoothstep(1.0 - width.y, 1.0, uv.y));
	
	gl_FragColor = mix(inputColor, edgeColor, w);
}