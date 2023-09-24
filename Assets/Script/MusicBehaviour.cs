using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBehaviour : MonoBehaviour
{
    // Les clips jouable de la musique : Le debut, la boucle, et la fin
    public AudioClip intro;
    public AudioClip loop;
    public AudioClip ending;        // TODO : Fix l'implementation de la fin

    // La source
    AudioSource listener;

    // Fade-in - Le volume max et le nombre d'unité a monter par frame
    public float maxVolume;
    public int riseAmount;

    // Start is called before the first frame update
    void Awake()
    {
        // Recupere l'audio source
        listener = GetComponent<AudioSource>();

        // Mets le volume a 0, le fade in
        listener.volume = 0;

        // Si l'intro existe
        if (intro is not null)
        {
            // La jouer
            listener.clip = intro;
            listener.Play();
        }

        // Si il y a pas d'intro, commencer la loop direct
        else
        {
            listener.clip = loop;
            listener.loop = true;
            listener.Play();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Si le volume n'est pas le volume max, l'augmenter de riseAmount convertie en float entre 0 et 1
        if (listener.volume < maxVolume) listener.volume += ((float)riseAmount / 1000.0f);

        // Si plus rien ne joue (si l'intro est fini)
        if (listener.isPlaying == false)
        {
            // Jouer la boucle principal, en mettant le flag "loop" en vrai pour dire a Unity de boucler
            listener.clip = loop;
            listener.loop = true;
            listener.Play();
        }
    }

    // TODO: Fix
    private void OnDestroy()
    {
        // Intention original : Quand la scene ce termine, jouer la fin, puis laisser la scene passer
        if (ending is not null)
        {
            listener.clip = ending;
            listener.loop = false;
            listener.Play();

            // Ne marche pas hors d'une coroutine -_-
            new WaitForSeconds(listener.clip.length);
        }
    }
}
