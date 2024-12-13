namespace Testament_App.Models;

public class Testator : Person
{
    private List<Asset> separateAssets = new();

    public Testator()
    {
        
    }

    public void AddAsset(Asset asset)
    {
        separateAssets.Add(asset);
    }

    public List<Asset> GetAssets()
    {
        return separateAssets;
    }
}