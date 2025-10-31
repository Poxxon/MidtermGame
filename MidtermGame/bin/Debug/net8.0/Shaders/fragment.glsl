#version 330 core

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct Material {
    sampler2D diffuse;
    vec3 specular;
    float shininess;
};

in vec3 vFragPos;
in vec3 vNormal;
in vec2 vUV;

out vec4 FragColor;

uniform Light uLight;
uniform Material uMaterial;
uniform vec3 uViewPos;
uniform vec3 uTint;
uniform bool uLightOn;

void main()
{
    vec3 norm = normalize(vNormal);
    vec3 texColor = texture(uMaterial.diffuse, vUV).rgb * uTint;

    vec3 ambient = uLight.ambient * texColor;
    vec3 result = ambient;

    if (uLightOn) {
        vec3 lightDir = normalize(uLight.position - vFragPos);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = uLight.diffuse * diff * texColor;

        vec3 viewDir = normalize(uViewPos - vFragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), uMaterial.shininess);
        vec3 specular = uLight.specular * spec * uMaterial.specular;

        result += diffuse + specular;
    }

    FragColor = vec4(result, 1.0);
}