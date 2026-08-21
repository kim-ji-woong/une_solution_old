using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.Event
{
    /// <summary>
    /// Observer를 연결하는 Subject인터페이스, 대상 객체가 Object형식인 경우 IClonable이어야 함
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender">대상 subject, 또는 Value를 포함하는 객체</param>
    /// <param name="oChangedValue"></param>
    public delegate void ValueChangeEventHandler<T>(object sender, T oChangedValue);
    public interface IValueSubject<T>
    {
        // 대상 값이 변경되었을때 호출되는 Event, IValueObserver의 OnValueChanged와 연결된다.
        event ValueChangeEventHandler<T> ValueChanged;

        // value가 변경되었을때 처리할 함수, Observer에 OnValueChanged함수를 호출하여 준다.
        void UpdateValueChanged(T value);
           
        // Observer를 등록하고 Event와 연결하여 준다.
        void AddObserver(IValueObserver<T> observer);

        // Observer를 등록 해지하고 Event와 연결 해지한다.
        void RemoveObserver(IValueObserver<T> observer);

        //  Value를 비교하는 함수 : 같으면 0, 크면 1, 작으면 -1
        int CompareTo(T target, T compare);
        
    }

    /// <summary>
    /// Observer Interface : 값 변경을 받을 Observer용
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueObserver<T>
    {
        void OnValueChanged(object sender, T value);
        void OnSubscribed(object target);
        void OnUnsubscribed(object target);
    }

    /// <summary>
    /// 기본형에 대해 ValueSubject를 이용할 수 있도록 사전에 만들어진 보조 클래스 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueSubjectAdapter<T> : IValueSubject<T>, IDisposable
    {
        public event ValueChangeEventHandler<T> ValueChanged;

        private bool m_bAsync = false;
        public bool Async
        {
            get { return m_bAsync; }
            set { m_bAsync = value; }
        }


        private bool m_bSetTargetValue = false;
        private T mTargetValue;        
        /// <summary>
        /// Object 타입인경우 반드시 ICloneable이어야 함
        /// </summary>
        public T TargetValue
        {
            get
            { 
                return mTargetValue;
            }
            
            set 
            {
                mTargetValue = Clone(value);
                m_bSetTargetValue = true;
            }
        }

        private List<IValueObserver<T>> mObservers = new List<IValueObserver<T>>();
        private bool m_bDisposing = false;
       
        public ValueSubjectAdapter()
        {
        }

        public ValueSubjectAdapter(T targetValue)
        {
            mTargetValue = targetValue;
            m_bSetTargetValue = true;
        }
        
        public void Dispose()
        {
            m_bDisposing = true;

            List<IValueObserver<T>> arClone = new List<IValueObserver<T>>(mObservers);
            foreach (IValueObserver<T> observer in arClone)
            {
                mObservers.Remove(observer);
                ValueChanged -= observer.OnValueChanged;
            }            
        }

        /// <summary>
        /// 실제 값이 변경되는 곳에서 호출하여 주는 함수
        /// </summary>
        /// <param name="value">변경된 Value</param>
        public virtual void UpdateValueChanged(T value)
        {
            // Dispose 중이라면 더이상 Value Update를 수행하지 않는다.
            if (m_bDisposing == true)
                return;

            // 감시 value가 설정되지 않은 경우 Update를 수행하지 않는다.
            if (m_bSetTargetValue == false)
                return;

            if (CompareTo(mTargetValue, value) != 0)
            {
                if (ValueChanged != null)
                {
                    Action action = new Action(() => ValueChanged(this, value));
                    Task t = new Task(action);

                    if (m_bAsync == true)
                        t.Start();
                    else
                        t.RunSynchronously();
                }

                mTargetValue = Clone(value);
            }
        }

        /// <summary>
        /// Observer 등록
        /// </summary>
        /// <param name="observer">Value변경을 받을 observer</param>
        public virtual void AddObserver(IValueObserver<T> observer)
        {
            if (observer == null)
                return;

            if (!mObservers.Contains(observer))
            {
                mObservers.Add(observer);
                ValueChanged += observer.OnValueChanged;

                observer.OnSubscribed(this);
            }
        }

        /// <summary>
        /// Observer 등록 해지
        /// </summary>
        /// <param name="observer">해지할 observer</param>
        public virtual void RemoveObserver(IValueObserver<T> observer)
        {
            if (observer == null)
                return;

            if (mObservers.Contains(observer))
            {
                mObservers.Remove(observer);
                ValueChanged -= observer.OnValueChanged;

                observer.OnUnsubscribed(this);
            }
        }

        /// <summary>
        /// 값이 같은지 비교하는 함수
        /// </summary>
        /// <param name="target">비교 원본</param>
        /// <param name="compare">비교 대상</param>
        /// <returns>같으면 0, 크면 1, 작으면 -1</returns>
        public virtual int CompareTo(T target, T compare)
        {
            return Comparer<T>.Default.Compare(target, compare);
        }

        private T Clone(T what)
        {
            var castWhat = what as ICloneable;
            if (castWhat != null)
                return (T)castWhat.Clone();
            else
                return what;
        }
    }
}
