using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

// Les 4 actions et NULL dans une enumeration
public enum ActionType
{
    NULL,
    ATTAQUE,
    DEFENSE,
    FOUILLE,
    VOL
}

// Structure d'un objet, qui contient un nom, le sprite de l'objet, l'action qu'il modifie et le modifier
public struct Objects
{
    public string name;
    public Sprite sprite;

    public ActionType affect;
    public int modifier;

    public Objects(string name, Sprite sprite, ActionType affect, int modifier)
    {
        this.name = name;
        this.sprite = sprite;
        this.affect = affect;
        this.modifier = modifier;
    }
}

// Structure d'une piece, qui a un nom et une liste d'objets trouvable
public struct Rooms
{
    public string name;
    public List<Objects> items_list;

    public Rooms(string name, List<Objects> items_list)
    {
        this.name = name;
        this.items_list = items_list;
    }
}

public class RollController : MonoBehaviour
{
    public int modifier;
    public int base_succes;
    public int dice_number;

    public GameObject diceOne;
    public GameObject diceTwo;

    public GameObject InventoryCanva;
    public GameObject PanelCanva;
    public GameObject MainCanva;
    public GameObject AddCanva;

    public List<GameObject> PlayerButtonList;
    public int current_player;

    public List<Sprite> objects_card;
    public List<Sprite> events_card;

    public List<Sprite> sprites;

    public GameObject Competence;
    public GameObject Mission;
    public GameObject MissionSec;

    public GameObject self_GO;

    public GameObject turnText;

    public GameObject CardPlayerText;
    public GameObject CardEtageText;
    public GameObject CardRoomText;
    public GameObject ModifierText;

    public GameObject add_GO;

    public GameObject bgImage;

    List<Objects> objects;

    List<Rooms> etage_0;
    List<Rooms> etage_1;
    List<Rooms> etage_2;
    List<Rooms> etage_3;

    int gliding;
    int player = 0;

    int etage = 1;
    int index_piece = -1;

    UnityEngine.Vector3 defaultpos;

    int tour = 1;
    // Start is called before the first frame update
    void Start()
    {
        // Dis a unity de ne pas attendre de V-Sync
        // Et limite les framerate a 60 FPS
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // Initialise tout les Listes
        objects = new List<Objects>();
        etage_0 = new List<Rooms>();
        etage_1 = new List<Rooms>();
        etage_2 = new List<Rooms>();
        etage_3 = new List<Rooms>();

        // Liste des objets et leurs stats (qui sont pour l'instant inutilisé)
        // Plan pour les stats : Auto-Modifier et Selection des objets de fouille automatique
        objects.Add(new Objects("Livre", objects_card[0], ActionType.DEFENSE, 1));
        objects.Add(new Objects("Miroir", objects_card[1], ActionType.DEFENSE, 2));
        objects.Add(new Objects("Armure", objects_card[2], ActionType.DEFENSE, 3));
        objects.Add(new Objects("Bouclier", objects_card[3], ActionType.DEFENSE, 3));
        objects.Add(new Objects("Couteau", objects_card[4], ActionType.ATTAQUE, 2));
        objects.Add(new Objects("Marteau", objects_card[5], ActionType.ATTAQUE, 1));
        objects.Add(new Objects("Epée", objects_card[6], ActionType.ATTAQUE, 3));
        objects.Add(new Objects("Arc", objects_card[7], ActionType.ATTAQUE, 0));
        objects.Add(new Objects("Flèches", objects_card[8], ActionType.ATTAQUE, 1));
        objects.Add(new Objects("Lampe torche", objects_card[9], ActionType.FOUILLE, 2));
        objects.Add(new Objects("Corde", objects_card[10], ActionType.NULL, 0));            // Pas de stats, juste une action particulière

        // Script generated - Nom de la piece, et une liste des objets definis juste avant
        etage_0.Add(new Rooms("Garage", new List<Objects> { objects[5], objects[7], objects[3]}));
        etage_0.Add(new Rooms("Cave à vins", new List<Objects> { objects[8], objects[4], objects[9]}));
        etage_0.Add(new Rooms("Cave", new List<Objects> { objects[9], objects[2], objects[6]}));

        etage_1.Add(new Rooms("Cuisine", new List<Objects> { objects[0], objects[4], objects[1]}));
        etage_1.Add(new Rooms("Salle à manger", new List<Objects> { objects[4], objects[6], objects[4]}));
        etage_1.Add(new Rooms("Chambre Bleu", new List<Objects> { objects[0], objects[9], objects[1]}));
        etage_1.Add(new Rooms("Bureau", new List<Objects> { objects[10], objects[0], objects[8]}));
        etage_1.Add(new Rooms("Petit Salon", new List<Objects> { objects[1], objects[0], objects[6]}));
        etage_1.Add(new Rooms("Salon de lecture", new List<Objects> { objects[5], objects[2], objects[0]}));
        etage_1.Add(new Rooms("Grande Bibliothèque", new List<Objects> { objects[0], objects[3], objects[5]}));
        etage_1.Add(new Rooms("Bibliothèque 2", new List<Objects> { objects[0], objects[3], objects[2]}));
        etage_1.Add(new Rooms("Hall d'entrée", new List<Objects> { objects[2], objects[8], objects[1]}));
        etage_1.Add(new Rooms("Cellier", new List<Objects> { objects[10], objects[7], objects[9]}));

        etage_2.Add(new Rooms("Piano", new List<Objects> { objects[8], objects[10], objects[5]}));
        etage_2.Add(new Rooms("Salle de bal", new List<Objects> { objects[1], objects[3], objects[6]}));
        etage_2.Add(new Rooms("Chambre Verte", new List<Objects> { objects[9], objects[10], objects[4]}));
        etage_2.Add(new Rooms("Chambre Jaune", new List<Objects> { objects[9], objects[1], objects[7]}));
        etage_2.Add(new Rooms("Grande Chambre", new List<Objects> { objects[6], objects[2], objects[5]}));
        etage_2.Add(new Rooms("Salle d'eau", new List<Objects> { objects[1], objects[8], objects[1]}));
        etage_2.Add(new Rooms("Grand salon", new List<Objects> { objects[4], objects[7], objects[3]}));
        etage_2.Add(new Rooms("Balcon", new List<Objects> { objects[7], objects[10], objects[8]}));

        // Pour chaque joueur, crée un bouton avec le texte " Role : Race ", visible et ou on peux interagir avec
        // Car pour les 8 boutons, le defaut c'est : alpha = 0 et pas interactable
        for (int i = 0; i < Parameters.player_number; i++)
        {
            string play = Parameters.players[i].ROLE.ToString() + " : " + Parameters.players[i].Race;
            PlayerButtonList[i].GetComponentInChildren<TextMeshProUGUI>().text = play;
            PlayerButtonList[i].GetComponent<CanvasGroup>().alpha = 1.0f;
            PlayerButtonList[i].GetComponent<CanvasGroup>().interactable = true;
        }

        // Change la couleur du bouton du premier joueur en vert, avec la transparance que tout les boutons ont par defaut
        PlayerButtonList[0].GetComponent<Image>().color = new Color(0, 1, 0, PlayerButtonList[0].GetComponent<Image>().color.a);

        // Change de piece (car par defaut la piece est -1, c'est pour la passer a 0 et avoir les bonnes image)
        ChangePiece(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Gere l'animation des Panel

        // Si le panel est completement sur l'ecran (que l'animation dois se finir), et que gliding est en dessous de 0 (qu'il glisse encore vers la gauche, du dehors de l'ecran a dedans)
        if (PanelCanva.transform.position.x <= 430 * (transform.localScale.x) && gliding < 0)
        {
            // Snap le panel au bonne endroit exactement (pour etre sur qu'il ne se decale un peu pour des raisons X ou Y)
            PanelCanva.transform.position = new UnityEngine.Vector3(430 * (transform.localScale.x) * 0.9f, PanelCanva.transform.position.y, PanelCanva.transform.position.z);

            // Et remets gliding a 0
            gliding = 0;
        }

        // Si le panel est completement hors de l'ecran (que l'animation dois se finir), et que gliding est au dessus de 0 (qu'il glisse encore vers la droite, du dedans de l'ecran a dehors)
        else if (PanelCanva.transform.position.x >= 1840 * 0.9f && gliding > 0)
        {
            // Snap le panel au bonne endroit exactement (pour etre sur qu'il ne se decale un peu pour des raisons X ou Y)
            PanelCanva.transform.position = new UnityEngine.Vector3(1840 * 0.9f, PanelCanva.transform.position.y, PanelCanva.transform.position.z);

            // Et remets gliding a 0
            gliding = 0;
        }

        // Deplace le panel de "gliding" pixel
        // (soit -30 si il arrive dans l'ecran, soit 0 si il ne bouge pas, soit 30 si il sort de l'ecran)
        PanelCanva.transform.position += new UnityEngine.Vector3(gliding, 0, 0);
    }

    public void ShowInventory(int player)
    {
        // Stock le joueur actuelle pour savoir a qui supprimer les cartes
        this.player = player;

        // Recupere la liste des images de l'inventaire 
        var elements = InventoryCanva.GetComponentsInChildren<Image>();

        // Rends l'inventaire visible
        InventoryCanva.GetComponent<CanvasGroup>().alpha = 1.0f;
        
        // Cache le canvas principale
        MainCanva.GetComponent<CanvasGroup>().alpha = 0.0f;
        MainCanva.GetComponent<CanvasGroup>().interactable = false;


        // Pour chaque carte, les rendre transparente
        for (int i = 0;i < 4;i++)
        {
            elements[i].color = new Color(0, 0, 0, 0);
        }

        // Pour chaque carte dans l'inventaire
        for(int i = 0; i < Parameters.players[player].Inventaire.Count; i++)
        {
            // Mettre le sprite dans liste des images, et le rendre apparant
            elements[i].sprite = Parameters.players[player].Inventaire[i].sprite;
            elements[i].color = new Color(1, 1, 1, 1);
        }


        // Meme maniere de faire le panel que sur RedistributionBehaviour.cs
        // ------------------------------------------------------------------------------------------------------------ //

        int ind = -1;
        for (int i = 0; i < sprites.Count; i++)
        {
            if (Parameters.players[player].Race == sprites[i].name) ind = i;
        }

        Competence.GetComponent<TextMeshProUGUI>().text = "Competence: " + Parameters.competence_name[ind];
        Competence.GetComponentsInChildren<TextMeshProUGUI>()[1].text = Parameters.competence[ind];
        string miss = "";

        if (Parameters.players[player].ROLE == Parameters.ROLE.TUEUR) miss = Parameters.objective[0][0].Item1;
        if (Parameters.players[player].ROLE == Parameters.ROLE.DETECTIVE) miss = Parameters.objective[1][0].Item1;
        else for (int i = 0; i < Parameters.objective[2].Count; i++)
            {
                if (Parameters.objective[2][i].Item2 == Parameters.players[player].Mission_Principal) miss = Parameters.objective[2][i].Item1;
            }

        Mission.GetComponent<TextMeshProUGUI>().text = "Mission: " + Parameters.players[player].Mission_Principal;
        Mission.GetComponentsInChildren<TextMeshProUGUI>()[1].text = miss;

        MissionSec.GetComponentInChildren<TextMeshProUGUI>().text = Parameters.players[player].Mission_Secondaire;

        // ------------------------------------------------------------------------------------------------------------ //

        // Bouge le canva principal pour que les hitbox des boutons ne pose pas de problemes.
        defaultpos = self_GO.GetComponent<RectTransform>().localPosition;
        self_GO.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(1200, 0, 0);

        // Rends les element du canvas inventaire cliquable
        InventoryCanva.GetComponent<CanvasGroup>().interactable = true;
    }

    public void HideInventory()
    {
        // Cache l'inventaire et le rends non cliquable
        InventoryCanva.GetComponent<CanvasGroup>().alpha = 0.0f;
        InventoryCanva.GetComponent<CanvasGroup>().interactable = false;

        // Remets le main canvas a l'endroid initial
        self_GO.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(0, -82, 0);

        // Rends le canva principal visible et cliquable
        MainCanva.GetComponent<CanvasGroup>().alpha = 1.0f;
        MainCanva.GetComponent<CanvasGroup>().interactable = true;
    }

    public void NextPlayer()
    {
        // Rends le bouton du joueur actuelle blanc
        PlayerButtonList[current_player].GetComponent<Image>().color = new Color(1, 1, 1, PlayerButtonList[current_player].GetComponent<Image>().color.a);

        if(current_player == Parameters.player_number - 1)
        {
            // Incremente le nombre de tour
            tour++;

            // Update le texte qui affiche le tour actuelle
            turnText.GetComponent<TextMeshProUGUI>().text = "Tour " + tour.ToString() + "/24";
        }

        // Si le joueur actuel est egale au nombre de joueur, c'est le tour de J1, sinon, c'est le tour de J(x+1)
        current_player = (current_player != Parameters.player_number - 1) ? current_player + 1 : 0;

        // Mettre le joueur actuelle en vert
        PlayerButtonList[current_player].GetComponent<Image>().color = new Color(0, 1, 0, PlayerButtonList[current_player].GetComponent<Image>().color.a);

        // Si on as fini le 24eme tour, aller a l'ending
        if (tour == 25)
        {
            SceneManager.LoadScene("Ending");
        }
    }

    public void AddCardToPlayer(int card)
    {
        // Recupere le numero du joueur dans le texte
        int p = int.Parse(CardPlayerText.GetComponent<TextMeshProUGUI>().text) - 1;

        // Si le joueur actuelle a 4 item dans son inventaire, ne pas lui donner, et retourner de la fonction maintenant
        if (Parameters.players[p].Inventaire.Count >= 4)
        {
            return;
        }

        // Ajouter l'objet a l'inventaire de joueur
        Parameters.players[p].Inventaire.Add(objects[card]);

        // En fonction de l'etage, enleve l'objet de la liste des objet de la piece
        switch (etage)
        {
            case 0:
                etage_0[index_piece].items_list.Remove(objects[card]);
                break;
            case 1:
                etage_1[index_piece].items_list.Remove(objects[card]);
                break;
            case 2:
                etage_2[index_piece].items_list.Remove(objects[card]);
                break;
        }

        // Puis refait un tour de changement de piece pour réactualiser la liste des objets de la piece
        index_piece -= 1;
        ChangePiece(true);
    }

    public void deleteCard(int card)
    {
        // Retire la carte et mets a jour l'affichage
        Parameters.players[player].Inventaire.RemoveAt(card);

        ShowInventory(player);
    }
 
    public void ChangePlayer(bool plus)
    {
        // Recupere le numero du joueur dans le texte
        int p = (int.Parse(CardPlayerText.GetComponent<TextMeshProUGUI>().text));

        // Si on veux additionner
        if (plus)
        {
            p += 1;
            if (p > Parameters.player_number) p = 0;
        }

        // Sinon
        else
        {
            p -= 1;
            if (p <= 0)  p = Parameters.player_number;
        }

        // Remettre le texte dans le TMP
        CardPlayerText.GetComponent<TextMeshProUGUI>().text = p.ToString();
    }

    // Cette fonction est si... ouaaaah qu'elle en deviens lethal a lire
    public void ChangePiece(bool plus)
    {
        // Alors, cette fonction...
        // Bonne chance

        // crée un nouvel objet Rooms, vide
        Rooms room = new Rooms();

        // Recupere l'index de la prochaine room, par etage
        switch (etage)
        {
            case 0:
                // Si plus, si l'indice piece est deja au max -> 0, sinon l'incrementé
                if(plus) index_piece = (index_piece == (etage_0.Count - 1)) ? 0 : index_piece + 1;

                // Sinon, si l'indice piece est deja a 0, la mettre au max, sinon la décrémenter
                else index_piece = index_piece == 0 ? etage_0.Count - 1 : index_piece - 1;

                // le nouvel objet deviens la piece de l'etage a l'indice calculé au dessus
                room = etage_0[index_piece];
                break;
            case 1:
                // Si plus, si l'indice piece est deja au max -> 0, sinon l'incrementé
                if (plus) index_piece = index_piece == (etage_1.Count - 1) ? 0 : index_piece + 1;

                // Sinon, si l'indice piece est deja a 0, la mettre au max, sinon la décrémenter
                else index_piece = index_piece == 0 ? etage_1.Count - 1 : index_piece - 1;

                // le nouvel objet deviens la piece de l'etage a l'indice calculé au dessus
                room = etage_1[index_piece];
                break;
            case 2:
                // Si plus, si l'indice piece est deja au max -> 0, sinon l'incrementé
                if (plus) index_piece = index_piece == (etage_2.Count - 1) ? 0 : index_piece + 1;
                
                // Sinon, si l'indice piece est deja a 0, la mettre au max, sinon la décrémenter
                else index_piece = index_piece == 0 ? etage_2.Count - 1 : index_piece - 1;

                // le nouvel objet deviens la piece de l'etage a l'indice calculé au dessus
                room = etage_2[index_piece];
                break;

        }

        // Pour chaque objets 
        for(int i = 0; i < objects.Count; i++)
        {
            // Recupere l'enfant de l'ecran d'ajout d'objet, et rends le bouton de celui ci inactif
            add_GO.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        // Pour chaque item dans la piece
        foreach (var item in room.items_list)
        {
            // Recupere l'enfant a la position "indice de item" de l'ecran d'ajout d'objet, et rends le bouton de celui ci actif
            add_GO.transform.GetChild(objects.IndexOf(item)).gameObject.GetComponent<Button>().interactable = true;
        }

        // Mettre le nom de la piece dans la zone de texte
        CardRoomText.GetComponent<TextMeshProUGUI>().text = room.name;
    }

    public void ChangeEtage(bool plus){
        // Recupere le numero de l'etage dans le texte
        int p = (int.Parse(CardEtageText.GetComponent<TextMeshProUGUI>().text));

        // Si on veux additionner
        if (plus)
        {
            p += 1;
            if (p > 2) p = 0;
        }

        // Sinon
        else
        {
            p -= 1;
            if (p < 0) p = 2;
        }

        // La variable etage deviens la valeur de p
        etage = p;

        // Le texte de l'etage deviens le numéro de l'étage
        CardEtageText.GetComponent<TextMeshProUGUI>().text = p.ToString();

        // L'indice de la piece est remise a -1 puis ré-initialiser pour avoir la premiere piece du deuxieme etage
        index_piece = -1;
        ChangePiece(true);
    }

    public void changeModifier(bool plus)
    {
        // Recupere le numero du modifier dans le texte
        int p = (int.Parse(ModifierText.GetComponent<TextMeshProUGUI>().text));

        // Si on veux additionner
        if (plus)
        {
            p += 1;
            if (p > 5) p = -5;
        }

        // Sinon
        else
        {
            p -= 1;
            if (p < -5) p = 5;
        }

        // Mets la variable dans le texte de modifier
        ModifierText.GetComponent<TextMeshProUGUI>().text = p.ToString();
    }

    // Je vais arreter de commenter ce genre de fonction car c'est le meme type de code qu'avant, changement d'opacité, et deplacement d'objet
    public void showAddCard()
    {
        AddCanva.GetComponent<CanvasGroup>().alpha = 1.0f;

        MainCanva.GetComponent<CanvasGroup>().alpha = 0.0f;
        MainCanva.GetComponent<CanvasGroup>().interactable = false;

        defaultpos = self_GO.GetComponent<RectTransform>().localPosition;
        self_GO.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(1200, 0, 0);
        add_GO.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(0, add_GO.GetComponent<RectTransform>().localPosition.y, add_GO.GetComponent<RectTransform>().localPosition.z);

        new WaitForSeconds(0.2f);
        AddCanva.GetComponent<CanvasGroup>().interactable = true;
    }

    public void hideAddCard() 
    {
        AddCanva.GetComponent<CanvasGroup>().alpha = 0.0f;
        AddCanva.GetComponent<CanvasGroup>().interactable = false;

        self_GO.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(0, -82, 0);
        add_GO.GetComponent<RectTransform>().localPosition = new UnityEngine.Vector3(1200, add_GO.GetComponent<RectTransform>().localPosition.y, add_GO.GetComponent<RectTransform>().localPosition.z);

        MainCanva.GetComponent<CanvasGroup>().alpha = 1.0f;
        MainCanva.GetComponent<CanvasGroup>().interactable = true;
    }

    public void LaunchMovDice()
    {
        // Debut la routine parallele de lancement des dé de mouvement
        // Les Coroutine on acces au fonction pour attendre
        StartCoroutine(LaunchMovementDice());
    }


    public IEnumerator LaunchMovementDice()
    {
        // Cache le canvas principale
        MainCanva.GetComponent<CanvasGroup>().interactable = false;
        MainCanva.GetComponent<CanvasGroup>().alpha = 0.0f;

        // Lance les dé
        diceOne.transform.GetChild(0).GetComponent<DiceBehvaiour>().LaunchDice(false);
        diceTwo.transform.GetChild(0).GetComponent<DiceBehvaiour>().LaunchDice(false);

        // calculé a partir du nombre de frame dans les LaunchDice
        yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(1.5f);

        // Reaffiche le canvas principale
        MainCanva.GetComponent<CanvasGroup>().interactable = true;
        MainCanva.GetComponent<CanvasGroup>().alpha = 1.0f;

        yield return null;
    }

    public void LaunchDice(int minimum)
    {
        // Debut la routine parallele de lancement des dé
        // Les Coroutine on acces au fonction pour attendre
        StartCoroutine(LaunchDice_CoRoutine(minimum));
    }

    IEnumerator LaunchDice_CoRoutine(int minimum)
    {
        // Recupere le modifier
        int p = (int.Parse(ModifierText.GetComponent<TextMeshProUGUI>().text));

        // Calcule le nombre de dé en fonction du nombre minimal a faire
        int dice_number = 1;
        if (minimum >= 6)
        {
            dice_number = 2;
        }

        // Cache le canvas principale
        MainCanva.GetComponent<CanvasGroup>().alpha = 0.0f;
        MainCanva.GetComponent<CanvasGroup>().interactable = false;

        // Lance les dé
        diceOne.transform.GetChild(0).GetComponent<DiceBehvaiour>().LaunchDice(dice_number == 1);
        if (dice_number == 2)
        {
            diceTwo.transform.GetChild(0).GetComponent<DiceBehvaiour>().LaunchDice(false);
        }

        yield return new WaitForSeconds(1);
        
        // Recupere les rolls des dés
        int roll = (int)diceOne.transform.GetChild(0).GetComponent<DiceBehvaiour>().latestRoll + (dice_number == 2 ? (int)diceTwo.transform.GetChild(0).GetComponent<DiceBehvaiour>().latestRoll : 0);

        // Si le roll est superieur ou egale au minimum - le modifier
        if (roll >= minimum - (p))
        {
            // Mettre en vert
            bgImage.GetComponent<Image>().color = Color.green;
        }
        // Sinon
        else
        {
            // Mettre en rouge
            bgImage.GetComponent<Image>().color = Color.red;

        }

        yield return new WaitForSeconds(1.5f);


        // Reaffiche le canvas principale
        MainCanva.GetComponent<CanvasGroup>().interactable = true;
        MainCanva.GetComponent<CanvasGroup>().alpha = 1.0f;

        // Remets l'image de fond en blanc
        bgImage.GetComponent<Image>().color = Color.white;

        yield return null;
    }

    public void GlideDisplay()
    {
        gliding = -30;
        PanelCanva.GetComponent<CanvasGroup>().interactable = true;
    }

    public void MakeDisplayDisappear()
    {
        gliding = 30;
        PanelCanva.GetComponent<CanvasGroup>().interactable = false;
    }
}
