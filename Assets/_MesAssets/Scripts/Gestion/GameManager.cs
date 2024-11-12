
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private float _vitesseEnnemi = 6.0f;
    public float VitesseEnnemi => _vitesseEnnemi;
    [SerializeField] private float _tempsApparitionEnnemis = 5f;
    public float TempsArraritionEnnemis => _tempsApparitionEnnemis;

    private int _score;
    public int Score => _score;

    private void Start()
    {
        _score = 0;
        
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }

    // Méthode qui permet l'augmentation du score
    public void AjouterScore(int points)
    {
        _score += points;
        UIManagerGame.Instance.UpdateScore(_score);
        if (_score % 1000 == 0)
        {
            _vitesseEnnemi += 2f;
        }

        if(_score % 2000 == 0)
        {
            _tempsApparitionEnnemis -= 1f;
        }
    }
}
