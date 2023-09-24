using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe qui definit tout les variable statique utilisé sur plusieurs scenes
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
                "Vous êtes un assassin, votre objectif est de tuer un joueur et de vous échapper sans être découvert (20pts) ",
                "Assassinat Pré-méditer"
            ),
        },

        new List<Tuple<string, string>> {
            new Tuple<string, string>(
                "Vous êtes un inspecteur de la police, vous avez été informer qu’un assassin se trouvait dans le manoir, débusquer le pour le livrer à la justice ! (20pts)",
                "Enquéteur professionnel"
            )
        },

        new List<Tuple<string, string>> {
            new Tuple<string, string>(
                "Vous êtes narcissique, voler 3 miroirs. (20pts)",
                "Narcissime"
            ),
            new Tuple<string, string>(
                "Vous êtes un voleur à la recherche de la celebre armure de Mr X, trouvez où elle se cachent et quittez le manoir sans être démasquer. (20pts) ",
                "Voleur experimenté"
            )
        } 
    };

    // Liste des objectifs secondaire
    public static List<string> obj_sec = new List<string>
    {
        "Vous ne vous sentez pas en sécurité dans ce manoir, trouvez une arme. (Pts = dégâts de l’arme x3)",
        "Vous avez un peu froid, allumez la cheminé du grand salon (6 Pts)",
        "Vous avez peur du noir, trouvez une lampe torche où allumer un chandelier (6 Pts)",
        "Quitte à être bloqué dans cette situation, autant en profiter un peu, chercher dans le manoir pour trouver des objets de valeur à revendre (4 Pts/objets volés)",
        "Les livres de la bibliothèque vous intrigue, allez lire l’ouvrage le plus ancien, un mystère se révèlera peut-être... (5 Pts par livre limite à 3)"
    };


    // TODO : Merge them as a List<Tuple<string, string>>
    // Liste des description des compétence
    public static List<string> competence = new List<String>
    {
        "Tu peux attaquer sans objet (plus de chance de tuer, attaque +2)",
        "Peut recevoir plus d'indices, perspicacité développée (fouille +2)",
        "Voler un objet aléatoire dans la maison / sur le joueur (succès garanti, general vol +1)",
        "Avance d'une case en plus par mouvement",
        "Déplace un objet ou une personne dans la salle (chance : 2 pour un dé)",
        "Peut creuser des tunnels pour passer dans des pièces non reliées (succès garanti)",
        "Peut se défendre sans objet (défense +2)",
        "Peut ranimer quelqu'un (x1, succès garanti)"
    };

    // List des noms des compétence
    public static List<string> competence_name = new List<string>
    {
        "Furie de sang",
        "Vision de la forêt",
        "Chippeur",
        "Grande Jambe",
        "Telekinésie",
        "Diggy Diggy Hole",
        "Armure de lumière",
        "Reviens a la vie"
    };

}
