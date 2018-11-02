using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    #region vars

    #region public Vars
    [Header("Default values")]
    public float speed = 6.0f;
    public float sprintSpeed = 26.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float rotationSpeed = 90.0f;
    public float hp = 100.0f;

    [Header("UI")]
    public Text Counter;
    public Slider slider;
    public Text HpText;
    public Text CoinText;

    [Header("GameObject")]
    public GameObject jumpEffect;
    public GameObject coin;
    public GameObject damage;
    public GameObject finishPanel;
    #endregion

    #region private Vars
    private bool doubleJump = false;

    private float defaultSpeed;
    private int count = 0;
    private float defpos;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private int countOfCoins;

    #endregion

    #endregion

    #region UnityMethods

    private void Awake()
    {
        InitalisatonOfObjects();
    }

    void Start () {

        controller = GetComponent<CharacterController>();
        defaultSpeed = speed;
        slider.maxValue = hp;
        slider.value = hp;
        jumpEffect.SetActive(false);
        defpos = transform.position.y;
        countOfCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

    }
	
	
	void Update () {

        MoveController();

        Sprint();

        HpSystemAndUI();
	}

    #endregion

    #region Controllers

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift)) speed = Mathf.Lerp(speed, sprintSpeed, Time.deltaTime * 0.2f);
        else speed = Mathf.Lerp(speed, defaultSpeed, Time.deltaTime * 0.2f);
    }

    private void MoveController()
    {

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
                doubleJump = true;
            }
            defpos = transform.position.y;
        }

        else
        {
            if (defpos - transform.position.y > 50) hp--;
            
            if (doubleJump && Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed * 1.5f;
                doubleJump = false;
                GameObject gm = Instantiate(jumpEffect, jumpEffect.transform.position, Quaternion.identity);
                gm.SetActive(true);
                StartCoroutine(DestroyJumpEffect(gm));
            }

            else moveDirection.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveDirection * Time.deltaTime);

        controller.transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0);
    }

    private void OnControllerColliderHit(ControllerColliderHit collider)
    {
        if (collider.gameObject.tag == "Coin")
        {
            Destroy(collider.gameObject);
            count++;
        }
        else if (collider.gameObject.tag == "Hit") hp -= 1.0f;
    }

    private void InitalisatonOfObjects()
    {
        for (int i = 0; i <= Random.Range(5, 50); i++)
        {
            GameObject coiner = Instantiate(coin, new Vector3(Random.Range(0, 50), Random.Range(2, 5), Random.Range(0, 50)), Quaternion.identity);
            coiner.transform.Rotate(new Vector3(90, 0, 0));
        }
        for (int i = 0; i <= Random.Range(3, 20); i++)
        {
            Instantiate(damage, new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50)), Quaternion.identity);
        }
    }

    #endregion

    #region UI

    private void HpSystemAndUI()
    {
        Counter.text = "Collected: " + count.ToString() + "/" + countOfCoins.ToString();

        slider.value = hp;

        HpText.text = "HP: " + hp.ToString();

        if (hp <= 0) SceneManager.LoadScene("menu");

        if (count >= countOfCoins) FinishGame();
    }

    private void FinishGame()
    {
        finishPanel.SetActive(true);
        CoinText.text = "Collected: " + count.ToString() + " of " + countOfCoins.ToString() + " coins";
    }

    #endregion

    #region Interfaces

    IEnumerator DestroyJumpEffect(GameObject _game)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(_game);
    }

    #endregion

}
