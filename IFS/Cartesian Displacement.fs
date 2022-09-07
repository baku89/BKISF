/*
{
	"DESCRIPTION": "Displaces an image in cartesian coordinate",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "matte",
			"LABEL": "Matte",
			"TYPE": "image"
		},
		{
			"NAME": "xChannelIndex",
			"LABEL": "Use for X Displacement",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3
			],
			"LABELS": [
				"Red",
				"Green",
				"Blue",
				"Alpha"
			],
			"DEFAULT": 0
		},
		{
			"NAME": "yChannelIndex",
			"TYPE": "long",
			"LABEL": "Use for Y Displacement",
			"VALUES": [
				0,
				1,
				2,
				3
			],
			"LABELS": [
				"Red",
				"Green",
				"Blue",
				"Alpha"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "xDist",
			"LABEL": "Max X Displacement",
			"TYPE": "float",
			"UNIT": "length",
			"DEFAULT": 0.1
		},
		{
			"NAME": "yDist",
			"LABEL": "Max Y Displacement",
			"TYPE": "float",
			"UNIT": "length",
			"DEFAULT": 0.1
		},
		{
			"NAME": "substeps",
			"LABEL": "Substeps",
			"TYPE": "float",
			"MIN": 1,
			"MAX": 100,
			"DEFAULT": 20,
			"CLAMP_MIN": true
		},
		{
			"NAME": "rotation",
			"LABEL": "Rotation",
			"UNIT": "angle",
			"TYPE": "float"
		},
		{
			"NAME": "wrapPixelsAround",
			"LABEL": "Wrap Pixels Around",
			"TYPE": "bool",
			"DEFAULT": true
		}
	]
}
*/

#define TAU 6.28318530718

vec2 mirrored(vec2 v) {
  vec2 m = mod(v, 2.0);
  return mix(m, 2.0 - m, step(1.0, m));
}

vec2 uv2suv(vec2 uv) {
  return uv * vec2(1.0, RENDERSIZE.y / RENDERSIZE.x);
}

vec2 suv2uv(vec2 suv) {
  return suv / vec2(1.0, RENDERSIZE.y / RENDERSIZE.x);
}

vec2 rotate(vec2 v, float a) {
  float s = sin(a);
  float c = cos(a);
  mat2 m = mat2(c, -s, s, c);
  return m * v;
}

void main() {
  vec2 suv = uv2suv(isf_FragNormCoord.xy);

  int intSubsteps = int(substeps);
  vec2 distPerStep = vec2(xDist, yDist) / float(intSubsteps);

  for (int i = 0; i < intSubsteps; i++) {
    vec4 sampledMatte = IMG_NORM_PIXEL(matte, suv2uv(suv));

    vec2 disp = vec2(sampledMatte[xChannelIndex], sampledMatte[yChannelIndex]);

    disp = rotate(disp, -rotation);

    vec2 offset = disp * distPerStep;

    suv -= offset;
  }

  vec2 uv = suv2uv(wrapPixelsAround ? mirrored(suv) : suv);

  gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
}