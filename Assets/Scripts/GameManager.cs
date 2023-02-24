using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerController playerController;
    public CameraController cameraController;
    public EnemyController enemyController;
    public FlyingEnemyController flyingEnemyController;
    public GolemController golemController;
    public EventSystem eventSystem;
    public AudioManager audioManager;

    public TMPro.TextMeshProUGUI tmpScore;
    public TMPro.TextMeshProUGUI tmpHp;
    public TMPro.TextMeshProUGUI tmpNotif;
    public TMPro.TextMeshProUGUI tmpNotifShop;
    public TMPro.TextMeshProUGUI tmpNotifSelectLevel;
    public TMPro.TextMeshProUGUI tmpNotifGameover;
    public TMPro.TextMeshProUGUI castCdText;
    public TMPro.TextMeshProUGUI atkCdText;
    public TMPro.TextMeshProUGUI shopCoin;
    public TMPro.TextMeshProUGUI shopAtkPrice;
    public TMPro.TextMeshProUGUI shopMagicPrice;
    public TMPro.TextMeshProUGUI shopHpPrice;
    public GameObject tutorial;
    public Canvas gameovercanvas;
    public Canvas selectlevelcanvas;
    public Canvas mainMenucanvas;
    public Canvas settingcanvas;
    public Canvas pausecanvas;
    public Canvas shopcanvas;
    public Image castCdImage;
    public Image atkCdImage;
    public Image buffAtk;
    public Image buffMagic;
    public Image buffHp;


    private int index = 1;
    private int coin = 0;
    private float delayOnAttack = 0;
    private float castCooldown = 3;
    private float meleeCooldown = 1.5f;
    private float castCooldownTimer;
    private float meleeCooldownTimer;
    public bool isDie;
    public float notifDelay;
    public string notifString;
    private float move;
    private string current_stage;
    private string lastcheck;
    private string stage1unlock;
    private string stage2unlock;
    private string stage3unlock;

    private System.Random rand = new System.Random();

    private bool fromMainMenu = false;
    private bool fromPause = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        coin = PlayerPrefs.GetInt("coin", 0);
        lastcheck = PlayerPrefs.GetString("checkpoint_stage", "");
        stage1unlock = PlayerPrefs.GetString("stage1unlock", "false");
        stage2unlock = PlayerPrefs.GetString("stage2unlock", "false");
        stage3unlock = PlayerPrefs.GetString("stage3unlock", "false");
        tmpNotifShop.SetText("");
        shopCoin.SetText("<sprite=157> " + coin);
        current_stage = lastcheck;
        checkBuff();
        gameovercanvas.gameObject.SetActive(false);
        selectlevelcanvas.gameObject.SetActive(false);
        tutorial.SetActive(true);
        mainMenucanvas.gameObject.SetActive(true);
        settingcanvas.gameObject.SetActive(false);
        pausecanvas.gameObject.SetActive(false);
        shopcanvas.gameObject.SetActive(false);

        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(cameraController.gameObject);
        DontDestroyOnLoad(eventSystem.gameObject);
        DontDestroyOnLoad(audioManager.gameObject);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        playerController = FindObjectOfType<PlayerController>();
        enemyController = FindObjectOfType<EnemyController>();
        flyingEnemyController = FindObjectOfType<FlyingEnemyController>();
        golemController = FindObjectOfType<GolemController>();
        playerController.action = Player_OnTriggerEnter2D;
        cameraController.target = playerController.transform;
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Player_OnTriggerEnter2D(Collider2D collider)
    {
        if (isDie == false)
        {
            Debug.Log(collider.gameObject.tag);
            if (collider != null && collider.gameObject.tag == "Finish")
            {
                if (SceneManager.GetActiveScene().name.ToString() == "Stage1")
                {
                    PlayerPrefs.SetString("stage1unlock", "true");
                }
                if (SceneManager.GetActiveScene().name.ToString() == "Stage2")
                {
                    PlayerPrefs.SetString("stage2unlock", "true");
                    PlayerPrefs.SetString("stage3unlock", "true");
                    current_stage = "Stage2";
                }

                PlayerPrefs.SetInt("hp", playerController.getHP());
                PlayerPrefs.SetString("checkpoint_stage", "");
                lastcheck = "";
                index++;
                SceneManager.LoadScene("Stage" + index);
            }
            else if (collider != null && collider.gameObject.tag == "Collectible")
            {
                int stash = rand.Next(1, 5);
                coin += stash;
                PlayerPrefs.SetInt("coin", coin);
                notifString = "Collectible collected! Receiving " + stash + " coins!";
                notifDelay = 3f;
                Destroy(collider.gameObject);
                audioManager.PlayAudio("itemCoin");
            }
            else if (collider != null && collider.gameObject.tag == "Zonk")
            {
                notifString = "You got zonk collectible!";
                notifDelay = 3f;
                Destroy(collider.gameObject);
            }
            else if (collider != null && collider.gameObject.tag == "Potion")
            {
                int value = 40;
                playerController.heal(value);
                PlayerPrefs.SetInt("hp", playerController.getHP());
                notifString = "Potion collected! Heal " + value + " health!";
                notifDelay = 3f;
                Destroy(collider.gameObject);
                audioManager.PlayAudio("itemPotion");
            }
            else if (collider != null && collider.gameObject.tag == "Bullet")
            {
                Debug.Log("Hit" + flyingEnemyController.getAP());
                playerController.hit(flyingEnemyController.getAP());
                if (playerController.getHP() <= 0)
                {
                    playerController.Die();
                    isDie = true;
                }
            }
            else if (collider != null && collider.gameObject.tag == "Laser")
            {
                Debug.Log("Hit" + golemController.getAP());
                playerController.hit(golemController.getAP());
                if (playerController.getHP() <= 0)
                {
                    playerController.Die();
                    isDie = true;
                }
            }
            else if (collider != null && collider.gameObject.tag == "Checkpoint")
            {
                Debug.Log(current_stage);
                Debug.Log(lastcheck);
                if (current_stage != lastcheck)
                {
                    notifString = "Checkpoint has been activated!";
                    notifDelay = 3f;
                    PlayerPrefs.SetString("checkpoint_stage", current_stage);
                    lastcheck = PlayerPrefs.GetString("checkpoint_stage", "");
                    PlayerPrefs.SetInt("hp", playerController.getHP());
                    audioManager.PlayAudio("playerCheck");
                }
                else
                {
                    notifString = "Checkpoint has already activated!";
                    notifDelay = 3f;
                }
            }
            else if (playerController.getHP() <= 0)
            {
                playerController.Die();
                isDie = true;
            }
        }
    }

    void Update()
    {
        if (playerController.getHP() > 0)
        {
            isDie = false;
            gameovercanvas.gameObject.SetActive(false);
        }
        Time.timeScale = 1;
        if (tutorial.active || PlayerPrefs.GetString("tutorial_done") != "true")
        {
            if (Input.anyKey || PlayerPrefs.GetString("tutorial_done") == "true")
            {
                tutorial.SetActive(false);
                PlayerPrefs.SetString("tutorial_done", "true");
            }
        }
        else if (isDie || playerController.getHP() <= 0)
        {
            gameovercanvas.gameObject.SetActive(true);
            isDie = true;
        }
        else
        {
            current_stage = SceneManager.GetActiveScene().name;
            gameovercanvas.gameObject.SetActive(false);
            move = Input.GetAxisRaw("Horizontal");
            if (Input.GetAxisRaw("Horizontal") == 1f || Input.GetAxisRaw("Horizontal") == -1f)
            {
                if (!audioManager.audioSrc.isPlaying)
                {
                    audioManager.PlayAudio("playerWalk");
                }
            }
            if (delayOnAttack > 0)
            {
                playerController.Move(0);
                delayOnAttack -= Time.deltaTime;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (move == 1)
                    {
                        move++;
                    }
                    else if (move == -1)
                    {
                        move--;
                    }
                }

                playerController.Move(move);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerController.Jump(1);
                }


                if (castCooldownTimer <= 0.0f && playerController.animator.GetBool("Jump") == false)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        playerController.Cast_Animation();
                        delayOnAttack = 1;
                        castCooldownTimer = castCooldown;
                    }
                }


                if (meleeCooldownTimer <= 0.0f && playerController.animator.GetBool("Jump") == false)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        playerController.Melee_Animation();
                        delayOnAttack = 1;
                        meleeCooldownTimer = meleeCooldown;
                    }
                }
            }
        }

        castCooldownTimer -= Time.deltaTime;

        if (castCooldownTimer > 0.0f)
        {
            castCdText.gameObject.SetActive(true);
            castCdText.text = Mathf.RoundToInt(castCooldownTimer).ToString();
            castCdImage.fillAmount = castCooldownTimer / castCooldown;
        }
        else
        {
            castCdText.gameObject.SetActive(false);
            castCdImage.fillAmount = 0.0f;
        }

        meleeCooldownTimer -= Time.deltaTime;

        if (meleeCooldownTimer > 0.0f)
        {
            atkCdText.gameObject.SetActive(true);
            atkCdText.text = Mathf.RoundToInt(meleeCooldownTimer).ToString();
            atkCdImage.fillAmount = meleeCooldownTimer / meleeCooldown;
        }
        else
        {
            atkCdText.gameObject.SetActive(false);
            atkCdImage.fillAmount = 0.0f;
        }

        stage1unlock = PlayerPrefs.GetString("stage1unlock", "false");
        stage2unlock = PlayerPrefs.GetString("stage2unlock", "false");
        stage3unlock = PlayerPrefs.GetString("stage3unlock", "false");
        coin = PlayerPrefs.GetInt("coin");
        tmpNotif.SetText(notifString);
        tmpScore.SetText("<sprite=157> " + coin);
        tmpHp.SetText("<sprite=0> " + playerController.getHP());

        checkBuff();

        if (PlayerPrefs.GetString("isAtkGetTemp") == "true" &&
        PlayerPrefs.GetString("isMagicGetTemp") == "true")
        {
            checkBuffTemp();
        }

        if (notifDelay > 0)
        {
            notifDelay -= Time.deltaTime;
        }
        else
        {
            tmpNotif.SetText("");
        }

        ////For auto destroy GameObject
        //GameObject[] allObjects = FindObjectsOfType<GameObject>();
        //foreach (GameObject go in allObjects)
        //{
        //    if (PlayerPrefs.GetString(go.name) == "true")
        //    {
        //        Destroy(go);
        //    }
        //}
    }

    public void Revive()
    {
        PlayerPrefs.SetString("revivePremium", "false");
        PlayerPrefs.SetString("revive", "true");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name.ToString());
        audioManager.PlayAudio("playerRevive");
    }

    public void Revive_Premium()
    {
        int coin = PlayerPrefs.GetInt("coin", 0);
        if (coin >= 10 && lastcheck != "")
        {
            PlayerPrefs.SetString("revivePremium", "true");
            PlayerPrefs.SetString("revive", "true");
            SceneManager.LoadScene(lastcheck);
            coin -= 10;
            PlayerPrefs.SetInt("coin", coin);
            audioManager.PlayAudio("playerRevive");

        }
        else if (lastcheck == "")
        {
            tmpNotifGameover.SetText("You need to reach checkpoint to use revive premium!");
        }
        else if (coin < 10)
        {
            tmpNotifGameover.SetText("Coin not enough to use revive premium!");
        }
    }

    public void back()
    {
        if (fromMainMenu)
        {
            fromMainMenu = false;
            selectlevelcanvas.gameObject.SetActive(false);
            settingcanvas.gameObject.SetActive(false);
            shopcanvas.gameObject.SetActive(false);
            mainMenucanvas.gameObject.SetActive(true);
        }
        if (fromPause)
        {
            fromPause = false;
            settingcanvas.gameObject.SetActive(false);
            pausecanvas.gameObject.SetActive(true);
        }
    }

    public void play()
    {
        if (lastcheck != "")
        {
            PlayerPrefs.SetString("continue", "true");
            Time.timeScale = 1;
            gameovercanvas.gameObject.SetActive(false);
            selectlevelcanvas.gameObject.SetActive(false);
            tutorial.SetActive(false);
            mainMenucanvas.gameObject.SetActive(false);
            settingcanvas.gameObject.SetActive(false);
            pausecanvas.gameObject.SetActive(false);
            shopcanvas.gameObject.SetActive(false);
            mainMenucanvas.gameObject.SetActive(false);
            SceneManager.LoadScene(lastcheck);
            playerController.StartSpawn();
            current_stage = lastcheck;
        }
        else
        {
            PlayerPrefs.SetString("checkpoint_stage", "");
            PlayerPrefs.SetString("continue", "false");
            Time.timeScale = 1;
            mainMenucanvas.gameObject.SetActive(false);
            SceneManager.LoadScene("Stage1");
            current_stage = "Stage1";
            isDie = false;
            gameovercanvas.gameObject.SetActive(false);
            selectlevelcanvas.gameObject.SetActive(false);
            tutorial.SetActive(true);
            mainMenucanvas.gameObject.SetActive(false);
            settingcanvas.gameObject.SetActive(false);
            pausecanvas.gameObject.SetActive(false);
            shopcanvas.gameObject.SetActive(false);
            playerController.StartSpawn();
        }
    }

    public void selectLevel()
    {

        if (mainMenucanvas.gameObject.active)
        {
            mainMenucanvas.gameObject.SetActive(false);
            fromMainMenu = true;
        }

        selectlevelcanvas.gameObject.SetActive(true);
    }

    public void pause()
    {
        Time.timeScale = 0;
        pausecanvas.gameObject.SetActive(true);
    }
    public void Continue()
    {
        Time.timeScale = 1;
        pausecanvas.gameObject.SetActive(false);
    }

    public void setting()
    {
        if (pausecanvas.gameObject.active)
        {
            fromPause = true;
            pausecanvas.gameObject.SetActive(false);
            settingcanvas.gameObject.SetActive(true);
        }
        if (mainMenucanvas.gameObject.active)
        {
            fromMainMenu = true;
            mainMenucanvas.gameObject.SetActive(false);
            settingcanvas.gameObject.SetActive(true);
        }
    }

    public void exit_stage()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("DummyForExit", LoadSceneMode.Single);

        pausecanvas.gameObject.SetActive(false);
        mainMenucanvas.gameObject.SetActive(true);
    }

    public void select_Stage1()
    {
        if (stage1unlock == "false")
        {
            tmpNotifSelectLevel.SetText("You need to clear this stage before you can select this stage");
        }
        else
        {

            PlayerPrefs.SetInt("hp", 0);
            index = 1;
            PlayerPrefs.SetString("continue", "false");
            PlayerPrefs.SetString("checkpoint_stage", "");
            selectlevelcanvas.gameObject.SetActive(false);
            SceneManager.LoadScene("Stage1");
            current_stage = "Stage1";
            lastcheck = "";
        }
    }

    public void select_Stage2()
    {
        if (stage2unlock == "false")
        {
            tmpNotifSelectLevel.SetText("You need to clear this stage before you can select this stage");
        }
        else
        {
            index = 2;

            PlayerPrefs.SetInt("hp", 0);
            PlayerPrefs.SetString("continue", "false");
            PlayerPrefs.SetString("checkpoint_stage", "");
            selectlevelcanvas.gameObject.SetActive(false);
            SceneManager.LoadScene("Stage2");
            current_stage = "Stage2";
            lastcheck = "";
        }
    }

    public void select_StageBoss()
    {
        if (stage3unlock == "false")
        {
            tmpNotifSelectLevel.SetText("You need to clear this stage before you can select this stage");
        }
        else
        {

            PlayerPrefs.SetInt("hp", 0);
            PlayerPrefs.SetString("continue", "false");
            PlayerPrefs.SetString("checkpoint_stage", "");
            lastcheck = "";
            selectlevelcanvas.gameObject.SetActive(false);
            SceneManager.LoadScene("Stage3");
            current_stage = "Stage3";
        }
    }

    public void shopping()
    {
        fromMainMenu = true;
        shopCoin.SetText("<sprite=157> " + PlayerPrefs.GetInt("coin", 0));
        shopcanvas.gameObject.SetActive(true);
    }

    public void buy_attack()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
        string check = PlayerPrefs.GetString("isAtkBought", "false");

        if (check == "true")
        {
            tmpNotifShop.SetText("Already bought!");
        }
        else
        {
            if (coin < 100)
            {
                tmpNotifShop.SetText("Can't buy! Coin not enough!");
            }
            else if (coin >= 100)
            {
                tmpNotifShop.SetText("Coin -100 for buy buff!");
                coin -= 100;
                PlayerPrefs.SetInt("coin", coin);
                PlayerPrefs.SetString("isAtkBought", "true");
                shopCoin.SetText("<sprite=157> " + coin);
                audioManager.PlayAudio("shopBuy");
            }
        }
    }

    public void buy_magic()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
        string check = PlayerPrefs.GetString("isMagicBought", "false");

        if (check == "true")
        {
            tmpNotifShop.SetText("Already bought!");
        }
        else
        {
            if (coin < 100)
            {
                tmpNotifShop.SetText("Can't buy! Coin not enough!");
            }
            else if (coin >= 100)
            {
                tmpNotifShop.SetText("Coin -100 for buy buff!");
                coin -= 100;
                PlayerPrefs.SetInt("coin", coin);
                PlayerPrefs.SetString("isMagicBought", "true");
                shopCoin.SetText("<sprite=157> " + coin);
                audioManager.PlayAudio("shopBuy");
            }
        }
    }

    public void buy_hp()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
        string check = PlayerPrefs.GetString("isHpBought", "false");

        if (check == "true")
        {
            tmpNotifShop.SetText("Already bought!");
        }
        else
        {
            if (coin < 100)
            {
                tmpNotifShop.SetText("Can't buy! Coin not enough!");
            }
            else if (coin >= 100)
            {
                tmpNotifShop.SetText("Coin -100 for buy buff!");
                coin -= 100;
                PlayerPrefs.SetInt("coin", coin);
                PlayerPrefs.SetString("isHpBought", "true");
                shopCoin.SetText("<sprite=157> " + coin);
                audioManager.PlayAudio("shopBuy");
            }
        }
    }

    private void checkBuff()
    {
        if (PlayerPrefs.GetString("isAtkBought") == "true")
        {
            shopAtkPrice.SetText("Sold Out!");
            buffAtk.gameObject.SetActive(true);
        }
        else
        {
            shopAtkPrice.SetText("100");
            buffAtk.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetString("isMagicBought") == "true")
        {
            shopMagicPrice.SetText("Sold Out!");
            buffMagic.gameObject.SetActive(true);
        }
        else
        {
            shopMagicPrice.SetText("100");
            buffMagic.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetString("isHpBought") == "true")
        {
            shopHpPrice.SetText("Sold Out!");
            buffHp.gameObject.SetActive(true);
        }
        else
        {
            shopHpPrice.SetText("100");
            buffHp.gameObject.SetActive(false);
        }
    }

    public void checkBuffTemp()
    {
        if (PlayerPrefs.GetString("isAtkGetTemp") == "true")
        {
            buffAtk.gameObject.SetActive(true);
        }
        else
        {
            buffAtk.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetString("isMagicGetTemp") == "true")
        {
            buffMagic.gameObject.SetActive(true);
        }
        else
        {
            buffMagic.gameObject.SetActive(false);
        }
    }

    public void Exit_Game()
    {
        PlayerPrefs.Save();
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void setNotif(string Text)
    {
        notifString = Text;
        notifDelay = 3;
    }
}
