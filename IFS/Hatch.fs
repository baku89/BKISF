/*
{
	"DESCRIPTION": "Places the input image repeatedly with feathered edge, attempting to turn it into a tiling texture",
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
			"TYPE": "float",
			"DEFAULT": 1,
			"UNIT": "percent"
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

  vec4 ret = vec4(0.0);

  for (int y = 0; y < sampleNum; y++) {
    for (int x = 0; x < sampleNum; x++) {
      ret += sample(float(x) / sampleNumF, float(y) / sampleNumF);
    }
  }

  gl_FragColor = ret / pow(sampleNumF, 2.0);
}