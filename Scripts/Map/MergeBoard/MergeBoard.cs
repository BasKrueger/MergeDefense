using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MergeBoard : MonoBehaviour
{
    private MergeUI ui;
    private MergeTiles tiles;
    private PreviewTile preview;

    [SerializeField, AssetsOnly, Required]
    private Coin coinTemplate;
    [SerializeField, AssetsOnly, Required]
    private List<Turret> turretTemplates;

    [SerializeField, FoldoutGroup("Settings")]
    private int coins = 0;

    [SerializeField, FoldoutGroup("Settings")]
    private int turretCost = 1;
    [SerializeField, FoldoutGroup("Settings")]
    private int turretLevel = 1;
    [SerializeField, FoldoutGroup("Settings")]
    private int levelUpCost = 10;

    private Turret nextTurret;

    private void Awake()
    {
        ui = GetComponentInChildren<MergeUI>();
        tiles = GetComponentInChildren<MergeTiles>();
        preview = GetComponentInChildren<PreviewTile>();
    }
    private void Start()
    {
        ui.SetTurretCost(turretCost);
        ui.SetTurretLevel(turretLevel);
        ui.SetLevelUpCost(levelUpCost);
        ui.UpdateCoinCounterValue(coins);

        nextTurret = turretTemplates[Random.Range(0, turretTemplates.Count)];
        preview.ShowcaseTurret(nextTurret);
        tiles.SetUp();
    }

    public void SpawnTurret()
    {
        if (tiles.CanSpawnTurret() && SpendCoins(turretCost))
        {
            tiles.TrySpawnTurret(nextTurret, turretLevel);
            nextTurret = turretTemplates[Random.Range(0, turretTemplates.Count)];
            preview.ShowcaseTurret(nextTurret);
        }
    }

    public void LevelUp()
    {
        if (SpendCoins(levelUpCost))
        {
            turretLevel++;
            levelUpCost *= 2;
            turretCost = Mathf.RoundToInt(turretCost * 1.75f);

            ui.SetLevelUpCost(levelUpCost);
            ui.SetTurretLevel(turretLevel);
            ui.SetTurretCost(turretCost);

            StartCoroutine(delay());
            IEnumerator delay()
            {
                for (int i = 0; i < levelUpCost / 5; i++)
                {
                    yield return new WaitForSeconds(0.1f);

                    Coin activeCoin = Instantiate(coinTemplate);
                    activeCoin.Setup(ui.coinCounterTarget.position);

                    StartCoroutine(activeCoin.Lerp(ui.coinUpgradeTarget[2].position, 500, ui.coinUpgradeTarget[0].position, ui.coinUpgradeTarget[1].position));
                }
            }
        }
    }

    public void AddCoin(Coin coin)
    {
        StartCoroutine(delay());
        IEnumerator delay()
        {
            ui.SetCoinCounterGlow(FindObjectsOfType<Coin>().Length > 0);

            yield return StartCoroutine(coin.Lerp(ui.coinCounterTarget.position, 500f));

            coins += coin.value;

            ui.SetCoinCounterGlow(FindObjectsOfType<Coin>().Length > 0);
            ui.BlinkCoinCounter();
            ui.UpdateCoinCounterValue(coins);

            yield return null;
        }
    }

    private bool SpendCoins(int value)
    {
        if (value > coins)
        {
            return false;
        }

        coins -= value;
        ui.UpdateCoinCounterValue(coins);

        return true;
    }

}
