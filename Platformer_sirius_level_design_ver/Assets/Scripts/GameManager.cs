using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    public int health = 3;

    public PlayerControl player;
    public GameObject[] stages;

    public Image[] UIhealth;
    public Text UIpoint;
    public Text UIstage;
    public GameObject UIrestartButton;




    public void NextStage()
    {
        if (stageIndex < stages.Length -1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIstage.text = "STAGE " + (stageIndex + 1);
        }
        else // game clear
        {
            // time stop
            Time.timeScale = 0;
            Text btnText = UIrestartButton.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIrestartButton.SetActive(true);
        }

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // player reposition
            if (health > 1)
            {
                PlayerReposition();
            }
            // health down
            HealthDown();

        }
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);

            player.OnDie();
            Text btnText = UIrestartButton.GetComponentInChildren<Text>();
            btnText.text = "ReStart?";
            UIrestartButton.SetActive(true);

        }

    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, 1);
        player.VelocityZero();
    }

    private void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }

    public void Restart()
    {
        UIrestartButton.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
