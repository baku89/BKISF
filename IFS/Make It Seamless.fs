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
			"NAME": "size",
			"LABEL": "Size",
			"TYPE": "point2D",
			"DEFAULT": [0.9, 0.1]
		},
		{
			"NAME": "maskWarping",
			"LABEL": "Mask Warping",
			"TYPE": "float",
			"UNIT": "percent",
			"MIN": 0,
			"MAX": 1,
			"DEFAULT": 0.5,
			"CLAMP_MIN": true
		},
		{
			"NAME": "noiseScale",
			"LABEL": "Scale of Mask Noise",
			"TYPE": "float",
			"UNIT": "percent",
			"MIN": 0,
			"MAX": 1,
			"DEFAULT": 1
		},
		{
			"NAME": "noiseComplexity",
			"LABEL": "Complexity of Mask Noise",
			"TYPE": "float",
			"MIN": 2,
			"MAX": 10,
			"CLAMP_MIN": true,
			"DEFAULT": 4
		}
	]
}
*/

vec3 permute(vec3 x) {
  return mod(((x * 34.0) + 1.0) * x, 289.0);
}

float snoise(vec2 v) {
  const vec4 C = vec4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
  vec2 i = floor(v + dot(v, C.yy));
  vec2 x0 = v - i + dot(i, C.xx);
  vec2 i1;
  i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
  vec4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;
  i = mod(i, 289.0);
  vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0)) + i.x + vec3(0.0, i1.x, 1.0));
  vec3 m = max(0.5 - vec3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
  m = m * m;
  m = m * m;
  vec3 x = 2.0 * fract(p * C.www) - 1.0;
  vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5);
  vec3 a0 = x - ox;
  m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
  vec3 g;
  g.x = a0.x * x0.x + h.x * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

float snoiseFractal(vec2 pos, int c) {
  float ret = 0.0;
  for (int i = 0; i < c; i++) {
    float s = pow(2.0, float(c - i - 1));
    ret += snoise(pos * pow(2.0, float(i))) * s;
  }
  return ret / (pow(2.0, float(c)) - 1.0);
}

float noise(vec2 pos) {
  int Cc = int(noiseComplexity + 1.0);
  int Cf = int(noiseComplexity);
  float fr = fract(noiseComplexity);

  return mix(snoiseFractal(pos, Cf), snoiseFractal(pos, Cc), fr) * 1.3;
}

float linearstep(float lower, float upper, float t) {
  return (t - lower) / (upper - lower);
}

vec4 repeatX(vec2 uv) {
  vec2 noisePos = vec2(uv.x, 1.0 - uv.y) * vec2(1.0, RENDERSIZE.y / RENDERSIZE.x) * 10.0 * noiseScale;

  float nf = noise(noisePos) * maskWarping;

  vec4 here = IMG_NORM_PIXEL(inputImage, uv);
  vec4 repX = IMG_NORM_PIXEL(inputImage, uv + vec2(size.x, 0.0));

  float xInterp = linearstep(0.0, min(1.0 - size.x, size.x), uv.x);

  xInterp = (1.0 + 2.0 * maskWarping) * xInterp - maskWarping + nf;
  xInterp = clamp(xInterp, 0.0, 1.0);

  return mix(repX, here, xInterp);
}

void main() {
  vec2 uv = isf_FragNormCoord.xy;

  vec2 noisePos = vec2(uv.x, 1.0 - uv.y) * vec2(1.0, RENDERSIZE.y / RENDERSIZE.x) * 10.0 * noiseScale;

  if (uv.x > size.x || uv.y < size.y) {
    gl_FragColor = vec4(0.0);
  } else {
    vec4 here = repeatX(uv);
    vec4 repY = repeatX(uv - vec2(0.0, 1.0 - size.y));

    float yInterp = linearstep(1.0, max(1.0 - size.y, size.y), uv.y);

    float nf = noise(noisePos) * maskWarping;

    yInterp = (1.0 + 2.0 * maskWarping) * yInterp - maskWarping + nf;
    yInterp = clamp(yInterp, 0.0, 1.0);

    gl_FragColor = mix(repY, here, yInterp);
  }
}