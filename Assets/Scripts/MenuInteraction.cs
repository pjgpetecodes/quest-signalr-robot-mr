using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInteraction : MonoBehaviour
{

    [SerializeField]
    private GameObject _menuParent;

    [SerializeField]
    private AudioSource _showMenuAudio;

    [SerializeField]
    private AudioSource _hideMenuAudio;

    // Start is called before the first frame update
    void Start()
    {
        _menuParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Show/hide the menu.
    /// </summary>
    public void ToggleMenu()
    {
        if (_menuParent.activeSelf)
        {
            _hideMenuAudio.Play();
            _menuParent.SetActive(false);
        }
        else
        {
            _showMenuAudio.Play();
            _menuParent.SetActive(true);
        }
    }
}
