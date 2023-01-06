using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip playerMelee, playerSpell, playerJump, playerHit, playerDie, playerWalk, playerCheck, playerRevive;
    public AudioClip spellExplode;
    public AudioClip enemyDie, enemyAttack, enemyHit;
    public AudioClip flyingDie, flyingAttack, flyingHit;
    public AudioClip golemLaser, golemSlam, golemShockwave;
    public AudioClip shopBuy;
    public AudioClip itemPotion, itemCoin;

    public AudioClip stageBgm, golemBgm;

    public AudioSource audioSrc;
    public AudioSource bgmSrc;

    public PlayerController playerController;
    public GameManager gm;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Image sfxSpeaker;
    [SerializeField] private Image bgmSpeaker;
    public Sprite mute;
    public Sprite on;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("audioVolume") && !PlayerPrefs.HasKey("bgmVolume"))
        {
            PlayerPrefs.SetFloat("audioVolume", 1);
            PlayerPrefs.SetFloat("bgmVolume", 1);
            Load();
        } else
        {
            Load();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeVolume();

        if(SceneManager.GetActiveScene().name.ToString() == "Stage1")
        {
            if (bgmSrc.clip != stageBgm)
            {
                bgmSrc.Stop();
            }

            if (!bgmSrc.isPlaying)
            {
                bgmSrc.clip = stageBgm;
                bgmSrc.Play();
            }   
        }
        else if (SceneManager.GetActiveScene().name.ToString() == "Stage2")
        {
            if (bgmSrc.clip != stageBgm)
            {
                bgmSrc.Stop();
            }

            if (!bgmSrc.isPlaying)
            {
                bgmSrc.clip = stageBgm;
                bgmSrc.Play();
            }
        }
        else if (SceneManager.GetActiveScene().name.ToString() == "Stage3")
        {
            if (bgmSrc.clip != golemBgm)
            {
                bgmSrc.Stop();
            }

            if (!bgmSrc.isPlaying)
            {
                bgmSrc.clip = golemBgm;
                bgmSrc.Play();
            }
        }
        else
        {
            if (bgmSrc.isPlaying)
            {
                bgmSrc.Stop();
            }
        }
    }

    public void ChangeVolume()
    {
        audioSrc.volume = volumeSlider.value;
        bgmSrc.volume = bgmSlider.value;
        if (bgmSlider.value == 0)
        {
            bgmSpeaker.sprite = mute;
        }
        else
        {
            bgmSpeaker.sprite = on;
        }

        if (volumeSlider.value == 0)
        {
            sfxSpeaker.sprite = mute;
        }
        else
        {
            sfxSpeaker.sprite = on;
        }
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("audioVolume");
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("audioVolume", volumeSlider.value);
        PlayerPrefs.SetFloat("bgmVolume", bgmSlider.value);
    }

    public void PlayAudio(string clip)
    {
        switch (clip)
        {
            case "playerMelee":
                audioSrc.PlayOneShot(playerMelee);
                break;
            case "playerSpell":
                audioSrc.PlayOneShot(playerSpell);
                break;
            case "playerJump":
                audioSrc.PlayOneShot(playerJump);
                break;
            case "playerHit":
                audioSrc.PlayOneShot(playerHit);
                break;
            case "playerDie":
                audioSrc.PlayOneShot(playerDie);
                break;
            case "playerWalk":
                audioSrc.PlayOneShot(playerWalk);
                break;
            case "playerCheck":
                audioSrc.PlayOneShot(playerCheck);
                break;
            case "playerRevive":
                audioSrc.PlayOneShot(playerRevive);
                break;
            case "spellExplode":
                audioSrc.PlayOneShot(spellExplode);
                break;
            case "shopBuy":
                audioSrc.PlayOneShot(shopBuy);
                break;
            case "enemyDie":
                audioSrc.PlayOneShot(enemyDie);
                break;
            case "enemyHit":
                audioSrc.PlayOneShot(enemyHit);
                break;
            case "enemyAttack":
                audioSrc.PlayOneShot(enemyAttack);
                break;
            case "flyingDie":
                audioSrc.PlayOneShot(flyingDie);
                break;
            case "flyingHit":
                audioSrc.PlayOneShot(flyingHit);
                break;
            case "flyingAttack":
                audioSrc.PlayOneShot(flyingAttack);
                break;
            case "golemLaser":
                audioSrc.PlayOneShot(golemLaser);
                break;
            case "golemSlam":
                audioSrc.PlayOneShot(golemSlam);
                break;
            case "golemShockwave":
                audioSrc.PlayOneShot(golemShockwave);
                break;
            case "stageBgm":
                bgmSrc.PlayOneShot(stageBgm);
                break;
            case "golemBgm":
                bgmSrc.PlayOneShot(golemBgm);
                break;
            case "itemPotion":
                audioSrc.PlayOneShot(itemPotion);
                break;
            case "itemCoin":
               audioSrc.PlayOneShot(itemCoin);
                break;
        }

    }
}
