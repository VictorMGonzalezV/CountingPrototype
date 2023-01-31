using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] spheres;
    [SerializeField] GameObject[] positions;
    [SerializeField] GameObject box;
    [SerializeField] float spinDelay=0.07f;
    [SerializeField] float spinDelayMultiplier=3f;
    [SerializeField] int baseSpinCount=30;
    [SerializeField] int bet = 1;
    [SerializeField] Text betText;
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] Text countText;
    [SerializeField] Button retryButton;
    [SerializeField] Button quitButton;
    [SerializeField] GameObject betButtonGroup;
    [SerializeField] Button spinButton;
    [SerializeField] GameObject wagerButtonGroup;
    [SerializeField] Text chipsText;
    [SerializeField] Text wagerText;
    [SerializeField] Text wagerWarningText;

    public static GameManager instance;
    //Static variables are persistent between scenes, so that's another option for small projects like this
    private static int startingChips = 50;
    private static int currentChips=50;
    private static int wager=0;
 
    private void Awake()
    {
        /*Remember this code block at the start, this ensures only one instance will exist at all times (singleton)
        if (instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;*/
        DontDestroyOnLoad(gameObject);
        //GameManager.currentChips = startingChips;
        GameManager.wager = 0;
        chipsText.text =currentChips+" $";
    }

    // Update is called once per frame
    void Update()
    {
        //Used to test the teleportation code, turned into a coroutine to have frame-independent duration and fine-tune the duration of the
        //"spinning" process
        //TeleportBox();
    }

    //This happens only once so it's easier to use a foreach loop rather than directly manipulating the value of the gravity in Unity.Physics
    private void DropBalls()
    {
        foreach (GameObject sphere in spheres)
        {
            sphere.GetComponent<Rigidbody>().useGravity = true;
        }
    }
    
    //The box doesn't need any forces moving it so this is a quick and easy way to create the effect
    private void TeleportBox()
    {
        box.transform.position = positions[Random.Range(0, positions.Length)].transform.position;
    }

    private IEnumerator SpinRoulette(float spinDelay)
    {
        int totalSpins = 0;
        
        while(totalSpins<baseSpinCount)
        {
            
            if (totalSpins<=(baseSpinCount-3))
            {
                yield return new WaitForSeconds(spinDelay);
                TeleportBox();
                totalSpins++;
            } else
            //Slow the last 3 "spins" down for dramatic effect
            {
                yield return new WaitForSeconds(spinDelay*spinDelayMultiplier);
                TeleportBox();
                totalSpins++;
            }

           
        }
              
    }

    public void IncreaseBet()
    {
        bet++;
        if(bet>15)
        {
            bet = 1;
        }
        betText.text = bet.ToString();
    }

    public void DecreaseBet()
    {
        bet--;
        if(bet<=0)
        {
            bet = 15;
        }
        betText.text = bet.ToString();
    }

    public void IncreaseWager()
    {
       
        if (GameManager.currentChips<=0)
        {
            return;
        }
     

        //Wagers are locked to increases/decreases of 10, may change later
        GameManager.wager += 10;
        GameManager.currentChips -= 10;
        if(GameManager.currentChips<0)
        {
            GameManager.wager -= 10;
            GameManager.currentChips +=10;
        }
       
        wagerText.text = GameManager.wager.ToString();
        chipsText.text = GameManager.currentChips + "$";
    }

    public void DecreaseWager()
    {
        if(GameManager.wager<=10)
        {
            return;
        }
       
        GameManager.wager -= 10;
        GameManager.currentChips += 10;
        if (GameManager.wager <= 0)
        {
            GameManager.wager= 10;
        }
        wagerText.text = GameManager.wager.ToString();
        chipsText.text = GameManager.currentChips + "$";

    }

    public void StartGame()
    {
        //Ensure players make a minimum bet to play
        if(GameManager.wager<10)
        {
            wagerWarningText.enabled = true;
            return;
        }
        chipsText.enabled=true;
        betButtonGroup.SetActive(false);
        wagerButtonGroup.SetActive(false);
        quitButton.gameObject.SetActive(false);
        spinButton.gameObject.SetActive(false);
        countText.enabled = true;
        wagerWarningText.enabled = false;
        StartCoroutine(SpinRoulette(spinDelay));
        Invoke("DropBalls", 1f);
        //The balls hit the ground after around 4s, so 5s is a safe value, and much faster than detecting whether all of the balls have hit something
        Invoke("EndGame", 5f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadGame()
    {
       
        SceneManager.LoadScene(0);
    }

    public void EndGame()
    {
        countText.enabled = true;
        if (bet==box.GetComponent<Counter>().ReturnCount())
        {
            winText.enabled = true;
            //Win double of the wager 
            GameManager.currentChips =GameManager.currentChips+(GameManager.wager*2);
            chipsText.text = GameManager.currentChips+"$";
        }else
        {
            loseText.enabled = true;
            chipsText.text = GameManager.currentChips + "$";
        }
        retryButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }
}
