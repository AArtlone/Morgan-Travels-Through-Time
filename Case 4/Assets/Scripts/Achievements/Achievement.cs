public class Achievement
{
    public string Name { get; set; }
    public string NameDutch { get; set; }
    public string Icon { get; set; }
    public bool Status { get; set; }

    public Achievement(string name, string nameDutch, string icon, bool status)
    {
        Name = name;
        NameDutch = nameDutch;
        Icon = icon;
        Status = status;
    }
}
