/*
{
	"DESCRIPTION": "Displaces an image along the gradient of matte",
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
			"NAME": "gradChannelIndex",
			"LABEL": "Use for Gradient",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3,
				4
			],
			"LABELS": [
				"Red",
				"Green",
				"Blue",
				"Alpha",
				"Luminance"
			],
			"DEFAULT": 4
		},
		{
			"NAME": "dist",
			"LABEL": "Max Displacement",
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
	],
	"PASSES": [
		{
			"TARGET": "sobelImage",
			"FLOAT": true
		},
		{
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

vec2 sobel(vec2 uv) {
  vec2 coord = uv * IMG_SIZE(matte);

  vec4 n[9];
  n[0] = IMG_PIXEL(matte, coord + vec2(-1.0, -1.0));
  n[1] = IMG_PIXEL(matte, coord + vec2(0.00, -1.0));
  n[2] = IMG_PIXEL(matte, coord + vec2(+1.0, -1.0));
  n[3] = IMG_PIXEL(matte, coord + vec2(-1.0, 0.00));
  n[4] = IMG_PIXEL(matte, coord + vec2(0.00, 0.00));
  n[5] = IMG_PIXEL(matte, coord + vec2(+1.0, 0.00));
  n[6] = IMG_PIXEL(matte, coord + vec2(-1.0, +1.0));
  n[7] = IMG_PIXEL(matte, coord + vec2(0.00, +1.0));
  n[8] = IMG_PIXEL(matte, coord + vec2(+1.0, +1.0));

  vec4 h = n[2] + (2.0 * n[5]) + n[8] - (n[0] + (2.0 * n[3]) + n[6]);
  vec4 v = n[0] + (2.0 * n[1]) + n[2] - (n[6] + (2.0 * n[7]) + n[8]);

  vec4 chs = vec4(0.0);

  if (gradChannelIndex < 4) {
    chs[0] = 1.0;
  } else {
    chs.rgb = vec3(1.0 / 3.0);
  }

  return vec2(-dot(h, chs), dot(v, chs));
}

void main() {
  if (PASSINDEX == 0) {
    gl_FragColor = vec4(sobel(isf_FragNormCoord.xy), 0.0, 1.0);
  } else {
    vec2 suv = uv2suv(isf_FragNormCoord.xy);

    int intSubsteps = int(substeps);
    float distPerStep = dist / float(intSubsteps);

    for (int i = 0; i < intSubsteps; i++) {
      vec2 grad = IMG_NORM_PIXEL(sobelImage, suv2uv(suv)).xy;

      vec2 offset = rotate(grad, -rotation) * distPerStep;

      suv -= offset;
    }

    vec2 uv = suv2uv(wrapPixelsAround ? mirrored(suv) : suv);

    gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
  }
}