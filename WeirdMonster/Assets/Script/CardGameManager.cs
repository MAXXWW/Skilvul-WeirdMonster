using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CardGameManager : MonoBehaviour, IOnEventCallback
{
    public GameObject netPlayerPrefabs;
    public CardPlayer P1;
    public CardPlayer P2;
    public PlayerStats defaultPlayerStats = new PlayerStats
    {
        MaxHealth = 100f,
        RestoreValue = 5f,
        DamageValue = 10f
    };
    // public float restoreValue = 5f;
    // public float damageValue = 10f;
    // public float maxHealth = 100f;
    public GameState state, nextState = GameState.NetPlayersInitialization;
    private CardPlayer damagedPlayer;
    // private Player winner;
    public GameObject gameOverPanel;
    public TMP_Text winnerText;
    public TMP_Text pingText;
    // public List<int> syncReadyPlayers = new List<int>(2);
    HashSet<int> syncReadyPlayers = new HashSet<int>();
    private int multipleDamageP1 = 1;
    private int multipleDamageP2 = 1;
    public AudioSource AttackSFX;
    public AudioSource AttackDrawSFX;
    public AudioSource WinSFX;
    public GameObject pauseButton;
    public TMP_Text P1ComboTxt;
    public TMP_Text P2ComboTxt;
    public bool Online = true;

    public enum GameState
    {
        SyncyState,
        NetPlayersInitialization,
        ChooseAttack,
        Attack,
        Damages,
        Draw,
        GameOver,
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        if (Online)
        {
            PhotonNetwork.Instantiate(netPlayerPrefabs.name, Vector3.zero, Quaternion.identity);
            StartCoroutine(PingCoroutine());

            state = GameState.NetPlayersInitialization;
            nextState = GameState.NetPlayersInitialization;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PlayerPropertyNames.Room.MaxHealth, out var maxHealth))
            {
                defaultPlayerStats.MaxHealth = (float)maxHealth;
                P1.Health = defaultPlayerStats.MaxHealth;
                P1.MaxHealth = defaultPlayerStats.MaxHealth;
                P2.Health = defaultPlayerStats.MaxHealth;
                P2.MaxHealth = defaultPlayerStats.MaxHealth;

                Debug.Log("Max Health: " + defaultPlayerStats.MaxHealth);
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PlayerPropertyNames.Room.RestoreValue, out var restoreValue))
            {
                defaultPlayerStats.RestoreValue = (float)restoreValue;
                Debug.Log("Restore value: " + defaultPlayerStats.RestoreValue);
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PlayerPropertyNames.Room.DamageValue, out var damageValue))
            {
                defaultPlayerStats.DamageValue = (float)damageValue;
                Debug.Log("Damage value: " + defaultPlayerStats.DamageValue);
            }
        }
        else
        {
            state = GameState.ChooseAttack;
        }

        P1.SetStats(defaultPlayerStats, true);
        P2.SetStats(defaultPlayerStats, true);
        P1.IsReady = true;
        P2.IsReady = true;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.SyncyState:
                if (syncReadyPlayers.Count == 2)
                {
                    syncReadyPlayers.Clear();
                    state = nextState;
                }
                break;
            case GameState.NetPlayersInitialization:
                if (CardNetPlayer.NetPlayers.Count == 2)
                {
                    foreach (var netPlayer in CardNetPlayer.NetPlayers)
                    {
                        if (netPlayer.photonView.IsMine)
                        {
                            netPlayer.Set(P1);
                        }
                        else
                        {
                            netPlayer.Set(P2);
                        }
                    }
                    ChangeState(GameState.ChooseAttack);
                    // state = GameState.ChooseAttack;
                }
                break;
            case GameState.ChooseAttack:
                if (P1.AttackValue != null && P2.AttackValue != null)
                {
                    P1.AnimateAttack();
                    P2.AnimateAttack();
                    P1.isClickAble(false);
                    P2.isClickAble(false);
                    // state = GameState.Attack;
                    ChangeState(GameState.Attack);
                }
                break;
            case GameState.Attack:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    damagedPlayer = GetDamagedPlayer();
                    if (damagedPlayer != null)
                    {
                        AttackSFX.Play(); //tambahan
                        damagedPlayer.AnimateDamage();
                        // state = GameState.Damages;
                        ChangeState(GameState.Damages);
                    }
                    else
                    {
                        AttackDrawSFX.Play();
                        P1.AnimateDraw();
                        P2.AnimateDraw();
                        // state = GameState.Draw;
                        ChangeState(GameState.Draw);
                    }
                }
                break;
            case GameState.Damages:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    if (damagedPlayer == P1)
                    {
                        if (multipleDamageP2 <= 3)
                        {
                            P1.ChangeHealth(-P2.stats.DamageValue * multipleDamageP2);
                            multipleDamageP2++;
                        }
                        else
                        {
                            multipleDamageP2 = 1;
                            P1.ChangeHealth(-P2.stats.DamageValue * multipleDamageP2);
                        }

                        multipleDamageP1 = 1;
                        // P1.ChangeHealth(-damageValue * multipleDamageP2);
                        P2.ChangeHealth(P2.stats.RestoreValue);
                    }
                    else
                    {
                        multipleDamageP2 = 1;
                        if (multipleDamageP1 <= 3)
                        {
                            P2.ChangeHealth(-P1.stats.DamageValue * multipleDamageP1);
                            multipleDamageP1++;
                        }
                        else
                        {
                            multipleDamageP1 = 1;
                            P2.ChangeHealth(-P1.stats.DamageValue * multipleDamageP1);
                        }

                        P1.ChangeHealth(P1.stats.RestoreValue);
                        // P2.ChangeHealth(-damageValue * multipleDamageP1);
                    }

                    if (multipleDamageP2 == 2)
                    {
                        P2ComboTxt.text = "Combo 2x";
                    }
                    else if (multipleDamageP2 == 3)
                    {
                        P2ComboTxt.text = "Combo 3x";
                    }
                    else
                    {
                        P2ComboTxt.text = "Combo 1x";
                    }

                    if (multipleDamageP1 == 2)
                    {
                        P1ComboTxt.text = "Combo 2x";
                    }
                    else if (multipleDamageP1 == 3)
                    {
                        P1ComboTxt.text = "Combo 3x";
                    }
                    else
                    {
                        P1ComboTxt.text = "Combo 1x";
                    }

                    var winner = GetWinner();

                    if (winner == null)
                    {
                        P1.AnimateDraw();
                        P2.AnimateDraw();
                        ResetPlayer();
                        P1.isClickAble(true);
                        P2.isClickAble(true);
                        // state = GameState.ChooseAttack;
                        ChangeState(GameState.ChooseAttack);
                    }
                    else
                    {
                        P1ComboTxt.enabled = false;
                        P2ComboTxt.enabled = false;
                        Debug.Log(winner + " is win");
                        WinSFX.Play();
                        pauseButton.SetActive(false);
                        gameOverPanel.SetActive(true);
                        winnerText.text = winner == P1 ? $"{P1.Name.text} Win!" : $"{P2.Name.text} Win!";
                        // winnerText.text = winner == P1 ? "Player 1 Win!" : "Player 2 Win!";
                        ResetPlayer();
                        // state = GameState.GameOver;
                        ChangeState(GameState.GameOver);
                    }
                }
                break;
            case GameState.Draw:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    ResetPlayer();
                    P1.isClickAble(true);
                    P2.isClickAble(true);
                    // state = GameState.ChooseAttack;
                    ChangeState(GameState.ChooseAttack);
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private const byte playerChangeState = 1;

    private void ChangeState(GameState nextState)
    {
        if (Online == false)
        {
            state = nextState;
            return;
        }

        if (this.nextState == nextState)
        {
            return;
        }

        // kirim message bahwa kita ready
        var actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        var raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(playerChangeState, actorNum, raiseEventOptions, SendOptions.SendReliable);

        // masuk ke state sync sebagai transisi sebelum state baru
        this.state = GameState.SyncyState;
        this.nextState = nextState;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == playerChangeState)
        {
            var actorNum = (int)photonEvent.CustomData;

            // if (syncReadyPlayers.Contains(actorNum) == false)
            // {
            //     syncReadyPlayers.Add(actorNum);
            // }

            // kalau pakai hash set engga perlu cek lagi, seperti yang di atas
            syncReadyPlayers.Add(actorNum);
        }
    }

    IEnumerator PingCoroutine()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            pingText.text = "Ping: " + PhotonNetwork.GetPing() + " ms";
            yield return wait;
        }
    }

    private void ResetPlayer()
    {
        damagedPlayer = null;
        P1.Reset();
        P2.Reset();
    }

    private CardPlayer GetDamagedPlayer()
    {
        Attack? PlayerAtk1 = P1.AttackValue;
        Attack? PlayerAtk2 = P2.AttackValue;

        if (PlayerAtk1 == Attack.Rock && PlayerAtk2 == Attack.Paper)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Rock && PlayerAtk2 == Attack.Scissor)
        {
            return P2;
        }
        else if (PlayerAtk1 == Attack.Paper && PlayerAtk2 == Attack.Rock)
        {
            return P2;
        }
        else if (PlayerAtk1 == Attack.Paper && PlayerAtk2 == Attack.Scissor)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Scissor && PlayerAtk2 == Attack.Rock)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Scissor && PlayerAtk2 == Attack.Paper)
        {
            return P2;
        }
        return null;
    }

    private CardPlayer GetWinner()
    {
        if (P1.Health == 0)
        {
            return P2;
        }
        else if (P2.Health == 0)
        {
            return P1;
        }
        else
        {
            return null;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
