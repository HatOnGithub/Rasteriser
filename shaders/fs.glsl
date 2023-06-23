#version 330
 
// shader inputs
in vec4 position;              // fragment position in World Space
in vec4 normal;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates
uniform sampler2D diffuseTexture;	// texture sampler
uniform vec3 specularColor;
uniform float specularReflectiveness;
uniform float n;
uniform float diffuseReflectiveness;
uniform vec3 ambientLight;
uniform vec3 camPos;
# define nrOfLights 100
uniform vec3 lightPos[nrOfLights];
uniform vec3 lightColor[nrOfLights];



// shader output
out vec4 outputColor;


vec4 Phong(vec3 lPos, vec3 lColor)
{
    vec3 L = lPos - vec3(position);
    float attenuation = 1.0 / dot(L, L);
    vec3 diffuse = vec3(texture(diffuseTexture, uv)) * diffuseReflectiveness * max(0, dot(normalize(vec3(normal)), normalize(L)));
    vec3 specular = specularColor * specularReflectiveness * max(0, pow(dot(normalize( camPos - vec3(position)), normalize(-L - 2 * dot(-L, vec3(normal)) * vec3(normal))), n));

    return vec4((lColor * attenuation * (diffuse + specular)) + ambientLight, 1.0);
}

// fragment shader
void main()
{
    outputColor = vec4(0,0,0,0);
    for (int i = 0; i < lightPos.length; i++)
    {
        if (lightColor[i] != vec3(0,0,0))
            outputColor += Phong(lightPos[i], lightColor[i]);
    }

    outputColor = vec4(min(255, outputColor.x), min(255, outputColor.y), min(255, outputColor.z), 1.0);
}

