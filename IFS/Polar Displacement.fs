/*
{
	"DESCRIPTION": "Displaces an image in polar coordinate",
	"ISFVSN": "2",
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
			"NAME": "distChannelIndex",
			"LABEL": "Channel for Distance",
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
			"NAME": "angleChannelIndex",
			"TYPE": "long",
			"LABEL": "Channel for Angle",
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
			"NAME": "dist",
			"LABEL": "Distance",
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
			"NAME": "direction",
			"LABEL": "Direction",
			"UNIT": "direction",
			"TYPE": "float"
		},
		{
			"NAME": "angleScale",
			"LABEL": "Angle Scale",
			"UNIT": "percent",
			"TYPE": "float",
			"DEFAULT": 1
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

void main() {
  vec2 suv = uv2suv(isf_FragNormCoord.xy);

  int intSubsteps = int(substeps);
  float distPerStep = dist / float(intSubsteps);

  for (int i = 0; i < intSubsteps; i++) {
    vec4 sampledMatte = IMG_NORM_PIXEL(matte, suv2uv(suv));

    float distCh = sampledMatte[distChannelIndex];
    float angleCh = sampledMatte[angleChannelIndex];

    float angle = (angleCh * TAU * angleScale) + direction;

    vec2 offset = vec2(cos(angle), sin(angle)) * distPerStep * distCh;

    suv -= offset;
  }

  vec2 uv = suv2uv(wrapPixelsAround ? mirrored(suv) : suv);

  gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
}