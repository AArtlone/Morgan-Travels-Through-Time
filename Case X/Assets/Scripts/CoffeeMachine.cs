using UnityEngine;
using LitJson;
using System.IO;
using System;

public class CoffeeMachine : MonoBehaviour
{
    private DateTime _dateUntilNewCoffee;
    public GameObject CoffeeErrorPopUp;

    public void GetLastDateOfCoffee()
    {
        string dataFromJson = File.ReadAllText(Application.persistentDataPath + "/Player.json");
        JsonData playerData = JsonMapper.ToObject(dataFromJson);

        DateTime dateOfLastCoffee = DateTime.Parse(playerData["DateOfLastCoffee"].ToString());
        // The player can drink between 12 and 14 pm so there is a two hour offset
        // after the player has a coffee.
        _dateUntilNewCoffee = dateOfLastCoffee.AddHours(20);
    }

    public void DrinkCoffee()
    {
        GetLastDateOfCoffee();

        //Debug.Log("Now: " + DateTime.Now + "New: " + _dateUntilNewCoffee);
        if (DateTime.Now > _dateUntilNewCoffee)
        {
            //Debug.Log("Had coffee");
            Character.Instance.DateOfLastCoffee = DateTime.Now.ToString();
            Character.Instance.RefreshJsonData();
            _dateUntilNewCoffee.AddHours(20);
        }
    }

    public void CheckForError()
    {
        GetLastDateOfCoffee();

        if (DateTime.Now < _dateUntilNewCoffee)
        {
            CoffeeErrorPopUp.SetActive(true);
        }
    }
}
