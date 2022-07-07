using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Person SSeries = new Person();
    Person Frog = new Person();
    private void Start() {

        Person.type = "사람";
    }
}

public class Person{
    public static string type;
    public string getType(){
        return type;
    }
    public void setType(string _type){
        type = _type;
    }
}