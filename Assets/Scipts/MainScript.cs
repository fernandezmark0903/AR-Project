using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Madhur.InfoPopup;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    public materialClass[] materialList;
    private bool checkDetection = false;
    public GameObject captureButton;
    public GameObject inventoryPanel;
    public GameObject boxModel;
    public GameObject popUp;
    public TextAsset materialsXML;
    private string path = "";

    
    //detect if item is detected by the camera
    public void checkDetector(bool detector)
    {
        checkDetection = detector;
    }

    //show and hide capture button
    private void Update()
    {
        if (checkDetection && !inventoryPanel.activeSelf)
            captureButton.SetActive(true);
        else
            captureButton.SetActive(false);
    }

    //open inventory system
    public void openInventory()
    {
        loadXML();

        captureButton.SetActive(false);
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    //generating XML
    private void Start()
    {
        path = Application.persistentDataPath + "/materialXML.xml";

        if(!File.Exists(path))
            generateXML();
        
    }

    //get xml materials
    private void loadXML()
    {
        int loop = 0;
        string xmlData = materialsXML.text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        string xmlPathPattern = "//items/materials";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);

        foreach (XmlNode node in myNodeList)
        {
            XmlNode name = node.FirstChild;
            XmlNode value = name.NextSibling;
            materialList[loop].name.text = name.InnerXml;
            materialList[loop].value.text = value.InnerXml;
            loop++;
        }
    }

    //capture target and set xml materials
    private void captureTarget(int[] material)
    {
        loadXML();
        int loop = 0;
        string xmlData = materialsXML.text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlData));
        string xmlPathPattern = "//items/materials";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);
        foreach (XmlNode node in myNodeList)
        {
            XmlNode name = node.FirstChild;
            XmlNode value = name.NextSibling;
            value.InnerText = ((int.Parse(materialList[loop].value.text)) + material[loop]).ToString();
            loop++;
        }
        xmlDoc.Save(path);
    }

    public void dropRandomMaterial()
    {
        int[] materialValue = new int[3];

        int randomizer = Random.Range(1, 4);

        Debug.Log("RANDOMIZER: " + randomizer);

        string[] dropMaterial = new string[randomizer];

        for (var i = 0; i < randomizer; i++)
        {
            int randomMaterial = Random.Range(0, materialList.Length);
            dropMaterial[i] = materialList[randomMaterial].materialName;
        }

        string materials = "";

        foreach (string item in dropMaterial)
        {
            Debug.Log("ITEM: " + item);
            if (item == "Plastic")
            {
                materialValue[0]++;
            }
            else if (item == "Wood")
            {
                materialValue[1]++;
            }
            else if (item == "Metal")
            {
                materialValue[2]++;
            }
        }

        materials = "Plastic " + materialValue[0] + "\nWood " + materialValue[1] + "\nMetal " + materialValue[2];

        string plastic = "Plastic 0";
        string wood = "\nWood 0";
        string metal = "\nMetal 0";

        string[] trimMaterials = new string[3];
        trimMaterials[0] = "Plastic 0";
        trimMaterials[1] = "\nWood 0";
        trimMaterials[2] = "\nMetal 0";

        foreach (string item in trimMaterials)
        {
            materials = materials.Replace(item, "");
        }


        popUp.GetComponent<InfoPopupUtil>().ShowInformation(materials);
        captureTarget(materialValue);


    }

    private void generateXML()
    {
        int loop = 0;
        string xmlData = materialsXML.text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlData));
        string xmlPathPattern = "//items/materials";
        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);
        foreach (XmlNode node in myNodeList)
        {
            XmlNode name = node.FirstChild;
            XmlNode value = name.NextSibling;
            name.InnerText = materialList[loop].materialName;
            value.InnerText = "0";
            loop++;
        }
        xmlDoc.Save(path);

    }




    [System.Serializable]
    public class materialClass
    {
        public string materialName;
        public int materialCount;
        public TextMeshProUGUI name;
        public TextMeshProUGUI value;
    }


}
