using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseFlowfield : MonoBehaviour
{
    FastNoise fastNoise;
    [SerializeField] private Vector3Int gridSize;
    [SerializeField] private float increment;
    [SerializeField] private Vector3 offset, offsetSpeed;

    //particles
    [SerializeField] private GameObject particlePrefab;
    public int amountofParticles;
    [HideInInspector]
    public List<FlowfieldParticle> particles;
    public List<MeshRenderer> particleMeshRenderers;
    public float particleScale, particleMoveSpeed, particleRotSpeed;
    public float spawnRadius;

    bool particleSpawnVerification(Vector3 pos)
    {
        bool valid = true;
        foreach (FlowfieldParticle particle in particles) {
            if (Vector3.Distance(pos, particle.transform.position) < spawnRadius)
            {
                valid = false;
                break;
            }
        }
        return valid;
    }

    public Vector3[,,] flowFieldDirection;
    public float cellSize;

    // Start is called before the first frame update
    void Awake()
    {
        flowFieldDirection = new Vector3[gridSize.x, gridSize.y, gridSize.z];
        fastNoise = new FastNoise();
        particles = new List<FlowfieldParticle>();
        particleMeshRenderers = new List<MeshRenderer>();
        for(int i = 0; i < amountofParticles; i++)
        {
            int attempt = 0;
            while (attempt < 100)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(this.transform.position.x, this.transform.position.x + gridSize.x * cellSize),
                    Random.Range(this.transform.position.y, this.transform.position.y + gridSize.y * cellSize),
                    Random.Range(this.transform.position.z, this.transform.position.z + gridSize.z * cellSize));
                bool isValid = particleSpawnVerification(randomPos);

                if (isValid)
                {
                    GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
                    particleInstance.transform.position = randomPos;
                    particleInstance.transform.parent = this.transform;
                    particleInstance.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
                    particles.Add(particleInstance.GetComponent<FlowfieldParticle>());
                    particleMeshRenderers.Add(particleInstance.GetComponent<MeshRenderer>());
                    break;
                }
                else if (!isValid)
                {
                    attempt++;
                }
            }
        }
        Debug.Log(particles.Count);
    }

    // Update is called once per frame
    void Update()
    {
        calculateFlowfieldDirections();
        ParticleBehavior();
    }

    void calculateFlowfieldDirections()
    {
        float xOff = 0f, yOff = 0f, zOff = 0f;
        offset = new Vector3(offset.x + (offsetSpeed.x * Time.deltaTime), offset.y + (offsetSpeed.y * Time.deltaTime), offset.z + (offsetSpeed.z * Time.deltaTime));

        for (int x = 0; x < gridSize.x; x++)
        {
            yOff = 0f;
            for (int y = 0; y < gridSize.y; y++)
            {
                zOff = 0f;
                for (int z = 0; z < gridSize.z; z++)
                {
                    float noise = fastNoise.GetSimplex(xOff + offset.x, yOff + offset.y, zOff + offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    flowFieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);
                    zOff += increment;
                }
                yOff += increment;
            }
            xOff += increment;
        }
    }

    void ParticleBehavior()
    {
        int count = 0;
        foreach (FlowfieldParticle p in particles)
        {
            //X edges
            if (p.transform.position.x > this.transform.position.x + (gridSize.x  * cellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            else if(p.transform.position.x < this.transform.position.x)
            {
                p.transform.position = new Vector3(this.transform.position.x + (gridSize.x * cellSize), p.transform.position.y, p.transform.position.z);
            }
            //Y edges
            if (p.transform.position.y > this.transform.position.y + (gridSize.y * cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
            }
            else if (p.transform.position.y < this.transform.position.y)
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y + (gridSize.y * cellSize), p.transform.position.z);
            }
            //Z edges
            if (p.transform.position.z > this.transform.position.z + (gridSize.z * cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z);
            }
            else if (p.transform.position.z < this.transform.position.z)
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z + (gridSize.z * cellSize));
            }

            Vector3Int particlePos = new Vector3Int(Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x) / cellSize, 0, gridSize.x - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / cellSize, 0, gridSize.y - 1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z) / cellSize, 0, gridSize.z - 1)));
            p.ApplyRotation(flowFieldDirection[particlePos.x, particlePos.y, particlePos.z], particleRotSpeed);
            p.moveSpeed = particleMoveSpeed;
            particleScale = p.scale;
            p.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
            count++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + new Vector3((gridSize.x * cellSize) * 0.5f, (gridSize.y * cellSize) * 0.5f, (gridSize.z * cellSize) * 0.5f), 
            new Vector3(gridSize.x * cellSize, gridSize.y * cellSize, gridSize.z * cellSize));
    }

    public void randomParticles()
    {
        foreach (FlowfieldParticle p in particles)
        {
            p.audioBand = (int)Random.Range(0, 63);
        }
        GetComponent<AudioFlowfield>().reAssignMaterials();
    }

    public void setIncrement(Slider s)
    {
        increment = s.value;
    }

    public void setOffestX(Slider s)
    {
        offsetSpeed.x = s.value;
    }

    public void setOffestY(Slider s)
    {
        offsetSpeed.y = s.value;
    }

    public void setOffestZ(Slider s)
    {
        offsetSpeed.z = s.value;
    }

    public void setParticleNum(Slider s)
    {
        amountofParticles = (int) s.value;

        if (amountofParticles > particles.Count)
        {
            for (int i = particles.Count; i < amountofParticles; i++)
            {
                int attempt = 0;
                while (attempt < 100)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(this.transform.position.x, this.transform.position.x + gridSize.x * cellSize),
                        Random.Range(this.transform.position.y, this.transform.position.y + gridSize.y * cellSize),
                        Random.Range(this.transform.position.z, this.transform.position.z + gridSize.z * cellSize));
                    bool isValid = particleSpawnVerification(randomPos);

                    if (isValid)
                    {
                        GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
                        particleInstance.transform.position = randomPos;
                        particleInstance.transform.parent = this.transform;
                        particleInstance.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
                        particles.Add(particleInstance.GetComponent<FlowfieldParticle>());
                        particleMeshRenderers.Add(particleInstance.GetComponent<MeshRenderer>());
                        break;
                    }
                    else if (!isValid)
                    {
                        attempt++;
                    }
                }
            }
            randomParticles();
        }
        else
        {
            for (int i = particles.Count - 1; i > amountofParticles; i--)
            {
                FlowfieldParticle p = particles[i];
                particles.Remove(p);
                particleMeshRenderers.Remove(p.GetComponent<MeshRenderer>());
                Destroy(p.gameObject);
            }
        }

    }
}
