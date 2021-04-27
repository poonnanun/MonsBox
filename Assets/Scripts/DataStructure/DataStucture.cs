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
    public string material;
    public string model;

    public MonsterAsset(string material, string model)
    {
        this.material = material;
        this.model = model;
    }

    public override string ToString()
    {
        string output = "";
        output += "material: " + material + " | ";
        output += "model: " + model + " | ";
        return output;
    }
}

#endregion

#region Data sending
[System.Serializable]
public class MonsterCare
{
    public MonsterCareActivities activities;
    public int experience;

    public MonsterCare(MonsterCareActivities act, int experience)
    {
        this.activities = act;
        this.experience = experience;
    }
}
[System.Serializable]
public class MonsterCareActivities
{
    public string status;
    public int effect;
    
    public MonsterCareActivities(string status, int effect)
    {
        this.status = status;
        this.effect = effect;
    }
}
[System.Serializable]
public class CreatingMonster
{
    public string userId;
    public string name;
    public string asset;

    public CreatingMonster(string userId, string name, string asset)
    {
        this.userId = userId;
        this.name = name;
        this.asset = asset;
    }
}
[System.Serializable]
public class SignInForm
{
    public string username;
    public string password;

    public SignInForm(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
[System.Serializable]
public class SignInResponse
{
    public string id;
    public string username;
    public string token;

    public SignInResponse(string id, string username, string token)
    {
        this.id = id;
        this.username = username;
        this.token = token;
    }
}
#endregion
