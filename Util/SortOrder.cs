using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortOrder : MonoBehaviour
{

    public SpriteRenderer[] m_SpriteGroup;

    public int sortingOrder = 0;
    public int sortingOrderOrigine = 0;

    private float Update_Tic = 0;
    private float Update_Time = 0.1f;
    private void Awake()
    {
        Init();
    }

    //void Update()
    //{
    //
    //    spriteOrder_Controller();
    //}

    void spriteOrder_Controller()
    {

        Update_Tic += Time.deltaTime;

        if (Update_Tic > 0.1f)
        {
            Order();

            Update_Tic = 0;
        }



    }
    [ContextMenu("Order")]
    public void Init()
    {
        m_SpriteGroup = this.transform.GetComponentsInChildren<SpriteRenderer>(true);
        Order();
    }
    private void Order()
    {
        sortingOrder = Mathf.RoundToInt(this.transform.position.y * 100);
        //Debug.Log("y::" + this.transform.position.y);
          Debug.Log("sortingOrder::" + sortingOrder);
        for (int i = 0; i < m_SpriteGroup.Length; i++)
        {

            int order = Mathf.RoundToInt(m_SpriteGroup[i].transform.position.z * 100);
            //m_SpriteGroup[i].sortingOrder = sortingOrderOrigine - sortingOrder + i;
            m_SpriteGroup[i].sortingOrder = -order;

        }
    }
}

