using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;
/// <summary>
/// rx딕셔너리의 값 변경 이벤트를 쉽게 처리하기 위한 래퍼
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[Serializable]
public class RxEventDicionaryContainer<TKey, TValue>
{
    [SerializeField] private ReactiveDictionary<TKey, TValue> dic;
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="keyValuePairs">사용할 딕셔너리</param>
    public RxEventDicionaryContainer(Dictionary<TKey, TValue> keyValuePairs)
    {
        if (keyValuePairs == null)
        {
            Debug.LogError("dic is null");
            return;
        }
        if (dic != null)
        {
            Debug.LogError("duplicate set error : " + this);
        }
        dic = keyValuePairs.ToReactiveDictionary();
      

        dic.ObserveAdd().Subscribe(x => m_acChange?.Invoke(x.Key, x.Value));
        dic.ObserveReplace().Subscribe(x => m_acChange?.Invoke(x.Key, x.NewValue));
        dic.ObserveRemove().Subscribe(x => m_acDelete?.Invoke(x.Key));
        dic.ObserveReset().Subscribe(x => m_acDelete?.Invoke(default));
    }
    /// <summary>
    /// 기본 딕셔너리로 리턴
    /// </summary>
    /// <returns></returns>
    public Dictionary<TKey, TValue> MakeNativeDic()
    {
        return new Dictionary<TKey, TValue>(dic);
    }

    private ReactiveDictionary<TKey, TValue> CheckContainsKey(TKey key)
    {
        if (!dic.ContainsKey(key)) dic[key] = default;
        return dic;
    }

    public TValue Get(TKey key)
    {
        return CheckContainsKey(key)[key];
    }

    public TValue Add(TKey key, TValue value)
    {
        dynamic valDic = CheckContainsKey(key)[key];
        dynamic ValParam = value;
        dic[key] = valDic + ValParam;

        return dic[key];
    }

    public TValue Set(TKey key, TValue value)
    {
        CheckContainsKey(key)[key] = value;
        return value;
    }

    private System.Action<TKey, TValue> m_acChange;
    private System.Action<TKey> m_acDelete;

    public ReactiveDictionary<TKey, TValue> AddEvent(System.Action<TKey, TValue> acChange, System.Action<TKey> acDelete)
    {
        if (acChange != null) m_acChange += acChange;
        if (acDelete != null) m_acDelete += acDelete;
        return dic;
    }
}
