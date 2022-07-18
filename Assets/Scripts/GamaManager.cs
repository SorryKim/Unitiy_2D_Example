using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamaManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;

   public void NextStage()
    {

        if (stageIndex < Stages.Length - 1)
        {
            // Change Stage
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
            // Calculate Point
            totalPoint += stagePoint;
            stagePoint = 0;
        }

        // Game Clear
        else
        {
            Time.timeScale = 0;
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "CLEAR!";
            RestartBtn.SetActive(true);
            
            
        }
    }

    void Update()
    {
        UIPoint.text = "Score: " + (totalPoint + stagePoint).ToString();
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.2f);
        }

        else
        {
            // All Health UI Off
            UIhealth[0].color = new Color(1, 1, 1, 0.2f);

            // Player Die Effect
            player.OnDie();

            // Result UI
            Debug.Log("ав╬З╫ю╢о╢ы....");

            // Retry Button UI
            RestartBtn.SetActive(true);
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Player Reposition
            if (health > 1)
            {
                PlayerReposition();
            }
            HealthDown();
        }
    }


    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
