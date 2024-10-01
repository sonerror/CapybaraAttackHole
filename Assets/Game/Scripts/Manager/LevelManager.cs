using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Player playerPrefabs;
    [SerializeField] private Transform tfPlayer;
    [SerializeField] private LevelDatas levelData;
    public Player player;

    public void Oninit()
    {
       CameraManager.Ins.Oninit();
        UIManager.Ins.GetUI<UIGamePlay>().ReLoadUIFollow();
    }
    public void OnLoadLevel()
    {
        if(playerPrefabs != null)
        {
            player = Instantiate(playerPrefabs, tfPlayer);
            player.transform.position = new Vector3(0,0,0);
            player.ChangeScale(0.1f);
            player.ChangeSpeed(1.2f);
            Oninit();
            player.GetDataLevel(levelData.GetDataWithID(DataManager.Ins.playerData.levelCurrent).checkPoints);
        }
    }
}
