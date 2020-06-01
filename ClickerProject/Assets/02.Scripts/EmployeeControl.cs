using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeControl : MonoBehaviour
{
    SpriteRenderer spr;

    public Employee info;
    // 이름, 성별, 능력치(기획, 프로그래밍, 디자인, 사운드), 월급, 체력

    public float speed;

    public Vector2 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();


        // SetInfo()는 이제 맨 처음 한 번만 사용하기 때문에
        if (string.IsNullOrEmpty(info.name))
        {
            SetInfo();
        }

        StartCoroutine(EarnMoneyAuto());
        StartCoroutine(HpDecreaseAuto());
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        SpriteChangae();
        ShowInfo();
    }

    IEnumerator EarnMoneyAuto()
    {
        while (true)
        {
            GameManager.money += 1;
            ShowTextMoney(1);
            yield return new WaitForSeconds(1.0f);
        }
    }

    void ShowTextMoney(int m)
    {
        GameObject obj = Instantiate(GameManager.gm.prefabTextMoney,
            transform.Find("Canvas"), false); // false = 월드스페이스 공간에 넣지 않겠다

        var anim = obj.GetComponent<Animator>();
        anim.SetTrigger("Start");

        Text txt = obj.GetComponent<Text>();
        txt.text = "+ " + m.ToString("###,###");
        Destroy(obj, 3f); // 3초 뒤 없애기
    }

    IEnumerator HpDecreaseAuto()
    {
        while (true)
        {
            info.hp -= 1;
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator Move()
    {
        while (true)
        {
            float x = transform.position.x + Random.Range(-2f, 2f);
            float y = transform.position.y + Random.Range(-2f, 2f);

            Vector2 target = new Vector2(x, y);
            target = CheckTraget(target);

            prevPosition = transform.position;
            
            while (Vector2.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                target, speed);
                yield return null;
            }
            

            yield return new WaitForSeconds(1.0f);
        }
    }

    Vector2 CheckTraget(Vector2 currentTarget)
    {
        Vector2 temp = currentTarget;

        // 위치 수정
        if(currentTarget.x < GameManager.gm.limitPoint1.x) // 왼쪽으로 너무 치우치는 경우
        {
            temp = new Vector2(currentTarget.x + 4, temp.y);
        }
        else if(currentTarget.x > GameManager.gm.limitPoint2.x) // 오른쪽
        {
            temp = new Vector2(currentTarget.x - 4, temp.y);
        }

        if (currentTarget.y > GameManager.gm.limitPoint1.y) // 위
        {
            temp = new Vector2(temp.x, currentTarget.y - 4);
        }
        else if(currentTarget.y < GameManager.gm.limitPoint2.y) // 아래
        {
            temp = new Vector2(temp.x, currentTarget.y + 4);
        }

        return temp;
    }

    void SpriteChangae()
    {
        Sprite[] set = null; // 0 = 정면, 1 = 후면, 2 = 측면

        if (info.gender == Gender.Female)
            set = GameManager.gm.spriteF;
        else
            set = GameManager.gm.spriteM;

        //현재 위치값 - 전의 위치 값
        Vector2 abs = (Vector2)transform.position - prevPosition;
        
        // 절대값
        if(Mathf.Abs(abs.x) > Mathf.Abs(abs.y))
        {
            
            // 왼쪽 또는 오른쪽
            spr.sprite = set[2];
            if(transform.position.x > prevPosition.x) // 오른쪽
            {
                spr.flipX = false;
            }
            else if(transform.position.x < prevPosition.x) // 왼쪽
            {
                spr.flipX = true;
            }
        }
        else
        {
            // 위쪽 또는 아래 
            spr.flipX = false;
            if(transform.position.y > prevPosition.y) // 위
            {
                spr.sprite = set[1];
            }
            else if(transform.position.y < prevPosition.y) // 아래
            {
                spr.sprite = set[0];
            }

        }
    }

    void SetInfo()
    {
        info.name = GameManager.familyName + GameManager.name;
        info.hp = 100;

        info.design = Random.Range(0, 101);
        info.programming = Random.Range(0, 101);
        info.sound = Random.Range(0, 101);
        info.art = Random.Range(0, 101);

        info.salery = Random.Range(100, 1001);

        int r = Random.Range(0, 2);
        info.gender = (Gender)r;
    }

    void ShowInfo()
    {
        Text txt = transform.Find("Canvas/Text_Name").GetComponent<Text>();
        txt.text = info.name;

        Image img = transform.Find("Canvas/Image/Image_Gauge").GetComponent<Image>();
        img.fillAmount = info.hp / 100f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coffee"))
        {
            info.hp = 100;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Employee"))
        {
            Collider2D col1 = GetComponent<Collider2D>();
            Collider2D col2 = collision.collider;
            Physics2D.IgnoreCollision(col1, col2);
        }
    }
}

public enum Gender
{
    Female = 0,
    Male = 1
}

[System.Serializable]
public class Employee
{
    public string name;
    //public int gender;
    public Gender gender;

    // 능력치
    public float design;
    public float programming;
    public float art;
    public float sound;

    public float hp;

    public long salery;
}
