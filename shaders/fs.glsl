#version 330
 
// shader inputs
in vec4 position;              // fragment position in World Space
in vec4 normal;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates
uniform sampler2D diffuseTexture;	// texture sampler
uniform sampler2D environmentTexture;
uniform sampler2D normalMap;
uniform int material;
uniform vec3 specularColor;
uniform float specularReflectiveness;
uniform float n;
uniform float diffuseReflectiveness;
uniform vec3 ambientLight;
uniform vec3 camPos;
# define nrOfLights 100
uniform vec3 lightPos[nrOfLights];
uniform vec3 lightColor[nrOfLights];
uniform vec3 lightDirection[nrOfLights];
uniform float lightConeAngle[nrOfLights];



// shader output
out vec4 outputColor;


vec4 Phong(vec3 lPos, vec3 lColor)
{
    vec3 L = lPos - vec3(position);
    float attenuation = 1.0 / dot(L, L);
    vec3 diffuse = vec3(texture(diffuseTexture, uv)) * diffuseReflectiveness * max(0, dot(normalize(vec3(normal)), normalize(L)));
    vec3 specular = specularColor * specularReflectiveness * max(0, pow(dot(normalize( camPos - vec3(position)), normalize(-L - 2 * dot(-L, vec3(normal)) * vec3(normal))), n));

    return vec4((lColor * attenuation * (diffuse + specular)), 1.0);
}

// fragment shader
void main()
{
    if (material == 0)
    {
            outputColor = vec4(ambientLight,0) * texture(diffuseTexture, uv);
            
            for (int i = 0; i < lightPos.length; i++) 
                if (lightColor[i] != vec3(0,0,0) )
                    outputColor += Phong(lightPos[i], lightColor[i]);
            
            outputColor = vec4(min(255, outputColor.x), min(255, outputColor.y), min(255, outputColor.z), 1.0);
    }
    if (material == 1)
    {
        outputColor = vec4(ambientLight,0) * texture(diffuseTexture, uv);

        for (int i = 0; i < lightPos.length; i++) 
            if (lightColor[i] != vec3(0,0,0) )
            {
                vec3 L = lightPos[i] - vec3(position);
                outputColor += vec4(lightColor[i] * (1.0 / dot(L, L)) * (specularColor * specularReflectiveness * max(0, pow(dot(normalize( camPos - vec3(position)), normalize(-L - 2 * dot(-L, vec3(normal)) * vec3(normal))), n))), 1.0);
            }  

        vec3 N = normalize(vec3(normal)); 
        vec3 I = normalize(vec3(position) - camPos);
        vec3 R = I - ((2 * dot(I,N)) * N);
        outputColor = texture(environmentTexture, vec2( 
        (1 - (atan(R.x, R.z) / (2 * 3.14159265359) + 0.5)),  
        (-R.y + 1) / 2));

         outputColor = vec4(min(255, outputColor.x), min(255, outputColor.y), min(255, outputColor.z), 1.0);
    }

    if (material == 2)
    {
        outputColor = vec4(ambientLight,0) * texture(diffuseTexture, uv);
            
        for (int i = 0; i < lightPos.length; i++) 
            if (lightColor[i] != vec3(0,0,0) )
                outputColor += Phong(lightPos[i], lightColor[i]);
        
        
        vec3 N = normalize(vec3(normal)); 
        vec3 I = normalize(vec3(position) - camPos);
        vec3 R = I - ((2 * dot(I,N)) * N);
        outputColor += texture(environmentTexture, vec2( 
        (1 - (atan(R.x, R.z) / (2 * 3.14159265359) + 0.5)),  
        (-R.y + 1) / 2)) * texture(diffuseTexture, uv) * specularReflectiveness;

        outputColor = vec4(min(255, outputColor.x), min(255, outputColor.y), min(255, outputColor.z), 1.0);

    }
}

