using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class XMLParsing : MonoBehaviour 
{
	public TextMesh fileDataTextbox;
	private string path;
	private string fileInfo;
	private XmlDocument xmlDoc;
	private WWW www;
	private TextAsset textXml;
	private List<Player> Players;
	private string fileName;

	// Structure for mainitaing the player information
	struct Player
	{
		public int Id;
		public string name;
		public int score;
	};

	void Awake()
	{
		fileName = "DemoXmlFile";
		Players = new List<Player>(); // initalize player list
		fileDataTextbox.text = "";
	}
	
	void Start ()
	{
		loadXMLFromAssest();
		readXml();
	}
	// Following method load xml file from resouces folder under Assets
	private void loadXMLFromAssest()
	{
		xmlDoc = new XmlDocument();
		if(System.IO.File.Exists(getPath()))
		{
			xmlDoc.LoadXml(System.IO.File.ReadAllText(getPath()));
		}
		else
		{
			textXml = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
			xmlDoc.LoadXml(textXml.text);
		}
	}
	
	void Update()
	{
		// Following code just check whether any button is touched on mouse down
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit) && hit.collider != null)
			{
				if(hit.collider.name.Equals("ModifyXmlButton"))
					modifyXml();
				else if(hit.collider.name.Equals("CreateElementButton"))
					createElement();
			}
		}
	}
	// Following method reads the xml file and display its content 
	private void readXml()
	{
		foreach(XmlElement node in xmlDoc.SelectNodes("Players/Player"))
		{
			Player tempPlayer = new Player();
			tempPlayer.Id = int.Parse(node.GetAttribute("id"));
			tempPlayer.name = node.SelectSingleNode("name").InnerText;
			tempPlayer.score = int.Parse(node.SelectSingleNode("score").InnerText);
			Players.Add(tempPlayer);
			displayPlayeData(tempPlayer);
		}
	}
	private void displayPlayeData(Player tempPlayer)
	{
		fileDataTextbox.text += tempPlayer.Id + "\t\t" + tempPlayer.name + "\t\t" + tempPlayer.score + "\n";
	}

	// Following method will be called by tapping ModifyXml button
	// It just multiply player's score by 10
	private void modifyXml()
	{
		fileDataTextbox.text = "";
		foreach(XmlElement node in xmlDoc.SelectNodes("Players/Player"))
		{
			int nodeId = int.Parse(node.GetAttribute("id"));
			Player tempPlayer = Players[nodeId-1];
			tempPlayer.score *= 10;
			node.SelectSingleNode("score").InnerText = tempPlayer.score + "";
			displayPlayeData(tempPlayer);
		}
		xmlDoc.Save(getPath()+".xml");
	}

	// Following method create new element of player
	private void createElement()
	{
		Player tempPlayer = new Player();
		tempPlayer.Id = Players.Count+1;
		tempPlayer.name = "Player"+tempPlayer.Id;
		tempPlayer.score = tempPlayer.Id*10;
		Players.Add(tempPlayer);
		displayPlayeData(tempPlayer);

		XmlNode parentNode = xmlDoc.SelectSingleNode("Players");
		XmlElement element = xmlDoc.CreateElement("Player");
		element.SetAttribute("id",tempPlayer.Id.ToString());
		element.AppendChild(createNodeByName("name",tempPlayer.name));
		element.AppendChild(createNodeByName("score",tempPlayer.score.ToString()));
		parentNode.AppendChild(element);
		xmlDoc.Save(getPath()+".xml");
	}
	private XmlNode createNodeByName(string name, string innerText)
	{
		XmlNode node = xmlDoc.CreateElement(name);
		node.InnerText = innerText;
		return node;
	}

	// Following method is used to retrive the relative path as device platform
	private string getPath(){
		#if UNITY_EDITOR
		return Application.dataPath +"/Resources/"+fileName;
		#elif UNITY_ANDROID
		return Application.persistentDataPath+fileName;
		#elif UNITY_IPHONE
		return GetiPhoneDocumentsPath()+"/"+fileName;
		#else
		return Application.dataPath +"/"+ fileName;
		#endif
	}
	private string GetiPhoneDocumentsPath()
	{
		// Strip "/Data" from path 
		string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
		// Strip application name 
		path = path.Substring(0, path.LastIndexOf('/'));
		return path + "/Documents";
	}
}