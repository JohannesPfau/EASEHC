
using System.Collections.Generic;

[System.Serializable]
public class PipelineInputData
{
    public string version;
    public string application;
    public string uuid;
    public string sentence;
    public Dictionary<string, string> parses = new Dictionary<string, string>();
    public string http_status;
    public jsonParses json_parses;
    public Dictionary<string, string> graphs = new Dictionary<string, string>();
    public string error;
}

public class jsonParses
{
    public sMain smain;
}

public class sMain
{
    public string __class__;
    public string name;
    public List<jRole> roles = new List<jRole>() { };
}

[System.Serializable]
public class jRole
{
    public string __class__;
    public string type;
    public jRole target;
    public string alternativeTarget;

    public static explicit operator jRole(string s)
    {
        jRole role = new jRole();
        role.alternativeTarget = s;
        return role;
    }
}

[System.Serializable]
public class jVariable
{
    public string __class__;
    public string name;
    public List<jRole> roles = new List<jRole>() { };
    public string type;
}

