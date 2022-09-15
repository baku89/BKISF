/*
{
	"DESCRIPTION": "Applies a hatching-like effect with using a pattern image",
	"ISFVSN": "2",
	"CREDIT": "Baku Hashimoto <baku89.com>",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "hatchImage",
			"LABEL": "Hatch Image",
			"TYPE": "image"
		},
		{
			"NAME": "scale",
			"LABEL": "Scale",
			"TYPE": "float",
			"DEFAULT": 1,
			"UNIT": "percent"
		},
		{
			"NAME": "influence",
			"LABEL": "Influence",
			"TYPE": "float",
			"UNIT": "percent",
			"CLAMP_MIN": true,
			"CLAMP_MAX": true,
			"MIN": 0,
			"MAX": 1,
			"DEFAULT": 1
		},
		{
			"NAME": "quality",
			"LABEL": "Quality",
			"TYPE": "long",
			"VALUES": [1, 2, 4],
			"LABELS": ["Low", "Medium", "High"]
		}
	]
}
*/

vec4 sample(float ox, float oy) {
  vec2 coord = gl_FragCoord.xy + vec2(ox, oy);
  vec4 color = IMG_PIXEL(inputImage, coord);
  vec4 hatch = IMG_PIXEL(hatchImage, mod(gl_FragCoord.xy / scale, IMG_SIZE(hatchImage)));

  return step(1.0 - hatch, color);
}

void main() {
  int sampleNum = int(quality);
  float sampleNumF = float(sampleNum);

  vec4 original = IMG_THIS_PIXEL(inputImage);

  vec4 ret = vec4(0.0);

  for (int y = 0; y < sampleNum; y++) {
    for (int x = 0; x < sampleNum; x++) {
      ret += sample(float(x) / sampleNumF, float(y) / sampleNumF);
    }
  }

  ret /= pow(sampleNumF, 2.0);

  gl_FragColor = mix(original, ret, influence);
}