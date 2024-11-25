using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;

public class UIManagerGame : MonoBehaviour  {

    //Singleton
    public static UIManagerGame Instance;

    private bool _pauseOn;

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

    [SerializeField] private TMP_Text _txtScore = default;
    [SerializeField] private Image _livesDisplayImage = default;
    [SerializeField] private Sprite[] _liveSprites = default;
    [SerializeField] private GameObject _pausePanel = default;
    [SerializeField] private GameObject _boutonReprendre = default;

    private void Start() {
        _pauseOn = false;
        Time.timeScale = 1;
        ChangeLivesDisplayImage(3);
        UpdateScore(0);
    }

    private void Update() {

        
        // Permet la gestion du panneau de pause (marche/arrêt)
        if ((Input.GetButtonDown("Pause") && !_pauseOn))  {
            _pausePanel.SetActive(true);
            Time.timeScale = 0;
            _pauseOn = true;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_boutonReprendre);
        }
        else if ((Input.GetButtonDown("Pause") && _pauseOn)) {
            ResumeGame();
        }

    }

    // Méthode qui change le pointage sur le UI
    public void UpdateScore(int score)
    {
        _txtScore.text = "Pointage : " + score.ToString();
    }

    // Méthode qui permet de changer l'image des vies restantes en fonction de la vie du joueur
    public void ChangeLivesDisplayImage(int noImage) {
        if (noImage < 0) {
            noImage = 0;
        }
        _livesDisplayImage.sprite = _liveSprites[noImage];
        
        // Si le joueur n'a plus de vie on lance la séquence de fin de partie
        if (noImage == 0) {
            PlayerPrefs.SetInt("Score", GameManager.Instance.Score);
            PlayerPrefs.Save();
            StartCoroutine("FinPartie");
        }
    }

    IEnumerator FinPartie()
    {
        yield return new WaitForSeconds(2f);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex + 1);
    }

    // Méthode qui relance la partie après une pause
    public void ResumeGame() {
        _pausePanel.SetActive(false);
        Time.timeScale = 1;
        _pauseOn = false;
    }

    public void OnReprendreClick()
    {
        ResumeGame();
    }
    
    public void OnMenuClick()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuitterClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
