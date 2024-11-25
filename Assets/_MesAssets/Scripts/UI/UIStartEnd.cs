using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UIStartEnd : MonoBehaviour
{
    // Scene de fin
    [Header("Variables pour fin de partie")]
    [SerializeField] private TextMeshProUGUI _txtGameOver = default;
    [SerializeField] private TextMeshProUGUI _txtScoreFin = default;
    [SerializeField] private Button _buttonMenu = default;
    [SerializeField] private Button _buttonQuitter = default;

    [Header("Variables pour scène départ")]
    [SerializeField] private GameObject _panneauDepart = default;
    [SerializeField] private GameObject _panneauBestScores = default;
    [SerializeField] private GameObject _buttonDemarrer = default;
    [SerializeField] private GameObject _buttonRetour = default;
    [SerializeField] private TMP_Text _txtCompteur = default;

    private void Start()
    {
        //Initialise le UI si scène de départ
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_buttonDemarrer);

            //Utilise un playerPrefs pour compteur le nombre de parties joué
            //sur cet ordinateur
            if (PlayerPrefs.HasKey("Compteur"))
            {
                //Le compteur est augmenter quand le joueur clique sur Démarrer une nouvelle partie
                _txtCompteur.text = "Nombres de parties : " + PlayerPrefs.GetInt("Compteur").ToString();
            }
            else
            {
                _txtCompteur.text = "Nombres de parties : 0";
            }
        }
        
        //Initialise le UI si scène de fin
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            _buttonMenu.onClick.AddListener(OnMenuClick);
            _buttonQuitter.onClick.AddListener(OnQuitterClick);
            _txtScoreFin.text = "Votre pointage : " + GameManager.Instance.Score.ToString();
            GameOverSequence();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_buttonMenu.gameObject);
        }
    }

    public void OnQuitterClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnDemarrerClick()
    {
        //Augemte le players pour le compteur de parties
        if (PlayerPrefs.HasKey("Compteur"))
        {
            PlayerPrefs.SetInt("Compteur", PlayerPrefs.GetInt("Compteur") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("Compteur", 1);
        }

        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene(0);
    }

    public void OnBestScoresClick()
    {
        _panneauDepart.SetActive(false);
        _panneauBestScores.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_buttonRetour.gameObject);

        //Coroutine qui ramène le panneau de départ automatiquement après 60 secondes
        StartCoroutine(DelaiRetourDebut());
    }

    IEnumerator DelaiRetourDebut()
    {
        yield return new WaitForSeconds(60f);
        OnRetourClick();
    }

    public void OnRetourClick()
    {
        _panneauDepart.SetActive(true);
        _panneauBestScores.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_buttonDemarrer.gameObject);
    }

    // Méthode qui affiche la fin de la partie et lance la coroutine d'animation
    private void GameOverSequence()
    {
        _txtGameOver.gameObject.SetActive(true);
        StartCoroutine(GameOverBlinkRoutine());
    }

    IEnumerator GameOverBlinkRoutine()
    {
        while (true)
        {
            _txtGameOver.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            _txtGameOver.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.7f);
        }
    }
}
