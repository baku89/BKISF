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
			"NAME": "shadowLength",
			"LABEL": "Shadow Length",
			"TYPE": "float",
			"DEFAULT": 100,
			"MIN": 0,
			"MAX": 100,
			"CLAMP_MIN": true
		},
		{
			"NAME": "angle",
			"LABEL": "Angle",
			"TYPE": "float",
			"UNIT": "direction"
		},
		{
			"NAME": "startColor",
			"LABEL": "Start Color",
			"TYPE": "color",
			"DEFAULT": [1, 0, 0, 1]
		},
		{
			"NAME": "startAlpha",
			"LABEL": "Start Alpha",
			"TYPE": "float",
			"UNIT": "percent",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1,
			"CLAMP_MIN": true,
			"CLAMP_MAX": true
		},
		{
			"NAME": "endColor",
			"LABEL": "End Color",
			"TYPE": "color",
			"DEFAULT": [0, 0, 1, 1]
		},
		{
			"NAME": "endAlpha",
			"LABEL": "End Alpha",
			"TYPE": "float",
			"UNIT": "percent",
			"DEFAULT": 0,
			"MIN": 0,
			"MAX": 1,
			"CLAMP_MIN": true,
			"CLAMP_MAX": true
		},
		{
			"NAME": "shadowOnly",
			"LABEL": "Shadow Only",
			"TYPE": "bool",
			"DEFAULT": false
		}
	]
}
*/

float blendOverAlpha(vec4 A, vec4 B) {
  return B.a + A.a * (1. - B.a);
}

vec4 blendOver(vec4 A, vec4 B) {
  float alpha = B.a + A.a * (1. - B.a);
  vec3 rgb = (B.rgb * B.a + A.rgb * A.a * (1. - B.a)) / alpha;
  return vec4(rgb, alpha);
}

vec2 blendOver2(vec2 A, vec2 B) {
  float alpha = B.y + A.y * (1. - B.y);
  float lum = (B.x * B.y + A.x * A.y * (1. - B.y)) / alpha;
  return vec2(lum, alpha);
}

void main() {
  int shadowLengthPx = int(shadowLength);
  if (shadowLengthPx <= 1 && shadowOnly) {
    gl_FragColor = vec4(startColor.rgb, IMG_THIS_PIXEL(inputImage).a);
  } else {
    vec2 coord = gl_FragCoord.xy;

    vec2 delta = -1.0 * vec2(cos(angle), sin(angle));

    vec4 color = vec4(0.0);
    float alpha = 1.0;

    for (int i = 0; i < shadowLengthPx; i++) {
      float t = float(i) / max(1.0, float(shadowLengthPx - 1));
      vec3 coloAtHere = mix(startColor.rgb, endColor.rgb, t);
      float startAlphatHere = mix(startAlpha, endAlpha, t);

      coord += delta;
      vec4 over = vec4(coloAtHere, IMG_PIXEL(inputImage, coord).a);

      alpha = blendOver2(vec2(startAlphatHere, over.a), vec2(alpha, color.a)).x;
      color = blendOver(over, color);
    }

    color.a *= alpha;

    if (shadowOnly) {
      gl_FragColor = color;
    } else {
      gl_FragColor = blendOver(color, IMG_THIS_PIXEL(inputImage));
    }
  }
}
