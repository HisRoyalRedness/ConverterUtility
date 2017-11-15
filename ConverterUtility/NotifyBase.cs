using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections;
using System.Windows.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;

#if REACTIVE
using System.Reactive.Subjects;
using System.Reactive.Linq;
#endif

namespace HisRoyalRedness.com
{
    public abstract class NotifyBase : NotifyBase<object>
    {
        protected NotifyBase()
            : base(null, null)
        { }

        protected NotifyBase(object propertyLock)
            : base(null, propertyLock)
        { }

        protected NotifyBase(Dispatcher dispatcher)
            : base(dispatcher, null)
        { }

        protected NotifyBase(Dispatcher dispatcher, object propertyLock)
            : base(dispatcher, propertyLock)
        { }
    }

    public abstract class NotifyBase<TLock> : INotifyPropertyChanged
        where TLock : class
    {
        protected NotifyBase()
            : this(null, null)
        { }

        protected NotifyBase(TLock propertyLock)
            : this(null, propertyLock)
        { }

        protected NotifyBase(Dispatcher dispatcher)
            : this(dispatcher, null)
        { }

        protected NotifyBase(Dispatcher dispatcher, TLock propertyLock)
        {
            _dispatcher = dispatcher;
            _propertyLock = propertyLock;
        }

        public event PropertyChangedEventHandler PropertyChanged;

#if REACTIVE
        readonly Subject<string> _changes = new Subject<string>();

        public IObservable<string> PropertyChanges
        { get { return _changes; } }

        /// <summary>
        /// Project all echoFrom properties onto echoTo. 
        /// This has the effect of making echoFrom changes appears as echoTo changes as well.
        /// </summary>
        protected void EchoChanges(string echoTo, params string[] echoFrom)
        {
            _changes.Where(prop => echoFrom.Contains(prop)).Subscribe(prop => NotifyPropertyChanged(echoTo));
        }
#endif

        protected TValue GetProperty<TValue>(ref TValue propertyMember)
        {
            if (_propertyLock == null)
                return propertyMember;

            TValue value;
            lock (_propertyLock)
                value = propertyMember;
            return value;
        }

        //[DebuggerStepThrough]
        protected bool SetProperty<TValue>(ref TValue propertyMember, TValue newValue, Action<TValue> actionIfChanged = null, [CallerMemberName]string propertyName = "")
            => SetProperty(ref propertyMember, newValue, _dispatcher, actionIfChanged, propertyName);


        /// <summary>
        /// Attempt to set the property. If the given value is different from the current value,
        /// a PropertyChanged event will be raised.
        /// </summary>
        protected bool SetProperty<TValue>(ref TValue propertyMember, TValue newValue, Dispatcher dispatcher, Action<TValue> actionIfChanged = null, [CallerMemberName]string propertyName = "")
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            var hasChanged = false;
            if (_propertyLock == null)
            {
                hasChanged = HasChanged(ref propertyMember, newValue);
                if (hasChanged)
                    propertyMember = newValue;
            }
            else
            {
                lock (_propertyLock)
                {
                    hasChanged = HasChanged(ref propertyMember, newValue);
                    if (hasChanged)
                        propertyMember = newValue;
                }
            }

            if (hasChanged)
            {
                if (dispatcher == null || dispatcher == Dispatcher.CurrentDispatcher)
                {
                    NotifyPropertyChanged(propertyName);
                    if (actionIfChanged != null)
                        actionIfChanged(newValue);
                }
                else
                {
                    dispatcher.InvokeAsync(() =>
                    {
                        NotifyPropertyChanged(propertyName);
                        if (actionIfChanged != null)
                            actionIfChanged(newValue);
                    });
                }
            }

            return hasChanged;
        }


        protected Task<bool> SetPropertyAsync<TValue>(ref TValue propertyMember, TValue newValue, Dispatcher dispatcher, Action<TValue> actionIfChanged = null, [CallerMemberName]string propertyName = "")
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            var hasChanged = false;
            if (_propertyLock == null)
            {
                hasChanged = HasChanged(ref propertyMember, newValue);
                if (hasChanged)
                    propertyMember = newValue;
            }
            else
            {
                lock (_propertyLock)
                {
                    hasChanged = HasChanged(ref propertyMember, newValue);
                    if (hasChanged)
                        propertyMember = newValue;
                }
            }

            if (hasChanged)
            {
                if (dispatcher == null || dispatcher == Dispatcher.CurrentDispatcher)
                {
                    NotifyPropertyChanged(propertyName);
                    if (actionIfChanged != null)
                        actionIfChanged(newValue);
                }
                else
                {
                    return dispatcher.InvokeAsync(() =>
                    {
                        NotifyPropertyChanged(propertyName);
                        if (actionIfChanged != null)
                            actionIfChanged(newValue);
                        return true;
                    }).Task;
                }
            }

            return Task.FromResult(hasChanged);
        }

        bool HasChanged<TValue>(ref TValue propertyMember, TValue newValue)
            => !(((propertyMember == null && newValue == null) ||
                (propertyMember != null && propertyMember.Equals(newValue))));


        /// <summary>
        /// Raise a PropertyChanged event for this given properties
        /// </summary>
        protected void NotifyPropertyChanged(params string[] properties)
        {
            foreach (var property in properties)
            {
#if REACTIVE
                _changes.OnNext(property);
#endif
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public bool IsInDesigner => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        protected readonly TLock _propertyLock = null;
        protected readonly Dispatcher _dispatcher;

        public class RelayCommand : ICommand
        {
            #region Fields

            readonly Action<object> ExecuteAction;
            readonly Predicate<object> CanExecutePredicate;

            #endregion // Fields

            #region Constructors

            public RelayCommand(Action<object> execute)
                : this(execute, null)
            { }

            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                if (execute == null)
                    throw new ArgumentNullException(nameof(execute));
                ExecuteAction = execute;
                CanExecutePredicate = canExecute;
            }
            #endregion // Constructors

            #region ICommand Members

            [DebuggerStepThrough]
            public bool CanExecute(object parameter) => (CanExecutePredicate == null ? true : CanExecutePredicate(parameter));

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter) => ExecuteAction(parameter);

            #endregion // ICommand Members
        }
    }
}
