using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class RedistributionBehaviour : MonoBehaviour
{
    // Hardcode des trois couleurs choisis pour les 3 roles
    // Unity a comme format une valeur entre 0 et 1
    // Donc je converti l'HEX en Color avec une division par 0xFF
    // (Comme une division par 100 en base 10, sauf que la, la valeur max est 0xFF)
    Color color_assassin = new Color(
        ((float)0x91 / 0xFF),
        ((float)0x02 / 0xFF),
        ((float)0x02 / 0xFF)
    );
    Color color_detective = new Color(
        0x00,
        ((float)0x19 / 0xFF),
        ((float)0x6B / 0xFF)
    );
    Color color_passif = new Color(
        0x00,
        ((float)0x6B / 0xFF),
        0x00
    );

    // GameObjects necessaire pour ce script

    // -------------------------------------------------------------------------------------------- //

    // Canva principal
    public GameObject Canvas;
    public GameObject Role;         // Le texte : Tu es ROLE
    public GameObject Card;         // Le sprite de la carte
    public GameObject ButtonInfo;   // Le bouton ? pour afficher le panel

    // Le panel d'information
    public GameObject Panel;
    public GameObject Mission;      // Le texte Mission : Lorem Ipsum
                                    // Contient comme enfant la description de la mission

    public GameObject Competence;   // Le texte Competence : Lorem Ipsum
                                    // Contient comme enfant la description de la mission

    public GameObject MissionSec;   // Le texte description de la mission secondaire

    // -------------------------------------------------------------------------------------------- //

    public GameObject Canvas2;      // Le deuxieme canvas : "JOUEUR SUIVANT, REGARDE TA CARTE"

    // -------------------------------------------------------------------------------------------- //

    // Liste de tout les sprites de cartes personnage
    public List<Sprite> card_sprite;

    // Velocité de mouvement de la panel d'information - aussi utilisé comme flag (0 = false, anything else = true)
    int gliding = 0;

    // Le joueur actuel, sert d'index pour enregistrer et utiliser les info
    int currentPlayer = 0;

    // My own random Object (See Random.cs for implementation)
    // Unification des algorithme de random pour pouvoir avoir quelque chose de plus uniforme
    // (Source : mes test d'algorithme)
    //
    // Clé = 6 car j'ai fais des test et c'etait la meilleure clé pour avoir un resultat uniforme
    //
    Random rand;

    // Appeler a la creation de l'objet
    void Start()
    {
        // Dis a unity de ne pas attendre de V-Sync
        // Et limite les framerate a 60 FPS
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        rand = new Random((UInt64)Time.frameCount, 6);

        // Initialise la liste des joueurs
        Parameters.players = new List<Parameters.Player>();

        // Liste des races, generér a partir du nom des cartes
        List<String> races_list = new List<string>();
        foreach (Sprite spr in card_sprite) races_list.Add(spr.name);

        List<int> modifiedRole = new List<int>();

        races_list.Remove("test");


        for (int i = 0; i < Parameters.player_number; i++)
        {
            string race = "test";
            if (!(races_list.Count == 0))
            {
                int index = rand.NextInt() % races_list.Count;
                race = races_list[index];
                races_list.RemoveAt(index);
            }

            // Ajouter la structure a la liste des joueurs
            Parameters.players.Add(

                // Initialisation de la structure avec les valeur par defaut
                new Parameters.Player
                {
                    ROLE = Parameters.ROLE.PASSIF,                                                                          // Tout le monde commence passif et le script donne ensuite de nouveau role
                    Race = race,                                                                                            // La race est calculé juste au dessus
                    Mission_Principal = Parameters.objective[2][rand.NextInt() % Parameters.objective[2].Count].Item2,      // On recupere la liste Passif (index 2), puis on prends une mission random dedans, puis on prends le 2eme item, le nom                                                          
                    Mission_Secondaire = Parameters.obj_sec[rand.NextInt() % Parameters.obj_sec.Count],                     // Mission secondaire random dans la liste 
                    Inventaire = new List<Objects>()                                                                        // Liste vide pour l'inventaire
                }

            );
        }

        // Definition des roles non passif
        int killer_index = rand.NextInt() % Parameters.player_number;

        // Recupere la structure du joueur selectionné au hasard
        var killer = Parameters.players[killer_index];

        // Change les valeurs Role et mission principal (qui sont en soit lié)
        killer.ROLE = Parameters.ROLE.TUEUR;
        killer.Mission_Principal = Parameters.objective[0][0].Item2;

        // Remets le Struct modifié dans la liste
        Parameters.players[killer_index] = killer;

        // Ajoute l'indice du tueur a la liste des indice auquel il ne faut plus toucher
        modifiedRole.Add(killer_index);

        // Si il y a moins de 7 joueur, faire la boucle 2 fois, sinon la faire 3 fois
        for (int i = 0; i < (Parameters.player_number >= 7 ? 3 : 2); i++)
        {

            // Genere un nombre random
            int j = rand.NextInt() % Parameters.player_number;

            // Si elle est dans la liste des indice auquel il ne faut plus toucher (IndexOf retourne -1 si l'element n'est pas dans la liste)
            if (modifiedRole.IndexOf(j) != -1)
            {
                // Decrementer i et refaire un tour de boucle (en gros dire a la boucle que cette etape n'a jamais exister)
                i--;
                continue;
            }

            // Recupere la structure du joueur selectionné au hasard
            var obj = Parameters.players[j];

            // Change les valeurs Role et mission principal (qui sont en soit lié)
            obj.ROLE = Parameters.ROLE.DETECTIVE;
            obj.Mission_Principal = Parameters.objective[1][0].Item2;

            // Remets le Struct modifié dans la liste
            Parameters.players[j] = obj;

            // Ajoute l'indice du tueur a la liste des indice auquel il ne faut plus toucher
            modifiedRole.Add(j);
        }

        // Execute seulement dans l'editeur et pas dans les build : Passe l'ecran pour montrer les cartes
#if UNITY_EDITOR
        SceneManager.LoadScene("GameScene");
#endif

        DrawPlayerCard(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Gere l'animation des Panel

        // Si le panel est completement sur l'ecran (que l'animation dois se finir), et que gliding est en dessous de 0 (qu'il glisse encore vers la gauche, du dehors de l'ecran a dedans)
        if (Panel.transform.position.x <= 540 * (transform.localScale.x) && gliding < 0)
        {
            // Snap le panel au bonne endroit exactement (pour etre sur qu'il ne se decale un peu pour des raisons X ou Y)
            Panel.transform.position = new Vector3(540 * (transform.localScale.x) * 0.9f, Panel.transform.position.y, Panel.transform.position.z);

            // Et remets gliding a 0
            gliding = 0;
        }

        // Si le panel est completement hors de l'ecran (que l'animation dois se finir), et que gliding est au dessus de 0 (qu'il glisse encore vers la droite, du dedans de l'ecran a dehors)
        else if (Panel.transform.position.x >= 1840 * 0.9f && gliding > 0)
        {
            // Snap le panel au bonne endroit exactement (pour etre sur qu'il ne se decale un peu pour des raisons X ou Y)
            Panel.transform.position = new Vector3(1840 * 0.9f, Panel.transform.position.y, Panel.transform.position.z);

            // Et remets gliding a 0
            gliding = 0;
        }

        // Deplace le panel de "gliding" pixel
        // (soit -30 si il arrive dans l'ecran, soit 0 si il ne bouge pas, soit 30 si il sort de l'ecran)
        Panel.transform.position += new Vector3(gliding, 0, 0);
    }

    void DrawPlayerCard(int index)
    {
        // Recupere la couleur avec des operation ternary
        //
        // Traduction en if/else :
        // 
        // if(Parameters.players[index].ROLE == Parameters.ROLE.TUEUR) {                 // Parameters.players[index].ROLE == Parameters.ROLE.TUEUR ?
        //      color = color_assassin                                                   // color_assassin
        // }
        // else {                                                                        // : (
        //    if(Parameters.players[index].ROLE == Parameters.ROLE.DETECTIVE) {          // Parameters.players[index].ROLE == Parameters.ROLE.DETECTIVE ?
        //          color = color_detective                                              // color_detective
        //    }                                                                          
        //    else color = color_passif                                                  // : color_passif
        // }                                                                             // )

        Color color = Parameters.players[index].ROLE == Parameters.ROLE.TUEUR ? color_assassin :
                (Parameters.players[index].ROLE == Parameters.ROLE.DETECTIVE ? color_detective :
                    color_passif);

        // Colore le text avec la couleur choisis precendemment
        Role.GetComponent<TextMeshProUGUI>().color = color;

        // Change le texte par TU ES, une nouvelle ligne, et le role, convertie en string, et mise entièrement en Majuscule
        Role.GetComponent<TextMeshProUGUI>().text = "TU ES\n" + Parameters.players[index].ROLE.ToString().ToUpper();

        // Change la surbrillance du texte et la couleur du role
        Role.GetComponent<TextMeshProUGUI>().fontSharedMaterial.SetColor("_GlowColor", color);

        // Change la couleur du bouton bulle
        ButtonInfo.GetComponent<Image>().color = color;

        // Recupere l'index de la race grace au nom
        int ind = -1;
        for (int i = 0; i < card_sprite.Count; i++)
        {
            if (Parameters.players[index].Race == card_sprite[i].name) ind = i;
        }

        // Recupere la competence de la race et le mets dans la boite de texte de la competence
        Competence.GetComponent<TextMeshProUGUI>().text = "Compétence: " + Parameters.competence_name[ind];
        Competence.GetComponentsInChildren<TextMeshProUGUI>()[1].text = Parameters.competence[ind];

        // Recupere le sprite avec l'indice et le mets sur la carte
        Card.GetComponent<Image>().sprite = card_sprite[ind];

        string miss = "";

        // Recupere la description de la mission
        if (Parameters.players[index].ROLE == Parameters.ROLE.TUEUR) miss = Parameters.objective[0][0].Item1;
        if (Parameters.players[index].ROLE == Parameters.ROLE.DETECTIVE) miss = Parameters.objective[1][0].Item1;
        else for (int i = 0; i < Parameters.objective[2].Count; i++)
            {
                if (Parameters.objective[2][i].Item2 == Parameters.players[index].Mission_Principal) miss = Parameters.objective[2][i].Item1;
            }

        // Mets les valeurs recupere dans les boites de texte de la missions
        Mission.GetComponent<TextMeshProUGUI>().text = "Mission: " + Parameters.players[index].Mission_Principal;
        Mission.GetComponentsInChildren<TextMeshProUGUI>()[1].text = miss;

        // Recupere la mission secondaire et le met dans la boite faites pour
        MissionSec.GetComponentInChildren<TextMeshProUGUI>().text = Parameters.players[index].Mission_Secondaire;

    }

    public void GlideDisplay()
    {
        // Change la velocité du panel pour le faire venir
        gliding = -30;
    }

    public void MakeDisplayDisappear()
    {
        // Change la velocité du panel pour le faire partir
        gliding = 30;
    }

    public void Transition()
    {
        // Cache le canvas de "PROCHAIN JOUEUR" et affiche le canvas principal
        Canvas.GetComponent<CanvasGroup>().alpha = 1;
        Canvas2.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ReTransition()
    {
        // // Cache le canvas principal et affiche le canvas de "PROCHAIN JOUEUR"
        Canvas.GetComponent<CanvasGroup>().alpha = 0;
        Canvas2.GetComponent<CanvasGroup>().alpha = 1;

        // Si c'etaits le dernier joueur
        if (currentPlayer == Parameters.player_number - 1)
        {
            // Change de scene
            Canvas2.GetComponent<CanvasGroup>().alpha = 0;
            SceneManager.LoadScene("GameScene");

        }

        // Incremente le compteur de joueur
        currentPlayer++;

        // Affiche la carte du joueur en arriere plan
        DrawPlayerCard(currentPlayer);
    }
}
