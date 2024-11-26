namespace Testament_App.Models;

public static class Inheritance
{
    //private List<Asset>
    private static List<Testator> _testators = new();
    private static List<Person> _heirs = new();
    //private static List<Beneficiary> _testators = new();

    public static void AddTestator(Testator testator)
    {
        _testators.Add(testator);
        FamilyTree.AddMember(testator);
    }

    public static Testator[] GetTestators()
    {
        return _testators.ToArray();
    }
    
    public static void AddHeir(Person heir)
    {
        _heirs.Add(heir);
        FamilyTree.AddMember(heir);
    }
    
    public static Person[] GetHeirs()
    {
        return _heirs.ToArray();
    }
}