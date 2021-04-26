using System.Collections;
using System.Collections.Generic;
using System.Linq;

#region Monster
[System.Serializable]
public class MonsterPool
{
    public int size;
    public List<MonsterRawData> data;
    public int GetSize()
    {
        return size;
    }
    public List<MonsterRawData> GetAllMonsters()
    {
        return data;
    }
    public MonsterRawData GetMonsterById(string id)
    {
        MonsterRawData a = data.Where(b => b.id == id).FirstOrDefault();
        return a;
    }
    public MonsterRawData GetMonsterByNumber(int number)
    {
        MonsterRawData tmp = null;
        if (number < data.Count())
        {
            tmp = data[number];
        }
        return tmp;
    }
    public override string ToString()
    {
        string output = "";
        output += "------------------------\n";
        output += data[0].ToString() + "\n";
        //foreach (MonsterRawData a in data)
        //{
        //    output += a.ToString() + "\n";
        //}
        return output;
    }
}

[System.Serializable]
public class MonsterRawData
{
    public string id;
    public string userId;
    public string name;
    public string asset;
    public MonsterStausRawData status;
    public string tier;

    public override string ToString()
    {
        string output = "";
        output += "id: " + id + " | ";
        output += "userId: " + userId + " | ";
        output += "name: " + name + " | ";
        output += "asset: " + asset + " | ";
        output += "tier: " + tier + " | ";
        output += "Status{ " + status + " } ";
        return output;
    }
}
[System.Serializable]
public class MonsterStausRawData
{
    public int hungry;
    public int cleanliness;
    public int happiness;
    public int weight;
    public int lastActivityTimestamp;

    public override string ToString()
    {
        string output = "";
        output += "Hungry: " + hungry + " | ";
        output += "cleanliness: " + cleanliness + " | ";
        output += "happiness: " + happiness + " | ";
        output += "weight: " + weight + " | ";
        output += "lastActivityTimestamp: " + lastActivityTimestamp + " | ";
        return output;
    }
}

[System.Serializable]
public class MonsterAsset
{
    public string color;
    public string material;
    public string model;
    public override string ToString()
    {
        string output = "";
        output += "color: " + color + " | ";
        output += "material: " + material + " | ";
        output += "model: " + model + " | ";
        return output;
    }
}

#endregion
