using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _container = default;
    [SerializeField] private GameObject[] _powerUpPrefab = default;
    [SerializeField] private GameObject[] _enemiesPrefabs = default;

    private bool _stopSpawn = false;
    
    void Start()
    {
        StartSpawning();  //Déclenche les coroutine pour le spawn des ennemis et des améliorations
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());  // Démarre la coroutine pour l'apparition des ennemis
        StartCoroutine(SpawnPUCoroutine());  // Démarre la coroutine pour l'apparition des PowerUps
    }

    // Coroutine pour l'apparition des PowerUps
    IEnumerator SpawnPUCoroutine()
    {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawn)
        {
            Vector3 positionSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
            //Choisi au hasard un powerUp faisant partie du tableau et l'instancie
            int randomPU = Random.Range(0, _powerUpPrefab.Length);
            Instantiate(_powerUpPrefab[randomPU], positionSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(10f, 15f));
        }
    }

    //Coroutine pour l'apparition des ennemis
    IEnumerator SpawnEnemies()
    {
        
        yield return new WaitForSeconds(1f); // Délai initial
        while (!_stopSpawn)
        {
            Vector3 positionSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0f);
            // Si le pointage est supérieur à 1000 on instancie au hasard un des deux ennemis
            // Sinon on instancie seulement le premier
            if(GameManager.Instance.Score > 1000)
            {
                //Choisi au hasard un enemy faisant partie du tableau et l'instancie
                int randomEnemy = Random.Range(0, _enemiesPrefabs.Length);
                GameObject newEnemy = Instantiate(_enemiesPrefabs[randomEnemy], positionSpawn, Quaternion.identity);
                newEnemy.transform.parent = _container.transform;
            }
            else
            {
                GameObject newEnemy = Instantiate(_enemiesPrefabs[0], positionSpawn, Quaternion.identity);
                newEnemy.transform.parent = _container.transform;
            }

            yield return new WaitForSeconds(Random.Range(GameManager.Instance.TempsArraritionEnnemis, 
                GameManager.Instance.TempsArraritionEnnemis + 3f));
        }

    }

    // Méthodes publiques ========================================================

    // Arrête le spawn à la mort du joueur (fin de partie)
    public void MortJoueur()
    {
        _stopSpawn = true;
    }
}
