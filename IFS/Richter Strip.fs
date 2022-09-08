/*
{
	"NAME": "Richter Strip",
	"DESCRIPTION": "Stretches an image like Gerhard Richter's Strip",
	"CREDIT": "Baku Hashimoto <baku89.com>",
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "origin",
			"LABEL": "Origin",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		},
		{
			"NAME": "angle",
			"LABEL": "Angle",
			"TYPE": "float",
			"UNIT": "direction"
		},
		{
			"NAME": "i4a_CustomUI",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "i4a_UIForegroundColor",
			"TYPE": "color"
		},
		{
			"NAME": "i4a_UIStrokeWidth",
			"TYPE": "float",
			"DEFAULT": 2
		}
	]
}
*/

vec2 toNDC(vec2 uv) {
  return (uv * 2.0 - 1.0) * vec2(1.0, RENDERSIZE.y / RENDERSIZE.x);
}

vec2 toUV(vec2 ndc) {
  return ((ndc / vec2(1.0, RENDERSIZE.y / RENDERSIZE.x) / 2.0) + 0.5);
}

void main() {
  vec2 offset = (origin - 0.5);
  vec2 pos = toNDC(isf_FragNormCoord.xy - offset);

  if (i4a_CustomUI) {
    vec2 perp = vec2(cos(angle), sin(angle));

    float d = abs(dot(pos, perp));

    float dx = 1.0 / RENDERSIZE.x;

    float mask = smoothstep(dx * (i4a_UIStrokeWidth + 1.0), dx * i4a_UIStrokeWidth, d);

    gl_FragColor = i4a_UIForegroundColor * vec4(vec3(1.0), mask);

  } else {
    vec2 dir = vec2(-sin(angle), cos(angle));

    float t = dot(pos, dir);

    pos = dir * t;

    vec2 uv = toUV(pos) + offset;
    ;

    gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
  }
}
