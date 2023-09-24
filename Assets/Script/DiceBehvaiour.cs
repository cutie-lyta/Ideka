using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class DiceBehvaiour : MonoBehaviour
{
    // Liste des faces -> Quaternion est un type Unity qui gere les angles
    List<Quaternion> faces = new List<Quaternion>();

    // Flag - le dé tourne t'il
    bool rolling = false;

    // Nombre de frames avant la fin de la phase actuelle
    int timer = 60;

    // Nombre, entre 0 et 5
    UInt64 number = 0;

    // Le dernier lancée enregistrer pour les autres objets, entre 1 et 6
    public UInt64 latestRoll;

    // Flag - le dé est il centrer ou un peu decalé (1 ou 2 dé sur le terrain ?)
    bool center;

    // Custom random object que j'ai crée (Random.cs)
    Random rand;

    // Start is called before the first frame update
    void Start()
    {
        // Tout les faces
        faces.Add(Quaternion.Euler(0.0f, 90.0f, 180.0f));    // 1
        faces.Add(Quaternion.Euler(0.0f, 180.0f, 180.0f));   // 2
        faces.Add(Quaternion.Euler(270.0f, 0.0f, 0.0f));     // 3
        faces.Add(Quaternion.Euler(270.0f, 0.0f, 90.0f));    // 4
        faces.Add(Quaternion.Euler(270.0f, 180.0f, 0.0f));   // 5
        faces.Add(Quaternion.Euler(180.0f, 180.0f, 0.0f));   // 6

        // Initialise la seed pour l'algorithme d'aleatoire
        // (seed = le nombre avec lequel l'algorithme commence, le point de depart
        // Avec comme parametre la seed -> Le nombre de frame + Un nombre aléatoire avec l'algorithme Unity par defaut entre 0 et 1000
        // convertie en unsigned int
        rand = new Random((UInt64)Time.frameCount + (UInt64)UnityEngine.Random.Range(0, 1000));

        // Pourquoi pas utiliser UnityEngine.Random pour tout les génération ? On tombais BIEN PLUS SOUVENT sur 1 puis 3 puis 5 que sur le reste
        // Oui, on veux un moyen de truqué les dé, mais pour favoriser 4, 5, 6, pas 1, 3, 5
        // Tl;Dr - L'algorithme de base est foireux selon les tests

    }

    // Update is called once per frame
    void Update()
    {
        // Si le flag rolling est actif, rentrer dans la procedure de rotation
        if (rolling)
        {
            // Si le nombre de frame est ecoulée
            if(timer == 0)
            {
                // Set le timer pour le temps d'attente avant de recacher les dés, en frames
                timer = 90;
                // Change le flag pour prevenir que le dé tourne plus
                rolling = false;

                // Update le nombre obtenu, pour que les autres objets puisse le recupérer
                latestRoll = number + 1;

                // Quitte la fonction, pour ne pas regénérer un nombre inutilement
                return;
            }

            // Decrémente le timer
            timer--;

            // Genere un nombre aléatoire entre 0, 4294967294 (source : https://docs.unity3d.com/Packages/com.unity.mathematics@1.3/api/Unity.Mathematics.Random.html)
            // Puis fait un modulo 6 pour avec un nombre entre 0 et 5
            number = rand.NextUInt64() % 6;

            // Inverse 3 4 5 et 0 1 2
            // Simple superstition de ma part en mode : vu qu'on as pas de chance on vas tomber sur 1 2 3, donc 1 2 3 va devenir 4 5 6
            number = number > 2 ? number - 3 : number + 3;

            // Prends la rotation de la face dans la liste des rotations
            // Puis l'applique a l'objet
            // Pour afficher le bon nombre
            this.transform.rotation = faces[(int)number];

        }

        // Si le flag n'est pas actif mais que le dé est en vu
        else if (GetComponentInParent<Transform>().position.x < 3)
        {
            // Diminuer le timer
            timer--;

            // Si le timer est a 0
            if(timer == 0)
            {
                // Remettre le dé a sa place et remettre le timer a la bonne valeur pour le prochain lancer
                GetComponentInParent<Transform>().position += new Vector3(12 + (center ? 1 : 0), 0, 0);
                timer = 60;
            }
        }
    }

    public void LaunchDice(bool center)
    {
        // Si le dé n'est pas deja au milieu
        if (!(GetComponentInParent<Transform>().position.x < 3)) { 
            // le mettre au milieu
            GetComponentInParent<Transform>().position += new Vector3(-12 - (center ? 1 : 0), 0, 0);
        }

        // Dire que ca commence a tourné
        rolling = true;

        // Mettre en place le flag pour savoir si le dé est centrer, car si oui, il faudra le remettre au bonne endroit
        this.center = center;
    }
}
