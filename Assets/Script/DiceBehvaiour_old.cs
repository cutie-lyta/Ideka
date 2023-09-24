// Ancienne iteration du script controllant les dé, avant d'implementer mon propre algorithme de PRNG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class DiceBehvaiour_old : MonoBehaviour
{
    List<Quaternion> faces = new List<Quaternion>();

    bool rolling = false;
    int timer = 60;

    UInt64 number = 0;

    public UInt64 latestRoll;
    Unity.Mathematics.Random random;

    bool center;

    UInt64 seed;

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

        // Initialise l'objet de Unity : Mathematics.Random()
        // Le systeme alternatif d'aléatoire par Unity
        // Avec comme parametre la seed -> Le nombre de frame + Un nombre aléatoire avec l'algorithme Unity par defaut entre 0 et 1000
        // convertie en unsigned int
        seed = ((uint)Time.frameCount + (uint)UnityEngine.Random.Range(0, 1000));
        random = new Unity.Mathematics.Random((uint)seed);

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
                // Rigging algorithm idea
                // Algorithme 1 - Entierement scrapped, reworked dans l'algo 2

                // Algorithme 2 - too much rigged
                /*
                uint r = random.NextUInt() % 6;
                // ReRoll si c'est 1 et que le prochain check ne sera pas vrai et que la chance le veux bien, ca peux retomber sur 1 c'est pas grave, mais donne une deuxieme chance
                if (number == 0 && r < 3 && random.NextBool())
                {
                    number = random.NextUInt() % 6;
                }

                // Si le prochain roll est au dessus de 4, et du nombre actuelle (commence a 0, donc 3 = 4)
                // Le mettre a la place
                // (Favorise un petit peu d'avoir des bons roll)
                if (r >= 3 && r > number)
                {
                    number = r;
                }
                */

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
            number = random.NextUInt() % 6;

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
        if (!(GetComponentInParent<Transform>().position.x < 3)) { 
            GetComponentInParent<Transform>().position += new Vector3(-12 - (center ? 1 : 0), 0, 0);
        }
        rolling = true;

        this.center = center;
    }
}
