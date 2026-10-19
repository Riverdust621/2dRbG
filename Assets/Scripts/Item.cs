using System;
using TMPro;
using UnityEngine;

public class item : MonoBehaviour
{
   public int ID;
   public PlayerControll playerControll;
   [SerializeField] private string Name;
   public int quantity = 1;
   public int usebookscount;
   public int startUseBooksCounts;

   private TMP_Text quantitytext;
   
   
   void Awake()
   {
      
      
      quantitytext = GetComponentInChildren<TMP_Text>();
      UpdateQuantityDisplay();
      GameObject player = GameObject.FindGameObjectWithTag("Player");

      // 2. Nur wenn das Objekt in der Szene existiert, holen wir die Komponente
      if (player != null)
      {
         playerControll = player.GetComponent<PlayerControll>();
      }
      else
      {
         player = null; // Falls kein Geschoss da ist, ist die Variable null
      }
      Name = gameObject.name.Replace("(Clone)", "").Trim();
      
   }
   public void AddToStack(int amount = 1)
   {
      quantity += amount;
      UpdateQuantityDisplay();
   }
   public int RemoveFromStack(int amount = 1)
   {
      int removed = Math.Min(amount, quantity);
      quantity -= removed;
      if(quantity == 0) Destroy(gameObject); HotbarController.Instance.HotbarUpdate();
      UpdateQuantityDisplay();
      return removed;
   }
   public void RemoveBookUse(int amount = 1)
   {
      usebookscount -= amount;
      if (usebookscount != 0) return;
      if(usebookscount == 0) RemoveFromStack(); 
      {
         if (name == "FireBook")
         {
            usebookscount = 10;
            
         }
         else
         {
            
            usebookscount = 3;
         }
      }
   }
   public GameObject CloneItem(int newQuantity)
   {
      GameObject clone = Instantiate(gameObject);
      item cloneitem = clone.GetComponent<item>();
      cloneitem.quantity = newQuantity;
      cloneitem.UpdateQuantityDisplay();
      return clone;
   }
   public void UpdateQuantityDisplay()
   {
      if (quantitytext != null)
      {
         quantitytext.text = quantity > 1 ? quantity.ToString() : "";
      }

   }

   public virtual void UseItem()
   {
      Debug.Log("UsingItem" + Name);
      playerControll.activeSpell = Name;
      PlayerControll.Instance.activeItem = this;


   }
}
