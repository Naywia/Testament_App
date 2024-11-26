namespace Testament_App.Models;

public static class Inheritance
{
    private static List<Asset> assets = new ();
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

    public static void DeleteTestator(Testator testator)
    {
        _testators.Remove(testator);
        FamilyTree.RemoveMember(testator);
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

    public static void DeleteHeir(Person heir)
    {
        _heirs.Remove(heir);
        FamilyTree.RemoveMember(heir);
    }
}