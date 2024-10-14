using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    public ObjectMagnet objectMagnet;
    public void SpawnObjMagnet()
    {
        objectMagnet = SimplePool.Spawn<ObjectMagnet>(PoolType.ObjMagnet);
        objectMagnet.Oninit();
        objectMagnet.SetTransform();
        objectMagnet.IsRotate(true);
    }
}
