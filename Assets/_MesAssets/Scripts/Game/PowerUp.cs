using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerUpID = default;  // _powerUpID  0=TripleShot   1=Speed    2=Shield
    [SerializeField] private AudioClip _powerUpSound = default;


    // Déplace le powerUp vers le bas et le détruit s'il n'est pas saisi
    void Update()    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (transform.position.y <= -5.0f) {
            Destroy(this.gameObject);
        }
    }

    // Gère la collision et le déclenchement de améliorations quand le powerUp touche le joueur
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            Player player = other.GetComponent<Player>();
            Destroy(this.gameObject);
            //AudioSource.PlayClipAtPoint(_powerUpSound, Camera.main.transform.position, 0.6f);
            if (player != null) {
                switch (_powerUpID) {
                    case 0:
                        player.PowerTripleShot();
                        break;
                    case 1:
                        player.SpeedPowerUp();
                        break;
                    case 2:
                        player.ShieldPowerUp();
                        break;
                }
            }
        }
    }
}
