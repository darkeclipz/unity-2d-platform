using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignMessage : MonoBehaviour {

    public bool hit = false;
    public string message;
    public Text text;
    private AudioSource audioSource;
    public AudioClip audioSignActivated;
    private bool fired = false;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CenterText()
    {
        var worldPos = Camera.main.WorldToScreenPoint(transform.position);
        worldPos.y += 30;
        text.transform.position = worldPos;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CenterText();
        text.text = message;

        if(!fired)
        {
            fired = true;
            audioSource.PlayOneShot(audioSignActivated);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        CenterText();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        text.text = "";
    }
}
