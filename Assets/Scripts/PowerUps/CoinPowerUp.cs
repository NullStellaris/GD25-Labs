using UnityEngine;

public class CoinPowerUp : BasePowerup
{
    public AudioClip coin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.Coin;
        GameManager.instance.gameRestart.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // interface implementation
    public override void SpawnPowerup()
    {
        Debug.Log("coin spawned");
        spawned = true;
        SoundManager.PlaysoundFXClip(coin, transform.position, 1f);
        GameManager.instance.IncreaseScore(5);
    }


    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        gameObject.SetActive(false);
    }
    public void GameStart()
    {
        gameObject.SetActive(true);
        spawned = false;
        this.GetComponent<Animator>().SetTrigger("reset");
    }
}
