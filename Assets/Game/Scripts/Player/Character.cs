using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : GameUnit
{
    [SerializeField] public List<Character> listTarget = new List<Character>();
    public float score = 1;
    public bool isDead { get; set; }
    private void Start()
    {
        listTarget.Clear();
    }
    public virtual void OnInit()
    {
        isDead = false;
    }
    public virtual void AddTarget(Character character)
    {
        this.listTarget.Add(character);

    }
    public virtual void RemoveTarget(Character character)
    {
        this.listTarget.Remove(character);
    }

    public void ChangeScale(float scale)
    {
        this.transform.localScale = this.transform.localScale * scale;
    }
  
}
