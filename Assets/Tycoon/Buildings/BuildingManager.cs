using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

namespace Tycoon {
    public class Buildings
    {
        public string timestamp { get; set; }
        public List<BuildingData> buildings { get; set; }
    }
    public class BuildingData
    {
        public string key { get; set; }
        public string type { get; set; }
        public float width { get; set; }
        public float length { get; set; }
        public float rotation { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class BuildingManager : MonoBehaviour
    {

        public GameObject Gym;
        public GameObject Academic;
        public GameObject Refectory;
        public GameObject Dorm;

        public string url = "http://127.0.0.1:59939/Assets/Tycoon/TestJSON/test.json";

        public Dictionary<string, BuildingData> buildingDataMap;
        public Dictionary<string, GameObject> buildingObjectMap;
        private List<string> removedBuildingKeys;
        private Dictionary<string, GameObject> buildingTypeMap;

		public float XScaleRatio = 500;
		public float YScaleRatio = 400;

        private string lastUpdated = "";

        // Use this for initialization
        void Start()
        {
            buildingTypeMap = new Dictionary<string, GameObject>()
            {
                { "dorm", Dorm },
                { "refectory", Refectory },
                { "academic", Academic },
                { "gym", Gym }
            };
            buildingDataMap = new Dictionary<string, BuildingData>();
            buildingObjectMap = new Dictionary<string, GameObject>();
            removedBuildingKeys = new List<string>();
            StartCoroutine(UpdateJson());
        }

        void UpdateBuildings()
        {
            foreach (KeyValuePair<string, BuildingData> kvpair in buildingDataMap)
            {
                BuildingData building = kvpair.Value;
                if (buildingObjectMap.ContainsKey(kvpair.Key))
                {
                    GameObject buildingObject = buildingObjectMap[kvpair.Key];
					buildingObject.transform.position = new Vector3(building.x * XScaleRatio, 0, building.y * -YScaleRatio);
                    buildingObject.transform.rotation = Quaternion.AngleAxis(building.rotation, Vector3.up);
                }
                else
                {
					GameObject buildingObject = Instantiate(buildingTypeMap[building.type], new Vector3(building.x * XScaleRatio, 0, building.y * -YScaleRatio), Quaternion.AngleAxis(building.rotation, Vector3.up)) as GameObject;
                    buildingObject.transform.SetParent(this.transform);
                    buildingObjectMap[kvpair.Key] = buildingObject;
                }
            }
            foreach (string key in removedBuildingKeys)
            {
                Destroy(buildingObjectMap[key]);
                buildingObjectMap.Remove(key);
            }
			removedBuildingKeys.Clear ();
        }

        void ReadJson(string jsonString)
        {
            
            Buildings buildingsWrapper = JsonConvert.DeserializeObject<Buildings>(jsonString);
            List<BuildingData> buildingDatas = buildingsWrapper.buildings;

			if (!buildingsWrapper.timestamp.Equals(lastUpdated))
            {
                lastUpdated = buildingsWrapper.timestamp;
                Dictionary<string, BuildingData> newBuildingDataMap = new Dictionary<string, BuildingData>();
                foreach (BuildingData building in buildingDatas)
                {
                    string key = building.key;
                    newBuildingDataMap[key] = building;
                }
                foreach (KeyValuePair<string, BuildingData> kvpair in buildingDataMap)
                {
                    if (!newBuildingDataMap.ContainsKey(kvpair.Key))
                    {
                        removedBuildingKeys.Add(kvpair.Key);
                    }
                }
                buildingDataMap = newBuildingDataMap;
            }

            UpdateBuildings();
        }

        IEnumerator UpdateJson()
        {
            while(true)
            {
                WWW www = new WWW(url);
                yield return www;
                if (www.error == null)
                {
                    ReadJson(www.text);
                }
                else
                {
                    Debug.LogError(www.error);
                }

				yield return new WaitForSeconds (1/60f);
            }
        }
    }

}

