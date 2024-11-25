using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HighScoreTable : MonoBehaviour
{
    [Header("Attibuts Panneau Principal")]
    [SerializeField] private GameObject _panneauPrincipal = default;
    [SerializeField] private GameObject _RetourDepart = default;

    [Header("Attributs Panneau Saisie Nom")]
    [SerializeField] private GameObject _panneauSaisieNom = default;
    [SerializeField] private TMP_Text _textNomPanneauSaisie = default;
    [SerializeField] private GameObject _ErreurSaisieNom = default;
    [SerializeField] private Button _buttonEnregistrerSaisieNom = default;
    [SerializeField] private Button _buttonAnnulerSaisieNom = default;
    [SerializeField] private GameObject _lettreDepartSaisieNom = default;

    [Header("Attributs Panneau Reset Scores")]
    [SerializeField] private GameObject _PanneauResetScore = default;
    [SerializeField] private Button _buttonEntrerPanneauReset = default;
    [SerializeField] private Button _buttonAnnulerPanneauReset = default;
    [SerializeField] private GameObject _ReussiResetScore = default;
    [SerializeField] private TMP_Text _textMotPassePanneauReset = default;
    [SerializeField] private GameObject _ErreurPassResetScore = default;
    [SerializeField] private GameObject _lettreDepartResetScore = default;

    // Attibuts pour la création visuelle du tableau
    private Transform _entryContainer;
    private Transform _entryTemplate;
    private List<Transform> _highScoreEntryTransformList;
    
    //Attribut de type HighScores cette classe est définie au bas de ce script
    private HighScores highScores;
    
    //Attribut utiliser lors de la saisies des lettres/mot de passe et de la véfification
    private string _texteTemp = "";
    private string _textePass = "";
    private string _texteEtoiles = "";

    private void Awake()
    {
        //PlayerPrefs.DeleteKey("highScoreTable"); // Enelever commentaire si l'on désire effacer les scores
        // Permet d'afficher la table des highscore
        GenererTableHighScore();
        
        //Vérifie si on est sur la scène de fin afin de gérer si on ajoute ou non les score obtenu
        //à la liste des meilleurs scores
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            //Si il n'y pas pas déjà 10 scores enregistré on saisie le nom et le score
            //Sinon on passe à la vérification suivante
            if (highScores._highScoreEntryList.Count >= 10)
            {
                // Si le score obtenu est plus haut que le 10e score de la liste on saisie le nom et score
                // Sinon on affiche pas le panneau de saisie
                if (PlayerPrefs.GetInt("Score") > highScores._highScoreEntryList[9].score)
                {
                    AffichePanneaudeSaisieNom();
                }
                else
                {
                    //S'assurer de sélection le bouton de retour sur le panneau principal
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(_RetourDepart);
                }
            }
            else
            {
                AffichePanneaudeSaisieNom();
            }
        }
    }

    private void Start()
    {
        // Sécurité si je suis sur la scène de fin du jeu après un certain délai
        // On retourne au menu d'accueil du jeu
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            StartCoroutine(RetourDebut());
        }
    }

    private void AffichePanneaudeSaisieNom()
    {
        _panneauSaisieNom.SetActive(true);  // Affiche le panneau de saisie
        _panneauPrincipal.SetActive(false); // Cache le panneau principal

        // Utiliser une coroutine pour placé la sélection sur la lettre
        // Car si on arrive d'un changement de scène le panneau n.a pas le temps de charger
        StartCoroutine(SetButtonWithDelay());
        
        Button btn = _buttonEnregistrerSaisieNom.GetComponent<Button>();
        btn.onClick.AddListener(EnregistrerNom);  // Ajoute action sur le bouton enregistrer nom

        Button btnAnnuler = _buttonAnnulerSaisieNom.GetComponent<Button>();
        btnAnnuler.onClick.AddListener(Annuler);

        //Cette coroutine est une sécurité si aucun nom est saisie dans le délai on reviens
        //au panneau principal
        StartCoroutine(DelaiSaisie());
    }

    IEnumerator SetButtonWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_lettreDepartSaisieNom);
    }
    
    //Coroutine qui attend un certain délai et si aucun nom est saisie
    //Réaffiche le panneau principal
    IEnumerator DelaiSaisie()
    {
        yield return new WaitForSeconds(60f);
        Annuler();  // Annule la saisie du nom
    }

    //Coroutine qui attend un certain délai et replace le jeu au menu de départ
    IEnumerator RetourDebut()
    {
        yield return new WaitForSeconds(300.0f);
        SceneManager.LoadScene(0);
    }

    // Méthode qui permet de générer la table des highScores
    private void GenererTableHighScore()
    {
        //Trouve sur la scène le conteneur et le place dans la variable
        _entryContainer = transform.Find("HighScoreEntryContainer");
        //Même chose pour le template qui contient une ligne type
        _entryTemplate = _entryContainer.Find("HighScoreEntryTemplate");
        //Cache la ligne template qui sert seulement de repère visuel
        _entryTemplate.gameObject.SetActive(false);

        // Utilise seulement pour les tests on enlève les commentaires pour générer
        // manuellement 10 entrées dans la table des highscores
        //AddHighScoreEntry(14500, "Dave");
        //AddHighScoreEntry(3400, "Alex");
        //AddHighScoreEntry(700, "Josée");
        //AddHighScoreEntry(5500, "Maxime");
        //AddHighScoreEntry(7800, "David");
        //AddHighScoreEntry(1800, "Shany");
        //AddHighScoreEntry(100, "François");
        //AddHighScoreEntry(2800, "Fabrice");
        //AddHighScoreEntry(5400, "Jonathan");
        //AddHighScoreEntry(5400, "Line");

        // Récupère la liste des highscores dans une liste à partir du PlayerPrefs
        // Cette liste est stocké à l'aide de JSON sous forme de chaine de caractère
        string jsonString = PlayerPrefs.GetString("highScoreTable");

        //Transforme à l'aide de JSON la chaine de caractère en objet de type HighScoreTable
        highScores = JsonUtility.FromJson<HighScores>(jsonString);

        //Pour la première partie sur un ordinateur donné comme le playerPerfs n'existe pas
        //On ajoute l'entrée CEGEPTR avec un pointage de 100 comme première entrée
        if (highScores == null)
        {
            AddHighScoreEntry(100, "CEGEPTR");
        }

        // Après avoir récupéré la liste on trie(ordonner la liste des highscores)
        // Du plus grand au plus petit
        for (int i = 0; i < highScores._highScoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highScores._highScoreEntryList.Count; j++)
            {
                if (highScores._highScoreEntryList[j].score > highScores._highScoreEntryList[i].score)
                {
                    //Si le score suivant est plus grand que celui en position actuelle on échange la position
                    HighScoreEntry tmp = highScores._highScoreEntryList[i];
                    highScores._highScoreEntryList[i] = highScores._highScoreEntryList[j];
                    highScores._highScoreEntryList[j] = tmp;
                }
            }
        }

        // Maintenant que la liste est classé on affiche les 10 premiers résultats
        _highScoreEntryTransformList = new List<Transform>();
        // Utilise la boucle pour ajouter chaque entrée de la liste dans ma table à afficher
        int compteur = 1;
        foreach (HighScoreEntry highScoreEntry in highScores._highScoreEntryList)
        {
            // Un utilise un compteur pour limiter à 10 le nombre de score à afficher
            if (compteur <= 10)
            {
                // Appele la méthode pour générer l'affiche graphique de la ligne dans la liste
                CreateHighScoreEntryTransform(highScoreEntry, _entryContainer, _highScoreEntryTransformList);
            }
            compteur++;
        }
    }

    // Méthode qui reçoit un entrée, le container et le transform(position) de la liste
    // Elle génère ensuite l'affichage graphique de la table
    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 50f; // Hauteur visuelle de la ligne
        Transform entryTransform = Instantiate(_entryTemplate, container);  //Instancie une ligne dans le container
        //Positionne la ligne dans le container
        RectTransform entryRectTranform = entryTransform.GetComponent<RectTransform>();
        entryRectTranform.anchoredPosition = new Vector2(0f, -templateHeight * transformList.Count);
        // Un coup positionner on rends la ligne active pour qu'elle s'affiche
        entryTransform.gameObject.SetActive(true);

        // Détermine la position de la ligne dans la liste
        // Celà permet de définir le préfixe à utiliser au début de la ligne
        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
            default:
                rankString = rank + "TH"; break;
        }
        //Change le texte pour la position de la ligne avec le préfixe construit ici haut
        entryTransform.Find("TxtPos").GetComponent<Text>().text = rankString;

        //Change le texte pour le pointage
        int score = highScoreEntry.score;
        entryTransform.Find("TxtScore").GetComponent<Text>().text = score.ToString();

        //Change le texte pour le nom
        string name = highScoreEntry.name;
        entryTransform.Find("TxtName").GetComponent<Text>().text = name;

        // On change la couleur de fond en fonction de la ligne
        if (rank == 1)
        {
            //Couleur or pour le ligne 1
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(255, 210, 3, 71);
        }
        else if (rank == 2)
        {
            //Couleur argent pour la ligne 2
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(203, 201, 193, 71);
        }
        else if (rank == 3)
        {
            //Couleur bronze pour la ligne 3
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(176, 114, 26, 71);
        }
        else
        {
            //Aucune couleur (transparent) pour les autres lignes
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
        
        //Ajoute la nouvelle ligne à la liste
        transformList.Add(entryTransform);
    }

    // Méthode qui est appelé quand on clique sur le bouton enregistrer de la saisie
    // Elle valide l'entrée et l'ajoute à la liste si valide
    private void EnregistrerNom()
    {
        bool valide = false;
        string saisie = _textNomPanneauSaisie.text;
        // Valide que les caractères entrés ne sont pas seulement des espaces vides
        foreach (char c in saisie)
        {
            if (c != ' ')
            {
                valide = true;
            }
        }

        // Si le texte saisie n'est pas vide et n'est pas remplit seulement d'espace
        // On ajoute le nom à la liste
        if (!string.IsNullOrEmpty(saisie) && valide)
        {
            // Méthode qui ajoute le score obtenu et le nom saisie à la liste des HighScores
            AddHighScoreEntry(PlayerPrefs.GetInt("Score"), saisie);
            _panneauSaisieNom.SetActive(false);  // Cache le panneau de saisie
            _panneauPrincipal.SetActive(true);  // Affiche le panneau principal
            // S'assure que le bouton retour soit sélectionne sur le panneau principal
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_RetourDepart);
            _ErreurSaisieNom.SetActive(false);  //S'assure que le message d'erreur soit caché
            
            // Comme une nouvelle entrée a été ajouté on doit effacer l'ancienne table des scores
            // Et l'afficher de nouveau avec la nouvelle entrée
            foreach (Transform child in _entryContainer.transform)
            {
                if (child.name != "HighScoreEntryTemplate")
                {
                    Destroy(child.gameObject);
                }
            }
            GenererTableHighScore();
        }
        else
        {
            //Si erreur dans la saisie afficher le message d'erreur
            _ErreurSaisieNom.SetActive(true);
        }
    }

    // Méthode qui ajoute le nom saisie et le score à la liste des HighScores
    public void AddHighScoreEntry(int p_score, string p_name)
    {
        //Creer un nouvel objet HighScore Entry à partir du score et nom recu
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = p_score, name = p_name };

        //Charger les HighScores sauvegarder dans le playerprefs sous forme de texte
        string jsonString = PlayerPrefs.GetString("highScoreTable");
        //Utiliser JSON pour convertir le texte en objet HighScoreTable
        highScores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highScores == null)  // Si jamais la table est vide on créer une nouvelle liste
        {
            // Appelle le constructeur de HighScores
            highScores = new HighScores()
            {
                //Ajoute une nouvelle liste vide qui contiendra des HighScoreEntry
                _highScoreEntryList = new List<HighScoreEntry>()
            };
        }

        //Ajouter la nouvelle entrée à la liste des highScores à partir l'objet crée plus haut
        highScores._highScoreEntryList.Add(highScoreEntry);

        //On doit sauvegarder la nouvelle table dans le playerperfs
        //Comme les playerPrefs ne peuvent stocker des objet on transforme l'objet
        //en string à l'aide de JSON
        string json = JsonUtility.ToJson(highScores);
        
        //On définit la nouvelle valeur du playerPrefs et le sauvegarde
        PlayerPrefs.SetString("highScoreTable", json);
        PlayerPrefs.Save();
    }

    // Méthode appeler sur le bouton entrer du panneau reset passWord
    // Elle vérifie le mot de passe et efface les scores si c'est le bon mot de passe
    private void ValiderPass()
    {
        if (_textePass == "TOTO")
        {
            _ErreurPassResetScore.SetActive(false); // Cache le message d'erreur
            _ReussiResetScore.SetActive(true); //Affiche le mot de confirmation
            _textePass = "";   
            _texteEtoiles = "";
            _textMotPassePanneauReset.text = "";
            PlayerPrefs.DeleteKey("highScoreTable");
            GenererTableHighScore();
            SceneManager.LoadScene(0);
        }
        else
        {
            _ErreurPassResetScore.SetActive(true);  //Affiche le message d'erreur
            _ReussiResetScore.SetActive(false);
            //Vide les champs textes pour le prochain essai
            _textePass = "";
            _texteEtoiles = "";
            _textMotPassePanneauReset.text = "";
        }
    }
 
    // ===================== MÉTHODES PUBLIQUES =========================

    //Méthode qui reçoit la lettre saisie et l'ajoute à la chaine de caractère
    //Utilisé autant pour le panneau saisieNom que pour le reset des scores
    public void AjouterLettre(string lettre)
    {
        //Si on utilise les lettre du panneau de saisie
        if (_panneauSaisieNom.activeSelf)
        {
            if (lettre == "Espace")
            {
                if (_texteTemp.Length < 3)
                {
                    _texteTemp += " ";
                }
            }
            else if (lettre == "←")
            {
                //Permet d'effacer le dernier caractère
                _texteTemp = _texteTemp.Remove(_texteTemp.Length - 1);
            }
            else
            {
                //On limite à 3 le nombre de caractère admis
                if (_texteTemp.Length < 3)
                {
                    _texteTemp += lettre;
                }
            }
            //Mets à jour le champ texte en ajoutant la lettre saisie
            _textNomPanneauSaisie.text = _texteTemp;
        }
        // Si on travaille sur le panneau du reset HighScore
        else
        {
            // Ici pour éviter que le mot de passe s'affiche on conserve le bon mot
            // dans la variable _textePass mais on ajoute un * pour afficher visuellement
            // Seulement des * dans le champ mot de passe
            if (lettre == "Espace")
            {
                if (_textePass.Length < 10)
                {
                    _textePass += " ";
                    _texteEtoiles += "*";
                }
            }
            else if (lettre == "←")
            {
                _textePass = _textePass.Remove(_textePass.Length - 1);
                _texteEtoiles = _texteEtoiles.Remove(_textePass.Length - 1);
            }
            else
            {
                // On limite à 10 le nombre de caractère possible pour le mot de passe
                if (_textePass.Length < 10)
                {
                    _textePass += lettre;
                    _texteEtoiles += "*";
                }
            }
            
            //Met à jout le champ de mot de passe en ajoutant un *
            _textMotPassePanneauReset.text = _texteEtoiles;
        }
    }

    // Méthode appeler pour afficher le panneau de reset scores
    public void ResetScores()
    {
        _panneauPrincipal.SetActive(false); //Cache le panneau principal
        _PanneauResetScore.SetActive(true);  //Affiche le panneau de reset score
        // S'assure que les champs texte soient vides
        _textePass = "";
        _texteEtoiles = "";
        _textMotPassePanneauReset.text = "";
        
        //Ajoute l'action sur le bouton Entrer d'appeler la méthode de validation du pass
        Button btn = _buttonEntrerPanneauReset.GetComponent<Button>();
        btn.onClick.AddListener(ValiderPass);

        //Ajoute l'action pour annuler la saisie du mot de passe
        Button btnAnnuler = _buttonAnnulerPanneauReset.GetComponent <Button>();
        btnAnnuler.onClick.AddListener(Annuler);

        // S'assurer que la lettre Q soit sélectionner à l'apparition du panneau
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_lettreDepartResetScore);
        
        //Sécurité si aucun mot est saisie après le délai on revient au panneau principal
        StartCoroutine(DelaiSaisie());
    }

    // Méthode appelée sur le bouton annuler qui retourne au panneau principal
    public void Annuler()
    {
        _panneauSaisieNom.SetActive(false);
        _PanneauResetScore.SetActive(false);
        _panneauPrincipal.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_RetourDepart);
        _ErreurSaisieNom.SetActive(false);
    }

    // ============================= CLASSES ========================================
    
    //Classe HighScore qui contient un attribut
    //Cet Attribut est une liste de HighScoreEntry
    //La Classe HighScoreEntry est défini juste en dessous
    private class HighScores
    {
        public List<HighScoreEntry> _highScoreEntryList;
        
    }
    
    /*
     * Classe qui représente une entrée HighScore
     * Composé de 2 attributs soit le nom et le score
     */
    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }
}
