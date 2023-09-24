using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe qui definit tout les variable statique utilis� sur plusieurs scenes
public class Parameters
{
    // Enumeration des trois role
    public enum ROLE
    {
        PASSIF,
        DETECTIVE,
        TUEUR,
    }

    public struct Player
    {
        public ROLE ROLE { get; set; }
        public string Race { get; set; }
        public string Mission_Principal { get; set; }
        public string Mission_Secondaire { get; set; }
        public List<Objects> Inventaire { get; set; }
    }

    // Nombre de joueur -> Initialiser sur l'ecran titre
    public static int player_number = 5;

    // Les joueur, initialiser dans la Redistribution des roles
    public static List<Player> players;

    // Liste des objectif : Item1 = Description, Item2 = Titre
    // Liste 1 -> Tueur
    // Liste 2 -> Detective
    // Liste 3 -> Passif
    public static List<List<Tuple<string, string>>> objective = new List<List<Tuple<string, string>>>
    {
        new List<Tuple<string, string>> { 
            new Tuple<string, string>(
                "Vous �tes un assassin, votre objectif est de tuer un joueur et de vous �chapper sans �tre d�couvert (20pts) ",
                "Assassinat Pr�-m�diter"
            ),
        },

        new List<Tuple<string, string>> {
            new Tuple<string, string>(
                "Vous �tes un inspecteur de la police, vous avez �t� informer qu�un assassin se trouvait dans le manoir, d�busquer le pour le livrer � la justice ! (20pts)",
                "Enqu�teur professionnel"
            )
        },

        new List<Tuple<string, string>> {
            new Tuple<string, string>(
                "Vous �tes narcissique, voler 3 miroirs. (20pts)",
                "Narcissime"
            ),
            new Tuple<string, string>(
                "Vous �tes un voleur � la recherche de la celebre armure de Mr X, trouvez o� elle se cachent et quittez le manoir sans �tre d�masquer. (20pts) ",
                "Voleur experiment�"
            )
        } 
    };

    // Liste des objectifs secondaire
    public static List<string> obj_sec = new List<string>
    {
        "Vous ne vous sentez pas en s�curit� dans ce manoir, trouvez une arme. (Pts = d�g�ts de l�arme x3)",
        "Vous avez un peu froid, allumez la chemin� du grand salon (6 Pts)",
        "Vous avez peur du noir, trouvez une lampe torche o� allumer un chandelier (6 Pts)",
        "Quitte � �tre bloqu� dans cette situation, autant en profiter un peu, chercher dans le manoir pour trouver des objets de valeur � revendre (4 Pts/objets vol�s)",
        "Les livres de la biblioth�que vous intrigue, allez lire l�ouvrage le plus ancien, un myst�re se r�v�lera peut-�tre... (5 Pts par livre limite � 3)"
    };


    // TODO : Merge them as a List<Tuple<string, string>>
    // Liste des description des comp�tence
    public static List<string> competence = new List<String>
    {
        "Tu peux attaquer sans objet (plus de chance de tuer, attaque +2)",
        "Peut recevoir plus d'indices, perspicacit� d�velopp�e (fouille +2)",
        "Voler un objet al�atoire dans la maison / sur le joueur (succ�s garanti, general vol +1)",
        "Avance d'une case en plus par mouvement",
        "D�place un objet ou une personne dans la salle (chance : 2 pour un d�)",
        "Peut creuser des tunnels pour passer dans des pi�ces non reli�es (succ�s garanti)",
        "Peut se d�fendre sans objet (d�fense +2)",
        "Peut ranimer quelqu'un (x1, succ�s garanti)"
    };

    // List des noms des comp�tence
    public static List<string> competence_name = new List<string>
    {
        "Furie de sang",
        "Vision de la for�t",
        "Chippeur",
        "Grande Jambe",
        "Telekin�sie",
        "Diggy Diggy Hole",
        "Armure de lumi�re",
        "Reviens a la vie"
    };

}
