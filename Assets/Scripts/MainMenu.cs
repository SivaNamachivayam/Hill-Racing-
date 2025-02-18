using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;

public class MainMenu : MonoBehaviourPun {

    public GameObject scrollView, scrollbar, purchaseUI;

    public GameObject[] Contents, Stages, Vehicles;

    public GameObject[] content;

    public Text moneyText, cantBuyText;


    public float scroll_pos = 0, distance;
    public float[] pos;

    public int selectedMenuIndex, selectedIndex;
    public bool changeIndex = true, start = true;

    public static MainMenu Data;
    public GameObject LoadingPanel;
    public void Awake()
    {
        Data = this;
    }

    public void OnEnable()
    {
        if (OnlyData.Data)
        {
            if (OnlyData.Data.AlreadyPlayedGames)
            {
                LoadingPanel.SetActive(false);
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1.0";
            }
        }
        
    }
    private void Start() {
      
        if(!PlayerPrefs.HasKey("Stage")) {
            PlayerPrefs.SetInt("Stage", 0);
            PlayerPrefs.SetInt("Vehicle", 0);

            PlayerPrefs.SetInt("Stage_0", 0);
            PlayerPrefs.SetInt("Stage_1", 0);
            PlayerPrefs.SetInt("Stage_2", 0);
            PlayerPrefs.SetInt("Stage_3", 0);

            PlayerPrefs.SetInt("Bike", 0);
            PlayerPrefs.SetInt("SuperCar1", 0);
            PlayerPrefs.SetInt("SuperCar2", 0);
            PlayerPrefs.SetInt("SuperCar3", 0);
            PlayerPrefs.SetInt("Money", 500000);
        }
        LoadData();
        MenuChange(1);  
        start = false;
    }

    
    private void LoadData() {

        Stages[1].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Stage_1").Equals(0));
        Stages[1].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Stage_1").Equals(0);
        Stages[2].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Stage_2").Equals(0));
        Stages[2].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Stage_2").Equals(0);
        Stages[3].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Stage_3").Equals(0));
        Stages[3].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Stage_3").Equals(0);


        Vehicles[1].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Bike").Equals(0));
        Vehicles[1].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Bike").Equals(0);
        Vehicles[2].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("SuperCar2").Equals(0));
        Vehicles[2].GetComponent<Button>().enabled = PlayerPrefs.GetInt("SuperCar2").Equals(0);
        Vehicles[3].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("SuperCar3").Equals(0));
        Vehicles[3].GetComponent<Button>().enabled = PlayerPrefs.GetInt("SuperCar3").Equals(0);

        moneyText.text = PlayerPrefs.GetInt("Money").ToString();
    }

    private void Update() {
        
        if(Input.GetMouseButton(0)) {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else {
            for(int i = 0; i < pos.Length; i++) {
                if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    selectedIndex = i;
                }
            }
            changeIndex = true;
        }

        
        for(int i = 0; i < pos.Length; i++) {
            if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) {
                content[i].transform.localScale = Vector2.Lerp(content[i].transform.localScale, new Vector2(1.2f, 1.2f), 0.1f);
                for(int j = 0; j < pos.Length; j++)
                    if(j != i)
                        content[j].transform.localScale = Vector2.Lerp(content[j].transform.localScale, new Vector2(0.8f, 0.8f), 0.1f);

                if(changeIndex) {  
                    SaveSelectedData(i);
                    changeIndex = false;
                }
            }
        }
    }

    
    public void MenuChange(int index) {
        
        if(!CheckPurchased() && !start) {
            if(!(selectedMenuIndex == 0 && selectedIndex > 1 || selectedMenuIndex == 1 && selectedIndex > 1)) {
                purchaseUI.SetActive(true);
                return;
            }
        }    
        selectedMenuIndex = index;  

        pos = new float[Contents[index].transform.childCount];
        distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++)
            pos[i] = distance * i;

        if(index.Equals(0)) { 
            content = Stages;
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value = pos[PlayerPrefs.GetInt("Stage")];
        }
        else if(index.Equals(1)) {  
            content = Vehicles;
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value = pos[PlayerPrefs.GetInt("Vehicle")];
        }

        foreach(var obj in Contents)
            obj.SetActive(false);
        Contents[index].SetActive(true);
        scrollView.GetComponent<ScrollRect>().content = Contents[index].GetComponent<RectTransform>();
    }

    public void Purchase() {
        int price, moneyOwned = 500000;
        if(Contents[0].activeSelf) {
            Debug.Log("STAGE SELECT");
            price = int.Parse(Stages[selectedIndex].transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<Text>().text);
            if(moneyOwned - price < 0) { cantBuyText.GetComponent<Animator>().SetTrigger("warning"); return; }
            //if(selectedIndex.Equals(1)) PlayerPrefs.SetInt("Stage_Mars", 1);

            switch (selectedIndex)
            {
                case 0:
                    PlayerPrefs.SetInt("Stage_0", 0);
                    break;
                case 1:
                    PlayerPrefs.SetInt("Stage_1", 1);
                    break;
                case 2:
                    PlayerPrefs.SetInt("Stage_2", 2);
                    break;
                case 3:
                    PlayerPrefs.SetInt("Stage_3", 3);
                    break;
                default:
                    break;
            }
            PlayerPrefs.SetInt("Money", moneyOwned - price);
            LoadData();
        }
        else if(Contents[1].activeSelf){
            Debug.Log("VACHILE SELECT");
            price = int.Parse(Vehicles[selectedIndex].transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<Text>().text);
            if(moneyOwned - price < 0) { cantBuyText.GetComponent<Animator>().SetTrigger("warning"); return; }

            switch (selectedIndex)
            {
                case 0:
                    PlayerPrefs.SetInt("SuperCar1", 0);
                    break;
                case 1:
                    PlayerPrefs.SetInt("Bike", 1);
                    break;
                case 2:
                    PlayerPrefs.SetInt("SuperCar2", 2);
                    break;
                case 3:
                    PlayerPrefs.SetInt("SuperCar3", 3);
                    break;
                default:
                    break;
            }
            PlayerPrefs.SetInt("Money", moneyOwned - price);
            LoadData();
        }
       
    }

    
    private bool CheckPurchased() {
        if(selectedMenuIndex.Equals(0)) {
            if(selectedIndex != 0)
                return !Stages[selectedIndex].transform.GetChild(1).gameObject.activeSelf;
        }
        else { 
            if(selectedIndex != 0) 
                return !Vehicles[selectedIndex].transform.GetChild(1).gameObject.activeSelf;
        }
        return true;
    }

   
    private void SaveSelectedData(int index) {
        if(selectedMenuIndex.Equals(0)) {
            if(!CheckPurchased()) return; 
            PlayerPrefs.SetInt("Stage", index);
            RoomCreationManager.data.MapName = PlayerPrefs.GetInt("Stage").ToString();
            RoomCreationManager.data.isLobbyJoined = false;
        }
        else {
            if(!CheckPurchased()) return; 
            PlayerPrefs.SetInt("Vehicle", index);
        }
    }

    
    public void GameStart() {
        if(!CheckPurchased()) {
            if(!(selectedMenuIndex == 0 && selectedIndex > 1 || selectedMenuIndex == 1 && selectedIndex > 1)) {
                purchaseUI.SetActive(true);
                return;
            }
        }
        //LoadingPanel.GetComponent<Animator>().SetTrigger("FadeOut");
        //LoadingPanel.SetActive(true);
        //SceneManager.LoadScene(1);
        if (OnlyData.Data.gametype == GameType.pass)
        {
            LoadingPanel.SetActive(true);
            //StartCoroutine(LoadingScreen());
            SceneManager.LoadScene(1);
        }
            
    }

   /* IEnumerator LoadingScreen()
    {
        LoadingPanel.SetActive(true);
        yield return new WaitForSeconds(0.3f);
    }*/
}