using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private string _nom = default;
    [SerializeField] private GameObject _miniExplosionPrefab = default;

    private UIManagerGame _uiManagerGame;
    private float _vitesseLaserEnnemi;

    private void Awake()
    {
        _uiManagerGame = FindObjectOfType<UIManagerGame>();
    }
    void Update()
    {
        
        if (_nom == "Player")
        {
            // Déplace le laser vers le haut
            DeplacementLaserJoueur();
        }
  
    }

    private void DeplacementLaserJoueur()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _speed);
        if (transform.position.y > 8f)
        {
            // Si le laser sort de l'écran il se détruit
            if (this.transform.parent == null)
            {
                Destroy(this.gameObject);
            }
            // Si le laser fait partie d'un conteneur il détruit le conteneur
            else
            {
                Destroy(this.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _nom != "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Degats();
            Instantiate(_miniExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
