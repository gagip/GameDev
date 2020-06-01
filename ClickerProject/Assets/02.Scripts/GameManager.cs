using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public static long money;

    public GameObject prefabCoffee;
    public GameObject prefabEmployee;
    public GameObject prefabTextMoney;

    // 왜 EmployeeControl.cs에 안하고 GameManager.cs에 저장을 하는가?
    // 이유는 간단하다. 데이터 효율성 때문인데, Employee가 여러 개가 생성된다면
    // Emplyee Sprite 3개도 같이 생성이 되어, Employee 의 3배가 된다.
    // 메모리 효율을 위해 싱글톤으로 저장이 된 GameManager에 저장하는게 효율적이다.
    public Sprite[] spriteF;
    public Sprite[] spriteM;

    public Vector2 limitPoint1;
    public Vector2 limitPoint2;

    public Text textMoney;

    public static string familyName
    {
        get
        {
            string[] names = new string[10];

            names[0] = "김";
            names[1] = "이";
            names[2] = "박";
            names[3] = "최";
            names[4] = "정";
            names[5] = "강";
            names[6] = "조";
            names[7] = "윤";
            names[8] = "장";
            names[9] = "임";

            int r = Random.Range(0, names.Length);
            string s = names[r];
            return s;
        }
    }
    public static string name
    {
        get
        {
            string[] names = new string[10];

            names[0] = "철수";
            names[1] = "영희";
            names[2] = "희동";
            names[3] = "티나";
            names[4] = "초홍";
            names[5] = "순희";
            names[6] = "춘자";
            names[7] = "희봉";
            names[8] = "영자";
            names[9] = "토리";

            int r = Random.Range(0, names.Length);
            string s = names[r];
            return s;
        }
    }

    public Sprite[] spriteButtonState;
    public Image imgButton;
    public bool isWhip;

    public GameObject panelinfo;

    public List<Employee> emps;

    private string savePath;

    public AudioClip clip;

    private void Awake()
    {
        gm = this;

        savePath = Application.persistentDataPath + "/save.sav";
    }

    // Start is called before the first frame update
    void Start()
    {
        emps = new List<Employee>();
        money = 10000;
        if (System.IO.File.Exists(savePath))
        {
            Load();
            foreach(var a in emps)
            {
                InitializeEmplyee(a);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //EarnMoney();
        // UI 위에서 마우스(터치)가 이루어지지 않을 때
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if (isWhip == true)
            {
                EarnMoney();
            }
            else
            {
                CreateCoffee();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<AudioSource>().PlayOneShot(clip, 1.0f);
        }
        
        ChangeButtonSprite();
        ShowMoneyText();

        if(GameObject.Find("Panel_Option") != null) // 옵션패널이 열렀을 때
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    // 화면을 클릭했을 때 돈을 얻는 함수
    public void EarnMoney()
    {
        if (Input.GetMouseButtonDown(0))
        {
            money += 1;
        }
    }

    // 커피를 만드는 함수
    void CreateCoffee()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼을 클릭하면
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 게임 오브젝트 생성
            Instantiate(prefabCoffee, mousePosition, Quaternion.identity);
        }
    }

    void ShowMoneyText()
    {
        textMoney.text = money.ToString("###,###");
    }

    public void ChangeButtonState()
    {
        if (isWhip == true)
            isWhip = false;
        else
            isWhip = true;
    }

    void ChangeButtonSprite()
    {
        if (isWhip == true)
        {
            imgButton.sprite = spriteButtonState[0]; // 채찍
            imgButton.rectTransform.sizeDelta = new Vector2(180, 180);
        }
 
        else
        {
            imgButton.sprite = spriteButtonState[1]; // 커피
            imgButton.rectTransform.sizeDelta = new Vector2(110, 180);
        }
            
    }

    public void PanelHireOpen()
    {
        panelinfo.SetActive(true);

        Employee e = RandomCreateEmployee();

        var textName = panelinfo.transform.Find("Text_Name").GetComponent<Text>();
        var imageCharacter = panelinfo.transform.Find("Image_Character").GetComponent<Image>();
        var textProgramming = panelinfo.transform.Find("Text_Programming").GetComponent<Text>();
        var textArt = panelinfo.transform.Find("Text_Art").GetComponent<Text>();
        var textDesign = panelinfo.transform.Find("Text_Design").GetComponent<Text>();
        var textSound = panelinfo.transform.Find("Text_Sound").GetComponent<Text>();
        var textSalery = panelinfo.transform.Find("Button_Hire/Image_Price/Text_Price").GetComponent<Text>();
        var buttonHire = panelinfo.transform.Find("Button_Hire").GetComponent<Button>();

        textName.text = e.name;
        if(e.gender == Gender.Female)
        {
            imageCharacter.sprite = spriteF[0];
        }
        else
        {
            imageCharacter.sprite = spriteM[0];
        }

        textProgramming.text = e.programming.ToString();
        //textDesign = e.design.ToString();
        textSound.text = e.sound.ToString();
        textArt.text = e.art.ToString();
        textSalery.text = e.salery.ToString();

        buttonHire.onClick.RemoveAllListeners();
        buttonHire.onClick.AddListener(delegate { Hire((int)e.salery, e); });
        
    }

    Employee RandomCreateEmployee()
    {
        Employee info = new Employee();

        info.name = GameManager.familyName + GameManager.name;
        info.hp = 100;

        info.sound = Random.Range(0, 101);
        info.programming = Random.Range(0, 101);
        info.design = Random.Range(0, 101);
        info.art = Random.Range(0, 101);

        info.salery = Random.Range(100, 1001);

        int r = Random.Range(0, 2);
        info.gender = (Gender)r;

        return info;
    }

    public void CreateEmplyee(Employee e)
    {
        GameObject obj = Instantiate(prefabEmployee, Vector3.zero, Quaternion.identity);
        obj.GetComponent<EmployeeControl>().info = e;
    }

    public void InitializeEmplyee(Employee e)
    {
        GameObject obj = Instantiate(prefabEmployee, Vector3.zero, Quaternion.identity);
        obj.GetComponent<EmployeeControl>().info = e;
    }

    public void Hire(int price, Employee e)
    {
        if(money >= price)
        {
            CreateEmplyee(e);
            money -= price;
            emps.Add(e);
        }
    }

    public void Save()
    {
        //PlayerPrefs.SetInt("MONEY",(int)money);
        //PlayerPrefs.SetString("MONEY", money.ToString());
        SaveData sd = new SaveData();
        sd.money = money;
        sd.empList = emps;
        XmlManager.XmlSave<SaveData>(sd, savePath);
    }

    public void Load()
    {
        //money = PlayerPrefs.GetInt("MONEY", 1000); // 디폴트 값을 설정할 수 있다. 저장 안 되어 있는데 불러오면 오류나니깐
        //money = long.Parse(PlayerPrefs.GetString("MONEY"));

        SaveData sd = XmlManager.XmlLoad<SaveData>(savePath);
        money = sd.money;
        emps = sd.empList;
    }

    private void OnApplicationQuit()
    {
        Save();
        Application.Quit();
    }

    private void OnDrawGizmos()
    {
        Vector2 limitPoint3 = new Vector2(limitPoint2.x, limitPoint1.y);
        Vector2 limitPoint4 = new Vector2(limitPoint1.x, limitPoint2.y);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(limitPoint1, limitPoint3);
        Gizmos.DrawLine(limitPoint3, limitPoint2);
        Gizmos.DrawLine(limitPoint1, limitPoint4);
        Gizmos.DrawLine(limitPoint4, limitPoint2);
    }

    // 코드로 구현하고 싶은 경우
    public void VolumeChange(Slider sl)
    {
        GetComponent<AudioSource>().volume = sl.value;
    }

    public void SaveDelete()
    {
        if (System.IO.File.Exists(savePath))
        {
            System.IO.File.Delete(savePath);
        }
    }

    public void IapTest()
    {
        money += 10000;
    }
}

[System.Serializable]
public class SaveData
{
    public long money;

    public List<Employee> empList;
}
