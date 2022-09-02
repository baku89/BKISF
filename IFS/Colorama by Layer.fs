/*
{
	"NAME": "Colorama by Layer",
	"DESCRIPTION": "Applies Colorama by sampling gradient map from another image",
	"CREDIT": "Baku Hashimoto",
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "gradientImage",
			"LABEL": "Gradient Image",
			"TYPE": "image"
		},
		{
			"NAME": "start",
			"LABEL": "Start Position",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0.5
			]
		},
		{
			"NAME": "end",
			"LABEL": "End Position",
			"TYPE": "point2D",
			"DEFAULT": [
				1,
				0.5
			]
		}
	]
}
*/

void main() {
  vec4 color = IMG_THIS_PIXEL(inputImage);

  float lum = dot(color.rgb, vec3(1.0 / 3.0));

  vec2 uv = mix(start, end, lum);

  gl_FragColor = IMG_NORM_PIXEL(gradientImage, uv);
}
