using System;
using UniRx;
using UnityEngine;

/// <summary>
/// 카운트 구성 요소
/// </summary>
public class RxCountDownTimer : MonoBehaviour
{
    /// <summary>
    /// 카운트 다운 스트림
    /// 이 Observable을 각 클래스가 Subscribe 한다.
    /// </summary>
    public IObservable<int> CountDownObservable => _countDownObservable.AsObservable();

    private IConnectableObservable<int> _countDownObservable;

    private float time = 2f;

    public RxCountDownTimer Init()
    {
        _countDownObservable = CreateCountDownObservable(time).Publish();
        _countDownObservable.Connect();
        return this;
    }


    /// <summary>
    /// countTime 만큼 카운트 다운하는 스트림
    /// </summary>
    /// <param name="countTime"></param>
    /// <returns></returns>
    private IObservable<int> CreateCountDownObservable(float countTime) =>

        Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Select(x => (int) (countTime - x))
            .TakeWhile(x => x > 0);
}