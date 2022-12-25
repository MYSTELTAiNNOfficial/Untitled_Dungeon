using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int index;
    public PlayerController playerController;
    public CameraController cameraController;
    public EventSystem eventSystem;
    public TMPro.TextMeshProUGUI tmpScore;
    public TMPro.TextMeshProUGUI tmpHp;
    public GameObject tutorial;
    public Canvas gameover;
    public Canvas selectlevel;
    private int score = 0;
    private float delayOnAttack = 0;
    private float castCooldown = 0;
    private float meleeColldown = 0;

    private float move;

    // Start is called before the first frame update
    void Start()
    {
        gameover.gameObject.SetActive(false);
        selectlevel.gameObject.SetActive(false);
        tutorial.SetActive(true);
        index = PlayerPrefs.GetInt("current_level", 1);
        score = PlayerPrefs.GetInt("score", 0);
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(cameraController.gameObject);
        DontDestroyOnLoad(eventSystem.gameObject);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.action = Player_OnTriggerEnter2D;
        cameraController.target = playerController.transform;
    }

    private void Player_OnTriggerEnter2D(Collider2D collider)
    {
        if (playerController.getHP() > 0)
        {
            if (collider != null && collider.gameObject.tag == "Finish")
            {
                Vector3 playerDefaultSpawn = playerController.transform.position;
                playerDefaultSpawn.x = -7.14f;
                playerDefaultSpawn.y = -3.72f;
                playerController.transform.position = playerDefaultSpawn;
                index++;
                PlayerPrefs.SetInt("current_level", index);
                SceneManager.LoadScene("Level" + index);
            }
            else if (collider != null && collider.gameObject.tag == "Collectible")
            {
                score += 100;
                PlayerPrefs.SetInt("score", score);
                PlayerPrefs.SetString(collider.gameObject.name, "true");
                Destroy(collider.gameObject);
            }
            else if (collider != null && collider.gameObject.tag == "Potion")
            {
                playerController.heal();
                PlayerPrefs.SetInt("hp", playerController.getHP());
                PlayerPrefs.SetString(collider.gameObject.name, "true");
                Destroy(collider.gameObject);
            }
            else if (collider != null && collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Bullet")
            {
                playerController.hit();
                PlayerPrefs.SetInt("hp", playerController.getHP());
            }
        }
        else
        {

        }

    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        if (tutorial.active || PlayerPrefs.GetString("tutorial_done") != "true")
        {
            if (Input.anyKey || PlayerPrefs.GetString("tutorial_done") == "true")
            {
                tutorial.SetActive(false);
                PlayerPrefs.SetString("tutorial_done", "true");
            }
        } else if (playerController.getHP() == 0)
        {
            gameover.gameObject.SetActive(true);
        }
        else
        {
            if (delayOnAttack > 0)
            {
                playerController.Move(0);
                delayOnAttack -= Time.deltaTime;
            }
            else
            {
                playerController.Move(move);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerController.Jump(1);
                }

                if (castCooldown > 0)
                {
                    castCooldown-= Time.deltaTime;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        playerController.Cast_Animation();
                        delayOnAttack = 1;
                        castCooldown = 3;
                    }
                }

                if (meleeColldown > 0)
                {
                    meleeColldown -= Time.deltaTime;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        playerController.Melee_Animation();
                        delayOnAttack = 1;
                        meleeColldown = 1.5F;
                    }
                }
            }
        }

        tmpScore.SetText("<sprite=160> " + score);
        tmpHp.SetText("<sprite=6> " + playerController.getHP());

        //For auto destroy GameObject
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (PlayerPrefs.GetString(go.name) == "true")
            {
                Destroy(go);
            }
        }
    }

    public void RestartLevel()
    {
        gameover.gameObject.SetActive(false);
        PlayerPrefs.DeleteAll();
        Vector3 playerDefaultSpawn = playerController.transform.position;
        playerDefaultSpawn.x = -7.14f;
        playerDefaultSpawn.y = -3.72f;
        playerController.transform.position = playerDefaultSpawn;
        SceneManager.LoadScene("Level" + index);
    }

    public void selectLevel()
    {
        if (gameover.gameObject.active)
        {
            gameover.gameObject.SetActive(false);
        }
        selectlevel.gameObject.SetActive(true);
    }

    public void select_Level1()
    {
        selectlevel.gameObject.SetActive(false);
        playerController.StartSpawn();
        SceneManager.LoadScene("Level1");
        PlayerPrefs.SetInt("current_level", 1);
    }

    public void select_Level2()
    {
        selectlevel.gameObject.SetActive(false);
        Vector3 playerDefaultSpawn = playerController.transform.position;
        playerController.StartSpawn();
        playerController.transform.position = playerDefaultSpawn;
        SceneManager.LoadScene("Level2");
        PlayerPrefs.SetInt("current_level", 2);
    }

    public void select_Level3()
    {
        selectlevel.gameObject.SetActive(false);
        Vector3 playerDefaultSpawn = playerController.transform.position;
        playerController.StartSpawn();
        playerController.transform.position = playerDefaultSpawn;
        SceneManager.LoadScene("Level3");
        PlayerPrefs.SetInt("current_level", 3);
    }
}
