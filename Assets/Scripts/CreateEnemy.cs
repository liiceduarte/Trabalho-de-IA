using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    public List<Transform> PontosDeGerarInimigos = new List<Transform>();

    public List<GameObject> Inimigos = new List<GameObject>();

    float X;
    float Y;

    public float DelayTime = 10;
    float spawnTimer = 0;

    public void EnemyGenerator()
    {
        int position = Random.Range(0, PontosDeGerarInimigos.Count);

        int inimigoEscolhido = Random.Range(0, Inimigos.Count);

        Instantiate(Inimigos[inimigoEscolhido], PontosDeGerarInimigos[position].transform.position, Quaternion.identity);
        
    }

    void Start()
    {
        EnemyGenerator();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if(spawnTimer >= DelayTime)
        {
            EnemyGenerator();
            spawnTimer = 0;
        }
    }
}
