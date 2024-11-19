using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _points = 100;
    [SerializeField] private GameObject _enemyLaserPrefab = default(GameObject);
    [SerializeField] private GameObject _explosionPrefab = default(GameObject);

    private float _cadenceTirEnnemi;
    private float _delaiTirEnnemi;

    private void Start()
    {
        // Si l'ennemi a une attaque on instancie le premier tir entre 0.5 à 1 sesonde
        _delaiTirEnnemi = Random.Range(0.5f, 1f);
    }

    void Update()
    {
        //Déplace l'ennemi vers le bas et s'il sort de l'écran le replace en
        //haut de la scène à une position aléatoire en X
        DeplacementEnnemi();

        //Méthode qui gère le tir pour l'attaque de l'ennemi
        TirEnnemi();
    }

    private void DeplacementEnnemi()
    {
        transform.Translate(Vector3.down * Time.deltaTime * GameManager.Instance.VitesseEnnemi);
        if (transform.position.y <= -5f)
        {
            float randomX = Random.Range(-8.17f, 8.17f);
            transform.position = new Vector3(randomX, 8f, 0f);
        }
    }

    private void TirEnnemi()
    {
        // Si le pointage est plus de 500 ont instancie une attaque pour l'ennemi
        if (GameManager.Instance.Score > 500)
        {
            if (Time.time > _delaiTirEnnemi)
            {
                Instantiate(_enemyLaserPrefab, transform.position + new Vector3(0f, -0.9f, 0f), Quaternion.identity);
                // On choisit le temps de la prochaine attaque entre 3 et 10 secondes
                _cadenceTirEnnemi = Random.Range(3f, 10f);
                _delaiTirEnnemi = Time.time + _cadenceTirEnnemi;
            }
        }
    }

    // Gère les collisions entre les ennemis et les lasers/joueur
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si la collision survient avec le joueur
        if (other.tag == "Player")
        {
            //Récupérer la classe Player afin d'accéder aux méthodes publiques
            Player player = other.transform.GetComponent<Player>();
            player.Degats();  // Appeler la méthode dégats du joueur
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject); // Détruire l'objet ennemi
        }
        // Si la collision se produit avec un laser
        else if(other.tag == "Laser")
        {
            // Détruit le laser
            Destroy(other.gameObject);
            // Vérifie si l'ennemi a un shield il enlève le shield
            // Sinon il détruit l'ennemi
            if(transform.childCount > 0)
            {
                GameObject shield = transform.GetChild(0).gameObject;
                Destroy(shield);
            }
            else
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject); // Détruire l'objet ennemi
                GameManager.Instance.AjouterScore(_points); // Appelle la méthode dans la classe UIManger pour augmenter le pointage
            }
        }
    }
}
